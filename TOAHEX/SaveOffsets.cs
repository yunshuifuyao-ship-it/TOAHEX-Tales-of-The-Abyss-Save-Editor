using System;

namespace TOAHEX
{
    public static class SaveOffsets
    {
        public const int TOA_XXX_SIZE = 49120;
        public const int TOASYS_SIZE = 1860;

        public const int HEADER_VERSION = 0x0000;
        // 0x24 是存档槽号（游戏 sub_37C948: v6[9]=slot，用于 /TOA_%03d 路径），不是难度
        public const int HEADER_SLOT_NUMBER = 0x0024;
        public const int HEADER_IDENT = 0x0008;
        public const int HEADER_DATA_OFFSET = 0x000C;
        public const int HEADER_CHECKSUM = 0x0010;
        public const int BODY_CHECKSUM = 0x0014;

        public const int HEAD_DATA_START = 0x0020;
        public const int HEAD_DATA_SIZE = 500;
        public const int BODY_DATA_START = 0x0214;
        public const int BODY_DATA_SIZE = 48588;

        public const int HEAD_VERSION = 0x0020;
        // 注：0x24(HEAD) 为存档槽号；body 0x51A 为保存时的 word[485] 快照（队伍相关标志），
        // 读档时被游戏忽略（word[485] 改由 0x518 恢复）。
        public const int HEAD_ENCOUNTER = 0x0038;
        public const int HEAD_HIT = 0x003C;
        public const int HEAD_PARTY_COUNT = 0x0028;
        public const int HEAD_GALD_COPY = 0x002C;
        public const int HEAD_PLAYTIME_COPY = 0x0030;
        public const int HEAD_PARTY_ORDER = 0x0044;
        // 头部地图信息（BuildToaSaveBuffer sub_37C948：v6[13]=runtime+988 当前地图ID，
        // 0x4C=maptable 显示名（sub_2D8E14 纯 strcpy 原样拷贝，TBL 编码）。每次游戏内存档会重写。
        // 剧情跳跃改地图后必须同步 0x34+0x4C，否则存档/读取界面仍显示旧地图。
        public const int HEAD_MAP_ID = 0x0034;           // u32 当前地图ID（与 body 0x528 同步）
        public const int HEAD_LOCATION_NAME = 0x004C;    // 32B 显示名（TBL 编码，0x00 截断）

