using System;
using System.IO;
using System.Reflection;

namespace TOAHEX
{
    /// <summary>
    /// PS2 → 3DS 存档转换器。
    /// 由 convert_ps2_to_3ds.py 逐行移植，输出与 Python 版逐字节一致。
    /// 自包含实现（不引用 SaveOffsets / ChecksumHelper），全部常量内嵌。
    /// </summary>
    public static class Ps2To3dsConverter
    {
        /// <summary>PS2 主存档大小（0xBFC8）</summary>
        public const int Ps2MainSize = 49096;   // 0xBFC8
        /// <summary>PS2 系统存档大小（0x728）</summary>
        public const int Ps2SysSize = 1832;     // 0x728
        /// <summary>3DS 主存档大小（0xBFE0）</summary>
        public const int DsMainSize = 49120;    // 0xBFE0
        /// <summary>3DS 系统存档大小（0x744）</summary>
        public const int DsSysSize = 1860;      // 0x744

        /// <summary>
        /// 尾部 B 段（0xB50C..0xB800 = 字段/摄像机参考区）映射分段：
        /// (3DS 起, 3DS 止, delta)，PS2 源偏移 = 3DS 偏移 - delta。
        /// 实测（本 PS2 档 ↔ TotA15 四份原生 3DS 档）该区整体 Δ=+24，但中间夹一段 3DS 专属插入：
        ///   3DS 0xB530/0xB534 恒为 0x01010101×2（四档一致，PS2 无对应）→ 保留 native；
        ///   其后 0xB538..0xB5D0 窗口 Δ 变为 +32（PS2 侧多出的 0xB5B0..0xB5B8 两个 dword 被丢弃）。
        /// 分段后与原生档逐 dword 吻合度 174/189（朴素 +24 仅 151/189），
        /// 且玩家位置副本恰好落在原生档的 0xB568/0xB574(X)、0xB57C(Z)。
        /// </summary>
        private static readonly (int Start, int End, int Delta)[] TailFieldSegments =
        {
            (0xB50C, 0xB530, 24),   // → PS2 0xB4F4..0xB518
            (0xB538, 0xB5D0, 32),   // → PS2 0xB518..0xB5B0
            (0xB5D0, 0xB800, 24)    // → PS2 0xB5B8..0xB7E8
        };

        /// <summary>3DS 专属插入区（0xB530..0xB538，无 PS2 对应）→ 整段保留 3DS(native)。</summary>
        private const int TailDsOnlyInsertStart = 0xB530;
        private const int TailDsOnlyInsertEnd = 0xB538;

        /// <summary>
        /// 4×120B 队伍编成预设块（3DS 10384..10864 = runtime+10084）。
        /// 实测两版结构完全同构、文件 Δ 与主体一致(+4)；唯一差异是每条预设的 16 字节"预设名"
        /// 文字编码（PS2 Shift-JIS ↔ 本地化 3DS ASCII）→ 名称保留 3DS(native) 以免乱码，
        /// 其余字节从 PS2 带入以保留玩家自定义编成。
        /// 若目标 3DS 为日版（名称同为 Shift-JIS），把 <see cref="PresetKeepNameFromTemplate"/> 设为 false。
        /// </summary>
        private const int PresetRecordBase = 10384;
        private const int PresetRecordSize = 120;
        private const int PresetRecordCount = 4;
        private const int PresetNameSize = 16;
        private const bool PresetKeepNameFromTemplate = true;

