using System;
using System.IO;
using System.Text;

namespace TOAHEX
{
    public enum SaveType
    {
        Unknown,
        ToaXxx,
        Toasys
    }

    public class SaveData
    {
        private byte[] _buffer;
        private string _filePath;
        private SaveType _saveType;

        public byte[] Buffer => _buffer;
        public string FilePath => _filePath;
        public SaveType Type => _saveType;
        public bool IsLoaded => _buffer != null;

        public float Version
        {
            get => ReadFloat(SaveOffsets.HEADER_VERSION);
        }

        public float Difficulty
        {
            get => ReadFloat(SaveOffsets.HEADER_DIFFICULTY);
        }

        public uint Gald
        {
            get => ReadU32(SaveOffsets.BODY_GALD);
            set
            {
                WriteU32(SaveOffsets.BODY_GALD, value);
                WriteU32(SaveOffsets.HEAD_GALD_COPY, value);
            }
        }

        public uint PlayTime
        {
            get => ReadU32(SaveOffsets.BODY_PLAYTIME);
            set
            {
                WriteU32(SaveOffsets.BODY_PLAYTIME, value);
                WriteU32(SaveOffsets.HEAD_PLAYTIME_COPY, value);
            }
        }

        public float Grade
        {
            get => ReadFloat(SaveOffsets.BODY_GRADE);
            set
            {
                WriteFloat(SaveOffsets.BODY_GRADE, value);
                WriteFloat(SaveOffsets.BODY_GRADE_COPY, value);
            }
        }

        public uint PartyCount
        {
            get => ReadU32(SaveOffsets.HEAD_PARTY_COUNT);
        }

        public uint ReadArteLearnedBitmap(int charIndex)
        {
            int baseOff = GetCharBaseOffset(charIndex);
            if (baseOff == 0) return 0;
            return ReadU32(baseOff + SaveOffsets.CHAR_ARTE_LEARNED_BITMAP);
        }

        public void WriteArteLearnedBitmap(int charIndex, uint bitmap)
        {
            int baseOff = GetCharBaseOffset(charIndex);
            if (baseOff == 0) return;
            WriteU32(baseOff + SaveOffsets.CHAR_ARTE_LEARNED_BITMAP, bitmap);
            WriteU32(baseOff + SaveOffsets.CHAR_ARTE_LEARNED_BITMAP_COPY, bitmap);
        }

        public string LocationName
        {
            get => ReadShiftJisString(SaveOffsets.HEAD_LOCATION_NAME, 32);
        }

        public byte[] ReadPartyOrder()
        {
            byte[] order = new byte[SaveOffsets.BODY_PARTY_ORDER_COUNT];
            for (int i = 0; i < SaveOffsets.BODY_PARTY_ORDER_COUNT; i++)
                order[i] = ReadU8(SaveOffsets.BODY_PARTY_ORDER + i);
            return order;
        }

        public void WritePartyOrder(byte[] order)
        {
            if (order == null || order.Length != SaveOffsets.BODY_PARTY_ORDER_COUNT) return;
            for (int i = 0; i < SaveOffsets.BODY_PARTY_ORDER_COUNT; i++)
                WriteU8(SaveOffsets.BODY_PARTY_ORDER + i, order[i]);
        }

        public SaveData()
        {
            _buffer = null;
            _filePath = null;
            _saveType = SaveType.Unknown;
        }

        public bool Load(string filePath)
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            int size = fileData.Length;

            if (size == SaveOffsets.TOA_XXX_SIZE)
            {
                _saveType = SaveType.ToaXxx;
            }
            else if (size == SaveOffsets.TOASYS_SIZE)
            {
                _saveType = SaveType.Toasys;
            }
            else
            {
                return false;
            }

            _buffer = fileData;
            _filePath = filePath;
            return true;
        }