        public const int BODY_GALD = 0x051C;
        public const int BODY_PLAYTIME = 0x0520;
        public const int BODY_PARTY_COUNT_COPY = 0x0518; // u16 队伍人数镜像（读档恢复到 word[485]）
        // ⚠ 旧 BODY_FEATURE_FLAGS = 0x052C 已删除（2026-09-03 IDA 复核）：
        //   3DS 0x52C 实为 u32 地图切换计数（见 BODY_MAP_TRANSITION_COUNT），PS2 0x52C 则是地图 ID。
        //   写它对功能解锁完全无效。C·コア / FSチャンバー 的真实解锁由全局 flag 位图(0x218)中的
        //   "教程完成 flag"控制（详见 GLOBAL_FLAG_C_CORE / GLOBAL_FLAG_FS_CHAMBER 注释）。
        public const int BODY_ENCOUNTER = 0x229C;
        public const int BODY_HIT = 0x22B0;
        public const int BODY_PARTY_ORDER = 0x07C4;
        public const int BODY_PARTY_ORDER_COUNT = 8;
        // 领队（0x7C3，runtime+1656）：u8 角色ID（1=卢克 2=缇娅 3=杰德 4=阿妮丝 5=凯 6=娜塔莉亚，0视为1）。
        // 用户实测写此字节改领队生效；游戏侧写入点为队伍编成菜单 sub_39A7F0
        // （写 +1656 = GetGlobalFlag(菜单基址676+选中项) = 所选角色ID）；读档 LoadToaSaveBuffer(sub_3A7C24)
        // 恢复 +1656 ← body[1455]，摘要块只覆盖 runtime[0..115]，+1656 超出范围 → 单写 0x7C3 即生效。
        public const int BODY_LEADER = 0x07C3;  // u8 领队角色ID（1=卢克..6=娜塔莉亚，0视为1；单写即生效）
        // 难度（BuildToaSaveBuffer sub_37C948 保存，LoadToaSaveBuffer sub_3A7C24 加载）：
        //   body[0x5BC]（文件 0x7D0）← 运行时难度专用字节（runtime[39]）
        //   文件 0xABF3 ← 0xABCC 起 116 字节摘要块内偏移 39 处的难度副本
        // LoadToaSaveBuffer 执行顺序：先 runtime[39]=body[1468](0x7D0)，
        // 之后 memcpy(runtime, body+43448, 116) 用摘要块整体覆盖 runtime[0..115]，
        // 第 2 步覆盖第 1 步 → 读档后难度最终生效值来自 0xABF3，写入必须双写两处。
        // 实测 7 份原生存档（含四难度）0x7D0 与 0xABF3 恒等。
        // 难度枚举：0=普通，1=困难，2=狂热，3=未知。
        // 注意：0x7CF（全局flag11）恒为 2 的战斗初始化标志（sub_32D6A4 每次地图加载重置），不是难度镜像，切勿写入；
        // 0x7C3（runtime+1656）= 领队角色ID（用户实测写此字节改领队生效，游戏侧写入点 sub_39A7F0，
        // 读档 LoadToaSaveBuffer 恢复 +1656 ← body[1455]，无摘要块覆盖问题，见上方 BODY_LEADER）；
        // 0x7C2（runtime+1655）= 战斗Tier栈备份槽（ScriptCmd_SetBattleTier 参数 999999 时把 +1656 的值
        // 备份到 +1655），非副领队——游戏无副领队机制，勿动。
        public const int BODY_DIFFICULTY = 0x07D0;            // u8 难度（0=普通 1=困难 2=狂热 3=未知）
        public const int BODY_DIFFICULTY_SUMMARY = 0xABF3;   // u8 摘要块内难度副本（0xABCC+39），读档时覆盖 0x7D0 后生效
        public const int BODY_ITEM_ARRAY = 0x0542;
        public const int BODY_ITEM_COUNT = 640;

        // ============ 收集/图鉴（2026-08-28 IDA MCP 重审计定案） ============
        // 旧"日志全开"写 0x224~0x500 全 FF —— 该区间位于全局 flag 位图(0x218~0x518, 768B)内部，
        // 覆盖剧情/支线进度 flag(96~5951)，全 FF = 互斥剧情状态同时成立 → 读档黑屏。严禁再整段填充。
        // 聊天(チャット)收集位图：bit 512+N（即 file 0x418+N/8），N=0..537 共 538 个条目
        //   （sub_2E98A4 读 / sub_2DB480 写 / sub_2E98CC 完成度计数，表 word_4F0C24）。
        //   ⚠ 该位图属于聊天收集、不属于"日记"（Example/TOA_000 日记全开参考档未动它），
        //   且置位后已 seen 的聊天不再触发，有副作用 → "日志全开"按钮不写。常量仅作知识记录。
        // 道具图鉴登记位：BOOK_EXTRA(file 0xBD10) 每道具 1 字节，bit0=已获得/登记
        //   （SetItemQtyWithClamp sub_2F366C：数量≠0 时 |=1；读档时对持有道具重新登记）
        public const int CHAT_SEEN_BITMAP = 0x0418;        // 68B（538 bit）全 FF = 聊天记录全开（勿随日志全开写入）
        public const int CHAT_SEEN_BITMAP_SIZE = 68;