        /// <summary>
        /// 136B + 168B 块（3DS 10864..11168 = runtime+10564 / +10700）
        /// 复核（2026-10-05 修正）：**并非 3DS 专有** —— PS2 打包 sub_37D5C0 由 obj+10540..10796
        /// 写入 PS2 文件 10868..11172，PS2 加载 sub_37BDF0 亦恢复同区，是双方 1:1 真字段块
        /// （文件 Δ 与主体一致 -4；源偏移 obj+10540↔rt+10564、obj+10676↔rt+10700，源 Δ=+24）。
        /// => 由主体 +4 复制带入即可，不做 native 覆盖（旧版仅覆盖首 4 字节 11000 同样多余）。
        /// 注：本 PS2 档该区全为 0；TotA15 三份中后期 3DS 档在 168B 块内为同一组参考值
        /// (0x2B00..0x2B0F)，早期档为 0 —— 属 3DS 端状态差异，非结构性差异。
        /// </summary>
        private const int PresetBlockRegion = 10864;   // 仅作文档记录，当前无需特判

        /// <summary>主存档头部长度（0x214），写入头部字段 @0x0C</summary>
        private const int HeaderSize = 0x214;

        /// <summary>
        /// 尾部 A 段结束（0xB50C）。0xB2C8..0xB50C 为运行时结构：3DS 侧密集存放 3DS 堆指针
        /// （0x0048xxxx / 0x0879xxxx），PS2 侧同槽位是索引/小整数，两版数据模型不同，
        /// 只能保留 3DS(native) 模板值。
        /// </summary>
        private const int TailPointerArrayEnd = 0xB50C;

        /// <summary>
        /// 尾部 B 段结束（0xB800）。0xB50C..0xB800 为字段/摄像机参考区（PS2 侧 0xB4F4..0xB7E8），
        /// 两版同构、整体 Δ=+24（实测 PS2 0xB600/0xB700/0xB7A0 ↔ 3DS 0xB618/0xB718/0xB7B8 逐 dword 全等），
        /// 含玩家/摄像机参考坐标。旧版整段回写模板值是"摄像机坐标不对应"的根因。
        /// </summary>
        private const int TailFieldRegionEnd = 0xB800;

        /// <summary>
        /// 平台指针带（0x00100000..0x09000000），同时覆盖 3DS 主存/堆指针（0x0048xxxx / 0x0879xxxx）
        /// 与 PS2 主存指针（0x001xxxxx）。落在该带内的槽视为"平台指针槽"，两侧地址不可迁移。
        /// </summary>
        private const uint PointerBandLo = 0x00100000;
        private const uint PointerBandHi = 0x09000000;

        private static bool IsPlatformPointer(uint value)
        {
            return value >= PointerBandLo && value < PointerBandHi;
        }

        /// <summary>
        /// 尾部字段区（0xB50C..0xB800）指针槽保护：
        /// 任一侧落在指针带内（模板侧 = 3DS 指针；PS2 源侧 = PS2 指针），
        /// 该槽一律还原为 3DS(native) 模板值，避免把 PS2 地址写进 3DS 指针槽；
        /// 其余槽位保留来自 PS2 的数据（分段复制已按对应 delta 带入）。
        /// </summary>
        private static void RestorePointerSlots(byte[] dst, byte[] template, byte[] ps2Src,
                                                int start, int end, int delta)
        {
            for (int off = start; off < end; off += 4)
            {
                if (IsPlatformPointer(BitConverter.ToUInt32(template, off)) ||
                    IsPlatformPointer(BitConverter.ToUInt32(ps2Src, off - delta)))
                {
                    Buffer.BlockCopy(template, off, dst, off, 4);
                }
            }
        }

        // 与 Python DS_MAIN_OPTIONS 完全一致（36字节，写入 3DS 主档 43980..44016 的 3DS 专属选项区）
        private static readonly byte[] DsMainOptions =
        { 0x02,0x00,0x00,0x00, 0x00,0x08,0x00,0x00, 0x01,0x00,0x00,0x00, 0x00,0x04,0x00,0x00,
          0x00,0x01,0x00,0x00, 0x10,0x00,0x00,0x00, 0x20,0x00,0x00,0x00, 0x00,0x02,0x00,0x00,
          0x80,0x00,0x00,0x00 };