        public void Save(string filePath = null)
        {
            if (_buffer == null) return;

            string target = filePath ?? _filePath;

            string backupPath = target + ".bak";
            try
            {
                if (System.IO.File.Exists(target))
                {
                    System.IO.File.Copy(target, backupPath, true);
                }
            }
            catch { }

            if (_saveType == SaveType.ToaXxx)
            {
                ChecksumHelper.FixToaChecksum(_buffer);
            }
            else if (_saveType == SaveType.Toasys)
            {
                ChecksumHelper.FixToasysChecksum(_buffer);
            }

            File.WriteAllBytes(target, _buffer);
            _filePath = target;
        }

        public bool VerifyChecksum()
        {
            if (_buffer == null) return false;

            if (_saveType == SaveType.ToaXxx)
                return ChecksumHelper.VerifyToaChecksum(_buffer);
            if (_saveType == SaveType.Toasys)
                return ChecksumHelper.VerifyToasysChecksum(_buffer);

            return false;
        }

        public uint ReadU32(int offset)
        {
            return BitConverter.ToUInt32(_buffer, offset);
        }

        public void WriteU32(int offset, uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            System.Buffer.BlockCopy(bytes, 0, _buffer, offset, 4);
        }

        public ushort ReadU16(int offset)
        {
            return BitConverter.ToUInt16(_buffer, offset);
        }

        public void WriteU16(int offset, ushort value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            System.Buffer.BlockCopy(bytes, 0, _buffer, offset, 2);
        }

        public byte ReadU8(int offset)
        {
            return _buffer[offset];
        }

        public void WriteU8(int offset, byte value)
        {
            _buffer[offset] = value;
        }

        public byte ReadByte(int offset) { return _buffer[offset]; }
        public void WriteByte(int offset, byte value) { _buffer[offset] = value; }

        public float ReadFloat(int offset)
        {
            return BitConverter.ToSingle(_buffer, offset);
        }

        public void WriteFloat(int offset, float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            System.Buffer.BlockCopy(bytes, 0, _buffer, offset, 4);
        }

        public byte[] ReadBytes(int offset, int count)
        {
            byte[] result = new byte[count];
            System.Buffer.BlockCopy(_buffer, offset, result, 0, count);
            return result;
        }

        public void WriteBytes(int offset, byte[] data)
        {
            System.Buffer.BlockCopy(data, 0, _buffer, offset, data.Length);
        }

        public string ReadShiftJisString(int offset, int maxLength)
        {
            Encoding shiftJis = Encoding.GetEncoding(932);
            int nullIndex = -1;
            for (int i = offset; i < offset + maxLength - 1; i++)
            {
                if (_buffer[i] == 0)
                {
                    nullIndex = i - offset;
                    break;
                }
            }
            int count = nullIndex >= 0 ? nullIndex : maxLength;
            if (count == 0) return string.Empty;
            return shiftJis.GetString(_buffer, offset, count);
        }

        public int GetCharBaseOffset(int charIndex)
        {
            if (charIndex < 1 || charIndex > 7) return 0;
            return SaveOffsets.CHAR_BASE_OFFSETS[charIndex];
        }

        public ushort ReadKyouritsufu(int charIndex)
        {
            int baseOff = GetCharBaseOffset(charIndex);
            if (baseOff == 0) return 0;
            return ReadU16(baseOff + SaveOffsets.CHAR_KYOURITSUFU);
        }

        public void WriteKyouritsufu(int charIndex, ushort id)
        {
            int baseOff = GetCharBaseOffset(charIndex);
            if (baseOff == 0) return;
            WriteU16(baseOff + SaveOffsets.CHAR_KYOURITSUFU, id);
        }