        // ===== 日记（ライブラリ）条目解锁表（2026-08-28 Example 差分 + IDA 复核修正旧误判）=====
        // BOOK_SUB(file 0xBAD0) 实为"日记"条目解锁计数表，并非道具图鉴来源数：
        //   sub_2DE8CC/sub_2D6488（日记菜单）按 BOOK_SUB[i] 枚举第 i 条日记的可见文本页数
        //   （0=锁定，1..N=前 N 页，0xFF=全部显示）；新游戏 sub_2EA314 清零；
        //   NG+ 收集继承 sub_171D80(bit 0x4000000) 整段 256B 拷贝；脚本命令(sub_19E054)直接写值。
        //   条目总数 = dword_4F0508 = 114（Example/TOA_000 日记全开参考档恰为前 114 字节全 FF）。
        //   旧结论"全 FF=图鉴全空"有误：0xFF 是"全部页可见"，非空哨兵。
        public const int DIARY_ENTRY_FLAGS = 0xBAD0;       // 日记条目解锁表（=BOOK_SUB 前 114B）
        public const int DIARY_ENTRY_COUNT = 114;

        // ===== 日记全开联动的脚本变量 type 标记（Example/TOA_000 实测差分）=====
        // 变量条目布局（LoadToaSaveBuffer body+0x298D ↔ file 0x2BA1）：
        //   [type u16 @0x2BA1+8N][高位u16 @0x2BA3+8N][value u32 @0x2BA5+8N]，type=0x0200 int/0x0300 float。
        // 日记全开参考档把以下 121 个变量的 type 高 16 位从 0x0000 置为 0x3F80（value 不变）：
        //   var 154-269（值 100~2000，Grade 商店项/价格）、338/339/340、563、621。
        // 与参考档逐字节对齐（Type u32: 0x00000200 → 0x3F800200）。
        public const int SCRIPT_VAR_ENTRY_BASE = 0x2BA1;  // var N 条目起始
        public const int SCRIPT_VAR_MARK_HIGH = 0x3F80;   // type 高16位解锁标记
        public static readonly int[] DIARY_MARK_VARS = new int[]
        {
            154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168,
            169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183,
            184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198,
            199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213,
            214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228,
            229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243,
            244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258,
            259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269,
            338, 339, 340, 563, 621
        };

        // ===== 地图全开（Example/TOA_002 差分 + TotA15 四档交叉验证）=====
        // 全局 flag 位图(0x218)中 flag 600-618(19个) + 650-686(37个) 共 56 个 = 世界地图地名全开。
        // TotA15 全部四份存档（含章1早期档）这 56 个 flag 恒为同一集合，与"只地图全开"参考档完全一致。
        // flag 619-649 不属于地图数据（所有参考档均未置位），严禁写入。
        public const int MAP_UNLOCK_FLAG_RANGES_LO1 = 600;   // 第一段起始 flag（含）
        public const int MAP_UNLOCK_FLAG_RANGES_HI1 = 618;   // 第一段结束 flag（含）
        public const int MAP_UNLOCK_FLAG_RANGES_LO2 = 650;   // 第二段起始 flag（含）
        public const int MAP_UNLOCK_FLAG_RANGES_HI2 = 686;   // 第二段结束 flag（含）

        // 图鉴四段布局（与游戏 sub_37C948 保存 / sub_3A7C24 加载逐段对应）
        // ⚠ 实测四段中只有 EXTRA 可安全写（bit0 登记）；其余三段并非纯收集位图：
        //   MAIN(0xB2D0)=运行时菜单结构原样转储（含堆指针/计时器，每次存档自然变化，勿写）；
        //   SUB(0xBAD0)=日记条目解锁表（见上方 DIARY_ENTRY_FLAGS，前 114B 可安全写 0xFF）；
        //   DETAIL(0xBBD0)=静态属性配置（四份不同进度存档完全相同），写 0x01=损坏。
        public const int BOOK_MAIN_FLAGS_OFFSET = 0xB2D0; // 2048B 主 flags（运行时全局+315740）
        public const int BOOK_MAIN_FLAGS_SIZE = 0x0800;
        public const int BOOK_SUB_FLAGS_OFFSET = 0xBAD0;   // 256B（运行时全局+317788）
        public const int BOOK_SUB_FLAGS_SIZE = 0x0100;
        public const int BOOK_DETAIL_DATA = 0xBBD0;        // 320B（运行时全局+250076，640道具×4bit）
        public const int BOOK_DETAIL_DATA_SIZE = 0x0140;
        public const int BOOK_EXTRA_DATA_OFFSET = 0xBD10;  // 720B（运行时全局+250396，每道具1B，bit0=登记）
        public const int BOOK_EXTRA_DATA_SIZE = 0x02D0;
        public const int BOOK_ITEM_REGISTER_COUNT = 640;   // 登记位有效道具数（0..639）