        // 与 Python DS_SYSTEM_OPTIONS 完全一致（36字节，写入 3DS 系统档 1552..1588 的 3DS 专属选项区）
        private static readonly byte[] DsSysOptions =
        { 0x01,0x00,0x00,0x00, 0x02,0x00,0x00,0x00, 0x00,0x08,0x00,0x00, 0x00,0x04,0x00,0x00,
          0x00,0x01,0x00,0x00, 0x20,0x00,0x00,0x00, 0x00,0x02,0x00,0x00, 0x80,0x00,0x00,0x00,
          0x00,0x00,0x00,0x00 };

        /// <summary>
        /// 从程序集内嵌资源加载 3DS 主存档模板。
        /// 清单名 = RootNamespace(Resources 目录路径)文件名 = TOAHEX.Resources.3ds_template.bin。
        /// </summary>
        public static byte[] LoadEmbeddedTemplate()
        {
            Stream stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("TOAHEX.Resources.3ds_template.bin");
            if (stream == null)
            {
                throw new InvalidOperationException(
                    "找不到内嵌资源 TOAHEX.Resources.3ds_template.bin，请确认项目已将其作为 EmbeddedResource 嵌入。");
            }
            using (stream)
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                byte[] data = ms.ToArray();
                if (data.Length != DsMainSize)
                {
                    throw new InvalidOperationException(string.Format(
                        "内嵌 3DS 模板大小错误：期望 {0} 字节，实际 {1} 字节。", DsMainSize, data.Length));
                }
                return data;
            }
        }

