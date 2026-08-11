using System;

namespace TOAHEX
{
    public static class SaveOffsets
    {
        public const int TOA_XXX_SIZE = 49120;
        public const int TOASYS_SIZE = 1860;

        public const int HEADER_VERSION = 0x0000;
        public const int HEADER_DIFFICULTY = 0x0004;
        public const int HEADER_IDENT = 0x0008;
        public const int HEADER_DATA_OFFSET = 0x000C;
        public const int HEADER_CHECKSUM = 0x0010;
        public const int BODY_CHECKSUM = 0x0014;

        public const int HEAD_DATA_START = 0x0020;
        public const int HEAD_DATA_SIZE = 500;
        public const int BODY_DATA_START = 0x0214;
        public const int BODY_DATA_SIZE = 48588;

        public const int HEAD_VERSION = 0x0020;
        public const int HEAD_DIFFICULTY = 0x0024;
        public const int HEAD_ENCOUNTER = 0x0038;
        public const int HEAD_HIT = 0x003C;
        public const int HEAD_PARTY_COUNT = 0x0028;
        public const int HEAD_GALD_COPY = 0x002C;
        public const int HEAD_PLAYTIME_COPY = 0x0030;
        public const int HEAD_PARTY_ORDER = 0x0044;
        public const int HEAD_LOCATION_NAME = 0x004C;

        public const int BODY_GALD = 0x051C;
        public const int BODY_PLAYTIME = 0x0520;
        public const int BODY_DIFFICULTY = 0x051A;
        public const int BODY_FEATURE_FLAGS = 0x052C;
        public const int BODY_ENCOUNTER = 0x229C;
        public const int BODY_HIT = 0x22B0;
        public const int BODY_PARTY_ORDER = 0x07C4;
        public const int BODY_PARTY_ORDER_COUNT = 8;
        public const int BODY_LEADER = 0x07C2;
        public const int BODY_SUB_LEADER = 0x07C3;
        public const int BODY_ITEM_ARRAY = 0x0542;
        public const int BODY_ITEM_COUNT = 640;

        public const int JOURNAL_FLAGS_OFFSET = 0x0224;
        public const int JOURNAL_FLAGS_SIZE = 0x02DC;
        public const int BOOK_FLAGS_OFFSET = 0xBAD0;
        public const int BOOK_FLAGS_SIZE = 0x0070;

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

        public const int CHAR_ID = 0x00;
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

        public const int BODY_GRADE = 0xB080;
        public const int BODY_GRADE_COPY = 0xB088;
        public const int BOOK_DETAIL_FLAGS = 0xBB40;
        public const int BOOK_DETAIL_DATA = 0xBBD0;
        public const int BOOK_DETAIL_DATA_SIZE = 0x03B8;

        public const int TOASYS_DIFFICULTY = 0x04;
        public const int TOASYS_GALD_COPY = 0x0C;
        public const int TOASYS_PLAYTIME_COPY = 0x10;
        public const int TOASYS_TOTAL_TIME = 0x14;
        public const int TOASYS_UNKNOWN2 = 0x18;
        public const int TOASYS_SAVE_COUNT = 0x1C;
        public const int TOASYS_SYS_FLAG1 = 0x28;
        public const int TOASYS_SYS_FLAG2 = 0x2C;
        public const int TOASYS_SYS_FLAG3 = 0x30;
        public const int TOASYS_ENCOUNTER = 0x34;
        public const int TOASYS_CHAR_USAGE = 0x40;
    }
}