        // ===== 道具图鉴全开安全规则（2026-08-28 排查黑屏新增，二次修订）=====
        // 道具图鉴 = EXT[id] bit0 登记（Load 会遍历 640 道具对 qty>0 自动 |=1，SetItemQtyWithClamp
        // 改数量时同步 |=1，跨周目继承）。全开登记应把"真实存在"的道具全部置位；图鉴渲染假定
        // bit0=已获得 的道具必有实体配置。以下 ID 为游戏内"无实体/占位"条目（DAT 道具表有记录名
        // 但 ptr4 分类为空、通关档从未持有），置位会使道具图鉴页读空配置崩溃黑屏，必须跳过：
        //   0（空槽占位）、43-51（黄/蓝/红/绿/白/黑谱石占位 + 攻击道具预备1-3）、
        //   561-563（人偶预备1-3）、566（敌人图鉴占位）、569（通灵指环占位）、
        //   617（幻想的音盘占位）、625（漆黑之翼的预告信占位）、631-639（道具表未收录）。
        // ⚠ 二次修订：216「金属利刃」(武器：剑) 与 619「应援俱乐部会报」(道具：贵重品) 是真实道具，
        //   之前误把 EXT 值 0x00 当"无实体"跳过了它们 → 图鉴显示问号，现已从集合中移除。
        // 另：发动"发道具补数量"时必须跳过剧情贵重品 ID 531「罗蕾莱的宝珠」（贵重品：纹章），
        // 提前持有会触发剧情判定异常。
        public static readonly int[] BOOK_NO_ENTITY_IDS = new int[]
        {
            0, 43, 44, 45, 46, 47, 48, 49, 50, 51,
            561, 562, 563, 566, 569, 617, 625,
            631, 632, 633, 634, 635, 636, 637, 638, 639
        };
        public const int BOOK_NO_ENTITY_IDS_LO = 43;   // 占位道具起始（43-51）
        public const int BOOK_NO_ENTITY_IDS_HI = 51;   // 占位道具结束
        public const int BOOK_STORY_KEEPSAKE_ID = 531;   // 罗蕾莱的宝珠（贵重品：纹章），发道具需跳过

        // ===== 收集图鉴开启道具（2026-08-28 用户实测）=====
        // 通关档持有 qty=1 / EXT=0x03，早期档 qty=0 —— 拥有该贵重品才开启主菜单的"道具图鉴"页面
        // （用户反馈点全开后图鉴菜单仍不显示，遂要求全开时把此道具设为 1）。
        // 同类还有 564 世界地图(世界地图菜单)、567 角色盘(角色图鉴)；按用户要求仅发放 565。
        public const int BOOK_COLLECT_BOOK_ID = 565;     // 道具收集图鉴（贵重品·菜单开启钥匙）

        public static readonly int[] CHAR_BASE_OFFSETS = new int[8]
        {
            0,
            0x07D0,
            0x0B20,
            0x0E70,
            0x11C0,
            0x1510,
            0x1860,
            0x1BB0
        };

        public const int CHAR_BLOCK_SIZE = 848;