        public ushort ReadFSChamberProgress(int charIndex, int arteSlot, int colorIndex)
        {
            int baseOff = GetCharBaseOffset(charIndex);
            if (baseOff == 0) return 0;
            if (arteSlot < 0 || arteSlot >= SaveOffsets.CHAR_ARTE_USAGE_COUNT) return 0;
            if (colorIndex < 0 || colorIndex >= SaveOffsets.CHAR_FS_CHAMBER_STONE_COUNT) return 0;
            int recordOffset = baseOff + SaveOffsets.CHAR_FS_CHAMBER_STONES + arteSlot * SaveOffsets.CHAR_FS_CHAMBER_RECORD_SIZE;
            return ReadU16(recordOffset + SaveOffsets.CHAR_FS_CHAMBER_COLOR_OFFSET + colorIndex * 2);
        }

        public void WriteFSChamberProgress(int charIndex, int arteSlot, int colorIndex, ushort value)
        {
            int baseOff = GetCharBaseOffset(charIndex);
            if (baseOff == 0) return;
            if (arteSlot < 0 || arteSlot >= SaveOffsets.CHAR_ARTE_USAGE_COUNT) return;
            if (colorIndex < 0 || colorIndex >= SaveOffsets.CHAR_FS_CHAMBER_STONE_COUNT) return;
            if (value > 100) value = 100;
            int recordOffset = baseOff + SaveOffsets.CHAR_FS_CHAMBER_STONES + arteSlot * SaveOffsets.CHAR_FS_CHAMBER_RECORD_SIZE;
            WriteU16(recordOffset + SaveOffsets.CHAR_FS_CHAMBER_COLOR_OFFSET + colorIndex * 2, value);
        }

        public byte ReadFSChamberEquippedType(int charIndex, int arteSlot)
        {
            int baseOff = GetCharBaseOffset(charIndex);
            if (baseOff == 0) return 0;
            if (arteSlot < 0 || arteSlot >= SaveOffsets.CHAR_ARTE_USAGE_COUNT) return 0;
            int recordOffset = baseOff + SaveOffsets.CHAR_FS_CHAMBER_EQUIPPED_TYPE + arteSlot * SaveOffsets.CHAR_FS_CHAMBER_RECORD_SIZE;
            return ReadU8(recordOffset);
        }

        public void WriteFSChamberEquippedType(int charIndex, int arteSlot, byte stoneType)
        {
            int baseOff = GetCharBaseOffset(charIndex);
            if (baseOff == 0) return;
            if (arteSlot < 0 || arteSlot >= SaveOffsets.CHAR_ARTE_USAGE_COUNT) return;
            if (stoneType > 4) return;
            int recordOffset = baseOff + SaveOffsets.CHAR_FS_CHAMBER_EQUIPPED_TYPE + arteSlot * SaveOffsets.CHAR_FS_CHAMBER_RECORD_SIZE;
            WriteU8(recordOffset, stoneType);
        }

        public int GetFSChamberLevel(int charIndex, int arteSlot, int colorIndex)
        {
            ushort progress = ReadFSChamberProgress(charIndex, arteSlot, colorIndex);
            if (progress >= 100) return 6;
            if (progress >= 80) return 5;
            if (progress >= 60) return 4;
            if (progress >= 40) return 3;
            if (progress >= 20) return 2;
            return 1;
        }

        public void SetFSChamberLevel(int charIndex, int arteSlot, int colorIndex, int level)
        {
            if (level < 1 || level > 6) return;
            ushort progress = (ushort)(20 * (level - 1));
            WriteFSChamberProgress(charIndex, arteSlot, colorIndex, progress);
        }

        public byte ReadFSChamberMax(int charIndex, int colorIndex)
        {
            if (charIndex < 1 || charIndex > 7) return 0;
            if (colorIndex < 0 || colorIndex >= SaveOffsets.FS_CHAMBER_MAX_COUNT) return 0;
            int off = SaveOffsets.BODY_ITEM_ARRAY + 1 * SaveOffsets.FS_CHAMBER_PER_CHAR + SaveOffsets.FS_CHAMBER_MAX_OFFSET + colorIndex;
            return ReadU8(off);
        }