        /// <summary>
        /// 将 PS2 主存档转换为 3DS 主存档。映射顺序与 Python convert_main 完全一致。
        /// </summary>
        public static byte[] ConvertMain(byte[] ps2Main, byte[] template)
        {
            if (ps2Main == null)
            {
                throw new ArgumentException("PS2 主存档数据不能为 null。");
            }
            if (template == null)
            {
                throw new ArgumentException("3DS 模板数据不能为 null。");
            }
            if (ps2Main.Length != Ps2MainSize)
            {
                throw new ArgumentException(string.Format(
                    "PS2 主存档大小错误：期望 {0} 字节，实际 {1} 字节。", Ps2MainSize, ps2Main.Length));
            }
            if (template.Length != DsMainSize)
            {
                throw new ArgumentException(string.Format(
                    "3DS 模板大小错误：期望 {0} 字节，实际 {1} 字节。", DsMainSize, template.Length));
            }

            // 以 3DS 模板克隆为底稿：native 覆盖区之外未被映射的区域天然保留模板值
            byte[] dst = (byte[])template.Clone();

            // 同位区：头部标识区（PS2 0..1320 → 3DS 0..1320，两版同偏移直接复制）
            Buffer.BlockCopy(ps2Main, 0, dst, 0, 1320);

            // +4 区：主体存档区（PS2 1324..43984 → 3DS 1320..43980，PS2 相对 3DS 整体多 4 字节偏移）
            Buffer.BlockCopy(ps2Main, 1324, dst, 1320, 42660);

            // 3DS 专属区：选项区 43980..44016（PS2 无对应数据，写入 3DS 固定 options 36 字节）
            Buffer.BlockCopy(DsMainOptions, 0, dst, 43980, 36);

            // -24 区：图鉴/事件区前段（PS2 43992..44072 → 3DS 44016..44096，PS2 相对 3DS 少 24 字节偏移）
            Buffer.BlockCopy(ps2Main, 43992, dst, 44016, 80);

            // -24 区：尾部区（PS2 44072..49096 → 3DS 44096..49120，两档尾部均到达文件末尾）
            Buffer.BlockCopy(ps2Main, 44072, dst, 44096, Ps2MainSize - 44072);

            // -4 微调区：地图/坐标字段（PS2 相对 3DS 少 4 字节：0x52C→0x528、0x530→0x52C、0x534→0x530）
            Buffer.BlockCopy(ps2Main, 0x52C, dst, 0x528, 4);
            Buffer.BlockCopy(ps2Main, 0x530, dst, 0x52C, 4);
            Buffer.BlockCopy(ps2Main, 0x534, dst, 0x530, 0x10);

            // native 覆盖区：尾部 A 段 0xB2C8..0xB50C
            // 3DS 此处为运行时结构（密集 3DS 堆指针 0x0048xxxx / 0x0879xxxx），PS2 同槽位为索引/小整数。
            // 从 PS2 复制会把 PS2 地址写进 3DS 指针槽 → 必须回写 3DS（native）模板值。
            Buffer.BlockCopy(template, 0xB2C8, dst, 0xB2C8, TailPointerArrayEnd - 0xB2C8);

            // 尾部 B 段 0xB50C..0xB800（字段/摄像机参考区，PS2 源区 0xB4F4..0xB7E8）：
            // 两版同构但 Δ 分段（见 TailFieldSegments），含玩家/摄像机参考坐标（玩家 X/Z 在此区重现）。
            // 旧版把整段回写成模板值是"玩家坐标对、摄像机坐标不对"的根因。
            foreach (var (segStart, segEnd, delta) in TailFieldSegments)
            {
                Buffer.BlockCopy(ps2Main, segStart - delta, dst, segStart, segEnd - segStart);
                RestorePointerSlots(dst, template, ps2Main, segStart, segEnd, delta);
            }

            // 3DS 专属插入区（0xB530..0xB538）整段保留 3DS(native)
            Buffer.BlockCopy(template, TailDsOnlyInsertStart, dst, TailDsOnlyInsertStart,
                             TailDsOnlyInsertEnd - TailDsOnlyInsertStart);

            // 0x2284：原"若与模板不一致则强制回写模板"的浮点特判已删除（2026-10-05 复核）：
            //   3DS 0x2284 = PS2 0x2288 = 800.0（两版恒为常量 0x44480000），主体 +4 复制已正确落位，
            //   该特判在实测四档中恒为 no-op。

            // 4×120B 队伍编成预设块：非名称部分从 PS2 带入，名称字段保留 3DS(native) 以免编码乱码。
            for (int k = 0; k < PresetRecordCount; k++)
            {
                int rec = PresetRecordBase + PresetRecordSize * k;
                Buffer.BlockCopy(ps2Main, rec + PresetNameSize + 4, dst,
                                 rec + PresetNameSize, PresetRecordSize - PresetNameSize);
                if (PresetKeepNameFromTemplate)
                {
                    Buffer.BlockCopy(template, rec, dst, rec, PresetNameSize);
                }
            }

            // 136B+168B 块（3DS 10864..11168）为双方 1:1 真字段块（PS2 sub_37D5C0 存 / sub_37BDF0 取），
            // 已由上方主体 +4 复制正确带入，故不再做 native 覆盖。

            // 头部字段：@0x0C = 头部长度 0x214；@0x10 = 头部校验和；@0x14 = 主体校验和
            PutU32(dst, 0x0C, 0x214);
            PutU32(dst, 0x10, WordSum(dst, 0x20, 0x1F4));
            PutU32(dst, 0x14, WordSum(dst, 0x214, DsMainSize - 0x214));

            return dst;
        }