        // 注意：0x7D0 处（原 CHAR_ID）并非角色ID，是真实难度字节（全局+39，见 BODY_DIFFICULTY），
        // 恰好落在角色块前的 4 字节间隙里，与角色字段（从 +0x04 开始）无冲突；
        // 角色块物理起点为 0x7D4（0x484+0x350*n），本表偏移经 +4 抵消后与游戏一致
        public const int CHAR_SLOT_INDEX = 0x16; // u8
        public const int CHAR_NAME = 0x04;
        public const int CHAR_LEVEL_FLAGS = 0x14;
        public const int CHAR_LEVEL = 0x14;
        public const int CHAR_TITLE_FLAGS = 0x18;
        public const int CHAR_EXP = 0x1C;
        public const int CHAR_HP = 0x20;
        public const int CHAR_TP = 0x24;
        public const int CHAR_MAXHP = 0x28;
        public const int CHAR_MAXTP = 0x2C;
        public const int CHAR_PATK = 0x30;
        public const int CHAR_PDEF = 0x34;
        public const int CHAR_FATK = 0x38;
        public const int CHAR_FDEF = 0x3C;
        public const int CHAR_AGI = 0x40;
        public const int CHAR_LUCK = 0x44;
        public const int CHAR_CCORE_PATK = 0x50;
        public const int CHAR_CCORE_PDEF = 0x54;
        public const int CHAR_CCORE_FATK = 0x58;
        public const int CHAR_CCORE_FDEF = 0x5C;
        public const int CHAR_CCORE_AGI = 0x60;
        public const int CHAR_CCORE_LUK = 0x64; // Actually Equipment LUK bonus sum, not C-Core
        public const int CHAR_MAXHP_COPY = 0x68; // Actually HP total (MaxHP)
        public const int CHAR_MAXTP_COPY = 0x6C; // Actually MP total (MaxMP)
        public const int CHAR_BASE_PATK = 0x70; // Actually PATK total
        public const int CHAR_BASE_FATK = 0x74; // Actually PDEF total (order differs from base stats)
        public const int CHAR_BASE_PDEF = 0x78; // Actually FATK total (order differs from base stats)
        public const int CHAR_BASE_FDEF = 0x7C; // Actually FDEF total
        public const int CHAR_BASE_AGI = 0x80; // Actually AGI total
        public const int CHAR_LUCK_COPY = 0x84; // Actually Total LUCK value (base + equip), max 120
        public const int CHAR_LUCK_TOTAL = 0x84;
        public const int CHAR_EQUIP_LUK = 0x64;
        public const int CHAR_OVL_GAUGE = 0x324;
        public const int CHAR_OVL_GAUGE_HEADER_BASE = 0x94;
        public const int CHAR_OVL_HEADER_ENTRY_SIZE = 48;
        public const int CHAR_OVL_HEADER_GAUGE_OFFSET = 40;
        public const int CHAR_CCORE_PATK_BONUS = 0x94;
        public const int CHAR_CCORE_PDEF_BONUS = 0x96;
        public const int CHAR_CCORE_FATK_BONUS = 0x98;
        public const int CHAR_CCORE_FDEF_BONUS = 0x9A;
        public const int CHAR_CCORE_AGI_BONUS = 0x9E;
        public const int CHAR_EQUIP_ARRAY = 0x08C;
        public const int CHAR_EQUIP_SLOT_COUNT = 4;
        public const int CHAR_EQUIP_SLOT_SIZE = 2;
        public const int CHAR_KYOURITSUFU = 0x094;
        public const int CHAR_GROWTH_POINTS = 0xA0;
        public const int CHAR_FS_CHAMBER_STONES = 0x140;
        public const int CHAR_FS_CHAMBER_EQUIPPED_TYPE = 0x140;
        public const int CHAR_FS_CHAMBER_COLOR_OFFSET = 4;
        public const int CHAR_FS_CHAMBER_RECORD_SIZE = 12;
        public const int CHAR_FS_CHAMBER_STONE_COUNT = 4;
        public const int FS_CHAMBER_PER_CHAR = 80;
        public const int FS_CHAMBER_MAX_OFFSET = 42;
        public const int FS_CHAMBER_MAX_COUNT = 4;
        public const int CHAR_ARTE_ARRAY = 0xA4;
        public const int CHAR_ARTE_COUNT = 4;
        public const int CHAR_ARTE_LEARNED_BITMAP = 0xB0;
        public const int CHAR_ARTE_LEARNED_SIZE = 4;
        public const int CHAR_ARTE_LEARNED_BITMAP_COPY = 0xB4;
        public const int CHAR_ARTE_USAGE = 0x0BC;
        public const int CHAR_ARTE_USAGE_COUNT = 25;
        public const int CHAR_AD_SKILL = 0x110;
        public const int CHAR_AD_SKILL_SIZE = 11;
        public const int CHAR_AD_SKILL_COPY = 0x120;
        public const int CHAR_TITLE_INDEX = 0x17;

