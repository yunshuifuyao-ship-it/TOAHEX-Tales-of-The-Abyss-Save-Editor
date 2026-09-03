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

        /// <summary>主存档头部长度（0x214），写入头部字段 @0x0C</summary>
        private const int HeaderSize = 0x214;

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

            // native 覆盖区：尾部静态区 0xB2C8..0xB800
            // 3DS 此处为运行时菜单结构/静态浮点配置（含疑似摄像机参数），
            // 从 PS2 复制会破坏 3DS 摄像机，必须回写 3DS（native）模板值
            Buffer.BlockCopy(template, 0xB2C8, dst, 0xB2C8, 0xB800 - 0xB2C8);

            // native 覆盖区：0x2284 处浮点（若映射后与模板不一致，则强制回写模板原始字节）
            if (BitConverter.ToSingle(dst, 0x2284) != BitConverter.ToSingle(template, 0x2284))
            {
                Buffer.BlockCopy(template, 0x2284, dst, 0x2284, 4);
            }

            // native 覆盖区：静态表 10384..10864（480 字节）与 11000 处 4 字节，强制使用 3DS 模板值
            Buffer.BlockCopy(template, 10384, dst, 10384, 480);
            Buffer.BlockCopy(template, 11000, dst, 11000, 4);

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