        public void WriteFSChamberMax(int charIndex, int colorIndex, byte value)
        {
            if (charIndex < 1 || charIndex > 7) return;
            if (colorIndex < 0 || colorIndex >= SaveOffsets.FS_CHAMBER_MAX_COUNT) return;
            int off = SaveOffsets.BODY_ITEM_ARRAY + 1 * SaveOffsets.FS_CHAMBER_PER_CHAR + SaveOffsets.FS_CHAMBER_MAX_OFFSET + colorIndex;
            WriteU8(off, value);
        }

        public byte ReadCookingMastery(int charIndex, int recipeIndex)
        {
            if (charIndex < 1 || charIndex >= SaveOffsets.CHAR_BASE_OFFSETS.Length) return 0;
            if (recipeIndex < 0 || recipeIndex >= 20) return 0;
            return ReadU8(SaveOffsets.CHAR_BASE_OFFSETS[charIndex] + SaveOffsets.CHAR_COOKING_PROFICIENCY + recipeIndex);
        }

        public void WriteCookingMastery(int charIndex, int recipeIndex, byte value)
        {
            if (charIndex < 1 || charIndex >= SaveOffsets.CHAR_BASE_OFFSETS.Length) return;
            if (recipeIndex < 0 || recipeIndex >= 20) return;
            WriteU8(SaveOffsets.CHAR_BASE_OFFSETS[charIndex] + SaveOffsets.CHAR_COOKING_PROFICIENCY + recipeIndex, value);
        }

        public int GetCookingMasteryStar(byte masteryValue)
        {
            int star = masteryValue / 20;
            return star > 3 ? 3 : star;
        }

        public void SetCookingMasteryStar(int charIndex, int recipeIndex, int starLevel)
        {
            if (starLevel < 0 || starLevel > 3) return;
            byte value;
            if (starLevel == 3) value = 60;
            else if (starLevel == 2) value = 40;
            else if (starLevel == 1) value = 20;
            else value = 0;
            WriteCookingMastery(charIndex, recipeIndex, value);
        }

        public uint ReadCookingFlags()
        {
            return ReadU32(SaveOffsets.COOKING_DATA_OFFSET);
        }

        public void WriteCookingFlags(uint flags)
        {
            WriteU32(SaveOffsets.COOKING_DATA_OFFSET, flags);
        }

        public byte[] GetItemQuantities()
        {
            byte[] quantities = new byte[SaveOffsets.BODY_ITEM_COUNT];
            System.Buffer.BlockCopy(_buffer, SaveOffsets.BODY_ITEM_ARRAY, quantities, 0, SaveOffsets.BODY_ITEM_COUNT);
            return quantities;
        }

        public void SetItemQuantity(int itemId, byte quantity)
        {
            if (itemId < 0 || itemId >= SaveOffsets.BODY_ITEM_COUNT) return;
            if (quantity > 99) quantity = 99;
            _buffer[SaveOffsets.BODY_ITEM_ARRAY + itemId] = quantity;
        }

        public uint ReadLuckBase(int charIndex)
        {
            int baseOff = GetCharBaseOffset(charIndex);
            if (baseOff == 0) return 0;
            return ReadU32(baseOff + SaveOffsets.CHAR_LUCK);
        }

        public void WriteLuckBase(int charIndex, uint value)
        {
            if (value > 120) value = 120;
            int baseOff = GetCharBaseOffset(charIndex);
            if (baseOff == 0) return;
            WriteU32(baseOff + SaveOffsets.CHAR_LUCK, value);
            uint total = value + ReadLuckEquipBonus(charIndex);
            if (total > 120) total = 120;
            WriteU32(baseOff + SaveOffsets.CHAR_LUCK_TOTAL, total);
        }

        public uint ReadLuckEquipBonus(int charIndex)
        {
            int baseOff = GetCharBaseOffset(charIndex);
            if (baseOff == 0) return 0;
            return ReadU32(baseOff + SaveOffsets.CHAR_EQUIP_LUK);
        }