        public const int TOASYS_CHECKSUM = 0x00;
        public const int TOASYS_DATA_START = 0x08;
        public const int TOASYS_DATA_SIZE = 1852;

        public const int COOKING_DATA_OFFSET = 0x2254;
        public const int CHAR_COOKING_PROFICIENCY = 0x329;

        // Grade 区（战斗结算 sub_2AA6D0 写入，运行时全局 +0x4CF0C）：
        // 0xB080 = 当前持有 Grade（+= 战斗获得，上限 10,000,000）
        // 0xB088 = 累计获得 Grade（+= 战斗获得，上限 10,000,000），两者语义不同
        public const int BODY_GRADE = 0xB080;
        public const int BODY_GRADE_TOTAL = 0xB088;
        // 赌场 Grade 余额（定点数 ×100，含 2 位小数）：赌场菜单显示的 Grade = floor(此值/100)。
        // 实测（2026-08-19 E/F 差分验证）：此值才是唯一源头；var#773(0x43CD) 只是整数缓存，
        // 进赌场时被此值重算覆盖。守恒式 0xABA4/100 + 筹码×10 = 战斗Grade - 商店已花费。
        public const int BODY_GRADE_CASINO = 0xABA4;

        // 每角色杀敌数（影响魔武器攻击力；IDA sub_199418 敌人被击杀时 +1，clamp 999999=0xF423F）。
        // 击杀时同时更新两处：off_4F0500+8540 区（→TOA_XXX）与 TOASYS_RuntimeBlock（→TOASYS）。
        // 角色 ID 1-7：1=卢克 2=缇娅 3=杰德 4=阿妮丝 5=凯 6=娜塔莉亚 7=阿修。
        public const int BODY_CHAR_KILLS = 0x230C;     // TOA_XXX：0x230C + 4*(角色ID-1)，当前周目杀敌数
        public const int TOASYS_CHAR_KILLS = 0x8C;     // TOASYS：0x8C + 4*(角色ID-1)，跨周目累计杀敌数
        public const int CHAR_KILL_COUNT = 7;

        // 脚本变量区（file 0x2BA1 起 32KB=4095 个 8 字节条目 [tag u32][value u32]，sub_34ACB4 分配 dword_53A3B8）。
        // 赌场实测（2026-08-19，用户以 10Grade↔1筹码 兑换制造差分，五档验证）：
        //   var#271 = 赌场筹码（游戏内赌场菜单显示的持有数）
        //   var#773 = 赌场 Grade 余额（游戏内赌场显示的 Grade；兑换只扣此变量，0xB080/0xB088 不变）
        //   var#774 = 兑换相关计数器（语义未完全确认，只读展示）
        public const int SCRIPT_VARS = 0x2BA5;  // 修正：实际起始 0x2BA5（LoadToaSaveBuffer body+10637），非 0x2BA1
        public const int SCRIPT_VAR_CHIPS = 271;
        public const int SCRIPT_VAR_GRADE = 773;
        public const int SCRIPT_VAR_EXCHANGE = 774;

