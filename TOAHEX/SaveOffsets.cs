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
        public const int HEAD_LOCATION_NAME = 0x004C;

        public const int BODY_GALD = 0x051C;
        public const int BODY_PLAYTIME = 0x0520;
        public const int BODY_PARTY_COUNT_COPY = 0x0518; // u16 队伍人数镜像（读档恢复到 word[485]）
        public const int BODY_FEATURE_FLAGS = 0x052C;
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

        public const int JOURNAL_FLAGS_OFFSET = 0x0224;
        public const int JOURNAL_FLAGS_SIZE = 0x02DC;

        // 图鉴四段布局（与游戏 sub_37C948 保存 / sub_3A7C24 加载逐段对应）
        public const int BOOK_MAIN_FLAGS_OFFSET = 0xB2D0; // 2048B 主 flags（运行时全局+315740）
        public const int BOOK_MAIN_FLAGS_SIZE = 0x0800;
        public const int BOOK_SUB_FLAGS_OFFSET = 0xBAD0;   // 256B（运行时全局+317788）
        public const int BOOK_SUB_FLAGS_SIZE = 0x0100;
        public const int BOOK_DETAIL_DATA = 0xBBD0;        // 320B（运行时全局+250076，640道具×4bit）
        public const int BOOK_DETAIL_DATA_SIZE = 0x0140;
        public const int BOOK_EXTRA_DATA_OFFSET = 0xBD10;  // 720B（运行时全局+250396）
        public const int BOOK_EXTRA_DATA_SIZE = 0x02D0;

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
    }
}