        public void WriteLuckEquipBonus(int charIndex, uint value)
        {
            int baseOff = GetCharBaseOffset(charIndex);
            if (baseOff == 0) return;
            WriteU32(baseOff + SaveOffsets.CHAR_EQUIP_LUK, value);
            uint total = value + ReadLuckBase(charIndex);
            if (total > 120) total = 120;
            WriteU32(baseOff + SaveOffsets.CHAR_LUCK_TOTAL, total);
        }

        public uint ReadLuckTotal(int charIndex)
        {
            int baseOff = GetCharBaseOffset(charIndex);
            if (baseOff == 0) return 0;
            return ReadU32(baseOff + SaveOffsets.CHAR_LUCK_TOTAL);
        }

        public ushort ReadOvlGauge(int charIndex)
        {
            int baseOff = GetCharBaseOffset(charIndex);
            if (baseOff == 0) return 0;
            return ReadU16(baseOff + SaveOffsets.CHAR_OVL_GAUGE);
        }

        public void WriteOvlGauge(int charIndex, ushort value)
        {
            if (value > 1000) value = 1000;
            int baseOff = GetCharBaseOffset(charIndex);
            if (baseOff == 0) return;
            WriteU16(baseOff + SaveOffsets.CHAR_OVL_GAUGE, value);
            int headerEntry = FindOvlHeaderEntry(charIndex);
            if (headerEntry >= 0)
                WriteU32(headerEntry + SaveOffsets.CHAR_OVL_HEADER_GAUGE_OFFSET, value);
        }

        public ushort ReadOvlGaugeFromHeader(int charIndex)
        {
            if (charIndex < 1 || charIndex > 6) return 0;
            int headerEntry = FindOvlHeaderEntry(charIndex);
            if (headerEntry < 0) return 0;
            return (ushort)ReadU32(headerEntry + SaveOffsets.CHAR_OVL_HEADER_GAUGE_OFFSET);
        }

        private int FindOvlHeaderEntry(int charIndex)
        {
            for (int i = 0; i < 6; i++)
            {
                int entryBase = SaveOffsets.CHAR_OVL_GAUGE_HEADER_BASE + i * SaveOffsets.CHAR_OVL_HEADER_ENTRY_SIZE;
                uint slotIdx = ReadU32(entryBase);
                if (slotIdx == charIndex)
                    return entryBase;
            }
            return -1;
        }

        public ushort ReadCCoreBonus(int charIndex, int statIndex)
        {
            int baseOff = GetCharBaseOffset(charIndex);
            if (baseOff == 0) return 0;
            int[] offsets = { SaveOffsets.CHAR_CCORE_PATK_BONUS, SaveOffsets.CHAR_CCORE_PDEF_BONUS, SaveOffsets.CHAR_CCORE_FATK_BONUS, SaveOffsets.CHAR_CCORE_FDEF_BONUS, SaveOffsets.CHAR_CCORE_AGI_BONUS };
            if (statIndex < 0 || statIndex >= offsets.Length) return 0;
            return ReadU16(baseOff + offsets[statIndex]);
        }

        public void WriteCCoreBonus(int charIndex, int statIndex, ushort value)
        {
            int baseOff = GetCharBaseOffset(charIndex);
            if (baseOff == 0) return;
            int[] offsets = { SaveOffsets.CHAR_CCORE_PATK_BONUS, SaveOffsets.CHAR_CCORE_PDEF_BONUS, SaveOffsets.CHAR_CCORE_FATK_BONUS, SaveOffsets.CHAR_CCORE_FDEF_BONUS, SaveOffsets.CHAR_CCORE_AGI_BONUS };
            if (statIndex < 0 || statIndex >= offsets.Length) return;
            WriteU16(baseOff + offsets[statIndex], value);
        }
    }
}