        // 剧情跳跃字段（IDA + SB7 脚本VM逆向 + 实机4档验证 2026-08-25）
        public const int BODY_MAP_ID = 0x528;              // u32 当前地图ID（0-649, maptable.mbt 索引）
        public const int BODY_MAP_TRANSITION_COUNT = 0x52C; // u32 地图切换计数（维护性，可不改）
        public const int BODY_PLAYER_X = 0x530;            // f32 玩家X坐标
        public const int BODY_PLAYER_Y = 0x534;            // f32 玩家Y坐标
        public const int BODY_PLAYER_Z = 0x538;            // f32 玩家Z坐标
        public const int BODY_PLAYER_ANGLE = 0x53C;        // f32 朝向角（度）
        // 脚本变量 var[136] = 当前事件ID（event_id），格式 XCCYYY0（X=篇1-4,CC=章节,YYY=事件序,末位0）
        // 游戏读档后 sub_2EB174(event_id) 调度执行对应剧情脚本
        public const int SCRIPT_VAR_EVENT_ID = 136;        // var 索引
        public const int BODY_EVENT_ID = 0x2FE5;           // u32 event_id（= SCRIPT_VARS + 136*8 的 tag 字段）
        // 全局 flag 位图（off_4F0500+116，file 0x218 起）
        public const int FLAG_BITMAP = 0x218;              // 位图基址，flag N = [N/8] 的 bit(N%8)
        public const int FLAG_BITMAP_SIZE = 768;           // 0x218~0x518，共 flag 0~6143

        // ===== 功能解锁 flag（2026-09-03 IDA + 四档差分定案）=====
        // 游戏侧证据（3DS ExeFS.elf，PS2 同语义）：
        //   1) 道具使用门 sub_3C4F7C（按 itemtable+21 类别分发）：类别 0x22 道具使用需 flag 2007、
        //      类别 0x23 需 flag 2009，未置位时按"无法使用"处理（响声提示）。
        //      类别 0x22 = C·コア(響律符 92-121)，类别 0x23 = FSチャンバー(嵌石 122-125)。
        //   2) 顶级菜单处理 sub_399724：菜单 id 3（カスタマイズ）的子页面列表为
        //      {0, (flag2007→1), 2, (flag2009→3)}，即 C·コア 页需 2007、FSチャンバー 页需 2009。
        //   3) 支线跳跃库：战斗新手教程 FOF(1005110)→clear 2008、新手教程 FS嵌石(1009060)→clear 2009，
        //      教程 flag 连续编号；"新手教程 CC符"(1003130) 对应 2007。
        //      （flag 2013 = NG+ 开局标志，sub_19C414 case31 写入，交叉印证 2000+ 段为系统解锁 flag。）
        //   4) TotA15 四档实证：TOA_000/001/003（后期档）2007/2009 恒置位；
        //      TOA_002（新档，仅地图 flag+2013）均未置位。
        //   PS2 存档 0x218 位图位于同位区间 [0,0x528)，经 MapOffset 自动适配。
        public const int GLOBAL_FLAG_C_CORE = 2007;        // 新手教程「CC符」完成 → 解锁 C·コア(响律符)
        public const int GLOBAL_FLAG_FS_CHAMBER = 2009;    // 新手教程「FS嵌石」完成 → 解锁 FSチャンバー(音素质点嵌石)