        /// <summary>
        /// 将 PS2 系统存档转换为 3DS 系统存档。映射顺序与 Python convert_system 完全一致。
        /// </summary>
        public static byte[] ConvertSystem(byte[] ps2Sys)
        {
            if (ps2Sys == null)
            {
                throw new ArgumentException("PS2 系统存档数据不能为 null。");
            }
            if (ps2Sys.Length != Ps2SysSize)
            {
                throw new ArgumentException(string.Format(
                    "PS2 系统存档大小错误：期望 {0} 字节，实际 {1} 字节。", Ps2SysSize, ps2Sys.Length));
            }

            // 3DS 系统档以零填充新建（系统档无模板底稿）
            byte[] dst = new byte[DsSysSize];

            // 同位区：系统数据前段（PS2 0..1552 → 3DS 0..1552）
            Buffer.BlockCopy(ps2Sys, 0, dst, 0, 1552);

            // 3DS 专属区：选项区 1552..1588（PS2 无对应数据，写入 3DS 固定 options 36 字节）
            Buffer.BlockCopy(DsSysOptions, 0, dst, 1552, 36);

            // +28 区：系统数据中段（PS2 1560..1640 → 3DS 1588..1668）
            Buffer.BlockCopy(ps2Sys, 1560, dst, 1588, 80);

            // +28 区：系统数据后段（PS2 1640..1704 → 3DS 1668..1732）
            Buffer.BlockCopy(ps2Sys, 1640, dst, 1668, 64);

            // +28 区：尾部（PS2 1704..1832 → 3DS 1732..1860，两档尾部均到达文件末尾）
            Buffer.BlockCopy(ps2Sys, 1704, dst, 1732, Ps2SysSize - 1704);

            // 头部字段：@0x00 = 全档校验和（数据区 8..1860 的小端 u32 累加和）
            PutU32(dst, 0, WordSum(dst, 8, DsSysSize - 8));

            return dst;
        }

        /// <summary>
        /// 校验 3DS 主存档：长度、头部校验和（@0x10）与主体校验和（@0x14）。
        /// </summary>
        public static void VerifyMain(byte[] dsMain)
        {
            if (dsMain == null)
            {
                throw new InvalidOperationException("3DS 主存档数据不能为 null。");
            }
            if (dsMain.Length != DsMainSize)
            {
                throw new InvalidOperationException(string.Format(
                    "3DS 主存档长度错误：期望 {0} 字节，实际 {1} 字节。", DsMainSize, dsMain.Length));
            }
            if (BitConverter.ToUInt32(dsMain, 0x10) != WordSum(dsMain, 0x20, 0x1F4))
            {
                throw new InvalidOperationException("3DS 主存档头部校验和（@0x10）不匹配。");
            }
            if (BitConverter.ToUInt32(dsMain, 0x14) != WordSum(dsMain, 0x214, DsMainSize - 0x214))
            {
                throw new InvalidOperationException("3DS 主存档主体校验和（@0x14）不匹配。");
            }
        }

        /// <summary>
        /// 校验 3DS 系统存档：长度与 @0x00 全档校验和。
        /// </summary>
        public static void VerifySystem(byte[] dsSys)
        {
            if (dsSys == null)
            {
                throw new InvalidOperationException("3DS 系统存档数据不能为 null。");
            }
            if (dsSys.Length != DsSysSize)
            {
                throw new InvalidOperationException(string.Format(
                    "3DS 系统存档长度错误：期望 {0} 字节，实际 {1} 字节。", DsSysSize, dsSys.Length));
            }
            if (BitConverter.ToUInt32(dsSys, 0) != WordSum(dsSys, 8, DsSysSize - 8))
            {
                throw new InvalidOperationException("3DS 系统存档校验和（@0x00）不匹配。");
            }
        }

        /// <summary>
        /// 小端 u32 累加和：对 [offset, offset+count) 每 4 字节取小端 u32 累加并回绕（& 0xFFFFFFFF）。
        /// 与 Python word_sum / ChecksumHelper.WordSum 语义一致（局部实现，避免并行任务冲突）。
        /// </summary>
        private static uint WordSum(byte[] data, int offset, int count)
        {
            uint sum = 0;
            int end = offset + count;
            for (int pos = offset; pos < end; pos += 4)
            {
                sum += BitConverter.ToUInt32(data, pos);
            }
            return sum;
        }

        /// <summary>
        /// 以小端字节序写入 u32。
        /// </summary>
        private static void PutU32(byte[] data, int offset, uint value)
        {
            data[offset] = (byte)(value & 0xFF);
            data[offset + 1] = (byte)((value >> 8) & 0xFF);
            data[offset + 2] = (byte)((value >> 16) & 0xFF);
            data[offset + 3] = (byte)((value >> 24) & 0xFF);
        }
    }
}