        // TOASYS 布局（sub_37D584 保存 / sub_3A9840 加载；数据区 = 运行时结构 unk_53C924 镜像）
        // 2026-08-19 双存档 diff + 用户记录交叉验证 + IDA（sub_333800 菜单构建/sub_199xxx 统计API族）定案：
        public const int TOASYS_VERSION = 0x04;            // float 0.2 版本号（只读）
        public const int TOASYS_GALD_MAX = 0x08;           // u32 最大持有GALD（clamp 99,999,999 sub_285284）TotA15=10,265,937
        public const int TOASYS_PLAYTIME_MAX = 0x0C;       // u32 最长游戏时间/帧（sub_3A2A90）TotA15=30,055,797≈139h08m49s
        public const int TOASYS_GALD_SPENT = 0x10;         // u32 累计使用GALD TotA15=11,728,269
        public const int TOASYS_SAVE_COUNT = 0x14;         // u32 存档次数 TotA15=1367
        public const int TOASYS_CLEAR_COUNT = 0x18;        // u32 通关次数（≠0 即解锁音效测试等通关后菜单 sub_333800）TotA15=4
        public const int TOASYS_ENCOUNTER = 0x1C;          // u32 遭遇数（使用率分母）TotA15=2805
        public const int TOASYS_ESCAPE = 0x28;             // u32 逃跑次数 TotA15=25
        public const int TOASYS_MAX_DAMAGE = 0x2C;         // u32 最大伤害（clamp 999999 sub_199224）TotA15=61288
        public const int TOASYS_MAX_COMBO = 0x30;          // u32 最大连击 TotA15=103
        public const int TOASYS_DAMAGE_DEALT = 0x38;       // u32 单次游玩造成总伤害 TotA15=167,078,185
        public const int TOASYS_DAMAGE_TAKEN = 0x3C;       // u32 单次游玩承受总伤害 TotA15=9,146,042
        public const int TOASYS_BATTLE_TIME = 0x5D0;       // u32 战斗总时间/帧（2805场×37.2s×60fps 吻合）
        public const int TOASYS_CHAR_USAGE = 0x6C;         // u32×6 角色使用计数（ID1-6，÷遭遇数=使用率）2209/2305/1688/1297/2056/797=78.8~28.4%
        public const int TOASYS_CHAR_USAGE_COUNT = 6;
        public const int TOASYS_UNLOCK_BITMAP = 0x6C4;     // 128B 累计解锁位图（byte_53CFE0，与 TOA_XXX 全局+628 OR 合并；收集/曲目累计）
        public const int TOASYS_UNLOCK_BITMAP_SIZE = 0x80;
        // ⚠ 0x1C-0x20 之间(0x34 等)与 0x40-0x6B 区含义未明；0x40-0x57 为开机队伍恢复数组（sub_159478）
        //    严禁 UI 写入；0x638/0x684 无代码引用不写入

        // ============ PS2 日版存档（平台核心支持）============
        // PS2 日版与 3DS 版存档字段布局语义一致，仅字节位置存在少量平移：
        // 主档 49096B（比 3DS 少 24B）、系统档 1832B（比 3DS 少 28B）。
        // 本类全部语义偏移常量保持 3DS 定义不变，PS2 偏移差异由 SaveData.MapOffset()
        // 在私有读写原语内统一翻译，上层（MainForm 等）无感知。
        public const int PS2_TOA_XXX_SIZE = 49096;        // 0xBFC8 PS2日版主存档
        public const int PS2_TOASYS_SIZE = 1832;          // 0x728 PS2日版系统存档
        public const int PS2_BODY_DATA_SIZE = 48564;      // [0x214, 0xBFC8)
        public const int PS2_TOASYS_DATA_SIZE = 1824;     // [0x08, 0x728)
        // PS2 主档偏移翻译区间边界（3DS 语义偏移基准）：
        // [0, PS2_MAP_IDENTITY_END) 同位；[PS2_MAP_IDENTITY_END, PS2_MAP_PLUS4_END) PS2=3DS+4；
        // [PS2_MAP_PLUS4_END, DS_ONLY_REGION_END) 为 3DS 专属常量区，PS2 无映射；之后到文件尾 PS2=3DS-24
        public const int PS2_MAP_IDENTITY_END = 1320;     // 0x528
        public const int PS2_MAP_PLUS4_END = 43980;       // 0xABCC
        public const int DS_ONLY_REGION_END = 44016;      // 0xABF0
        // PS2 系统档：[0, SYS_MAP_IDENTITY_END) 同位；之后 PS2=3DS-28
        public const int SYS_MAP_IDENTITY_END = 1552;
    }
}
