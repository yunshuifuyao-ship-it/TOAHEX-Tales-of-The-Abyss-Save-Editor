using System;
using System.Collections.Generic;
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

        // 0xB080 当前持有 Grade（游戏 sub_2AA6D0：战斗 +=，上限 1e7）
        public float Grade
        {
            get => ReadFloat(SaveOffsets.BODY_GRADE);
            set => WriteFloat(SaveOffsets.BODY_GRADE, value);
        }

        // 0xB088 累计获得 Grade（游戏 sub_2AA6D0：+= 战斗获得，上限 1e7；与持有是不同字段）
        public float TotalGrade
        {
            get => ReadFloat(SaveOffsets.BODY_GRADE_TOTAL);
            set => WriteFloat(SaveOffsets.BODY_GRADE_TOTAL, value);
        }

        // 赌场 Grade 余额整数缓存 = 脚本变量 #773（进赌场时被 0xABA4 重算覆盖，非源头）
        public uint CasinoGrade
        {
            get => ReadU32(SaveOffsets.SCRIPT_VARS + SaveOffsets.SCRIPT_VAR_GRADE * 8 + 4);
            set => WriteU32(SaveOffsets.SCRIPT_VARS + SaveOffsets.SCRIPT_VAR_GRADE * 8 + 4, value);
        }

        // 0xABA4 赌场 Grade 余额定点数（×100，含 2 位小数）。游戏显示 = floor(/100)，唯一源头。
        public uint CasinoGradePoint
        {
            get => ReadU32(SaveOffsets.BODY_GRADE_CASINO);
            set => WriteU32(SaveOffsets.BODY_GRADE_CASINO, value);
        }

        // 赌场 Grade 余额（整数，= floor(0xABA4/100)，游戏实际显示值）
        public uint CasinoGradeDisplay
        {
            get => ReadU32(SaveOffsets.BODY_GRADE_CASINO) / 100;
        }

        // 写赌场 Grade 余额：写 0xABA4（保留原小数 0.xx），并同步 var#773 整数缓存
        public void WriteCasinoGrade(uint grade)
        {
            uint frac = ReadU32(SaveOffsets.BODY_GRADE_CASINO) % 100;
            WriteU32(SaveOffsets.BODY_GRADE_CASINO, grade * 100 + frac);
            WriteU32(SaveOffsets.SCRIPT_VARS + SaveOffsets.SCRIPT_VAR_GRADE * 8 + 4, grade);
        }

        // 脚本变量类型 tag（0x200=int 有效；其他=脚本复用后的垃圾值，勿信）
        public uint ReadScriptVarTag(int index)
        {
            return ReadU32(SaveOffsets.SCRIPT_VARS + index * 8);
        }

        // 赌场筹码 = 脚本变量 #271（赌场菜单显示的持有筹码数）
        public uint CasinoChips
        {
            get => ReadU32(SaveOffsets.SCRIPT_VARS + SaveOffsets.SCRIPT_VAR_CHIPS * 8 + 4);
            set => WriteU32(SaveOffsets.SCRIPT_VARS + SaveOffsets.SCRIPT_VAR_CHIPS * 8 + 4, value);
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
            get
            {
                // 优先用汉化码表解码（Decode 内部处理 0x00 截断），码表未加载时回退 Shift-JIS
                if (TblCodec.IsLoaded)
                    return TblCodec.Decode(ReadBytes(SaveOffsets.HEAD_LOCATION_NAME, 32));
                return ReadShiftJisString(SaveOffsets.HEAD_LOCATION_NAME, 32);
            }
        }

        /// <summary>读取角色名（charIndex 1-7，CHAR_NAME=0x04，16 字节，0x00 截断），优先码表解码</summary>
        public string ReadCharName(int charIndex)
        {
            int baseOff = GetCharBaseOffset(charIndex);
            if (baseOff == 0) return string.Empty;
            if (TblCodec.IsLoaded)
                return TblCodec.Decode(ReadBytes(baseOff + SaveOffsets.CHAR_NAME, 16));
            return ReadShiftJisString(baseOff + SaveOffsets.CHAR_NAME, 16);
        }

        /// <summary>
        /// 写入角色名（16 字节字段，0x00 补齐；有效数据最多 15 字节）。
        /// 失败时 error 说明原因（中文），返回 false；成功 error=null 返回 true。
        /// </summary>
        public bool WriteCharName(int charIndex, string name, out string error)
        {
            error = null;
            if (_buffer == null || _saveType != SaveType.ToaXxx)
            {
                error = "仅支持 TOA_XXX 存档。";
                return false;
            }
            if (string.IsNullOrEmpty(name))
            {
                error = "名称不能为空。";
                return false;
            }
            int baseOff = GetCharBaseOffset(charIndex);
            if (baseOff == 0)
            {
                error = "无效的角色索引。";
                return false;
            }
            if (!TblCodec.IsLoaded)
            {
                error = "码表未加载（new_patched.tbl），无法编码角色名。";
                return false;
            }

            byte[] encoded = TblCodec.Encode(name, out List<string> invalidChars);
            if (invalidChars.Count > 0)
            {
                error = "以下字符无法用码表编码：" + string.Join("、", invalidChars);
                return false;
            }
            if (encoded.Length > 15)
            {
                error = string.Format("名称编码后为 {0} 字节，超过 15 字节上限。", encoded.Length);
                return false;
            }

            byte[] buf = new byte[16];
            System.Buffer.BlockCopy(encoded, 0, buf, 0, encoded.Length);
            WriteBytes(baseOff + SaveOffsets.CHAR_NAME, buf);
            return true;
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

            // 如果加载的是 .bak 文件，保存时生成去掉 .bak 后缀的文件
            if (target.EndsWith(".bak", StringComparison.OrdinalIgnoreCase))
            {
                target = target.Substring(0, target.Length - 4);
            }

            // .bak 备份文件永不更新：仅在不存在时创建一次
            string backupPath = target + ".bak";
            try
            {
                if (!System.IO.File.Exists(backupPath) && System.IO.File.Exists(target))
                {
                    System.IO.File.Copy(target, backupPath, false);
                }
            }
            catch { }

            if (_saveType == SaveType.ToaXxx)
            {
                // 保存前按游戏逻辑(sub_37C948)从 body 角色块重建 HEAD 摘要区(0x94)，
                // 保证存档槽预览与编辑后的数据同步
                RebuildHeadSummary();
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

        /// <summary>
        /// 依据游戏保存逻辑（sub_37C948 头摘要循环）从 body 角色块重建 HEAD 摘要区（0x94 起，每条目 48 字节）。
        /// 条目布局（相对 0x94+48n）：+0 角色ID、+4..+19 名字16B、+20 等级u8、+24 HP总值、
        /// +28 MP总值、+32 当前HP、+36 当前TP、+40 OVL(u16 来源 0x324)。
        /// </summary>
        public int RebuildHeadSummary()
        {
            if (_buffer == null || _saveType != SaveType.ToaXxx) return 0;

            // 以 body 队伍顺序(0x7C4, 1-based 角色 ID)为准重建
            int rebuilt = 0;
            for (int i = 0; i < 6 && i < SaveOffsets.BODY_PARTY_ORDER_COUNT; i++)
            {
                int roleId = ReadU8(SaveOffsets.BODY_PARTY_ORDER + i);
                if (roleId < 1 || roleId > 7) continue;
                int baseOff = GetCharBaseOffset(roleId);
                if (baseOff == 0) continue;

                int entry = SaveOffsets.CHAR_OVL_GAUGE_HEADER_BASE + rebuilt * SaveOffsets.CHAR_OVL_HEADER_ENTRY_SIZE;
                WriteU32(entry + 0, (uint)roleId);
                byte[] name = ReadBytes(baseOff + SaveOffsets.CHAR_NAME, 16);
                WriteBytes(entry + 4, name);
                WriteU8(entry + 20, (byte)(ReadU32(baseOff + SaveOffsets.CHAR_LEVEL) & 0xFF));
                WriteU32(entry + 24, ReadU32(baseOff + 0x68));  // HP 总值
                WriteU32(entry + 28, ReadU32(baseOff + 0x6C));  // MP 总值
                WriteU32(entry + 32, ReadU32(baseOff + SaveOffsets.CHAR_HP));
                WriteU32(entry + 36, ReadU32(baseOff + SaveOffsets.CHAR_TP));
                WriteU32(entry + 40, ReadOvlGauge(roleId));
                rebuilt++;
            }
            return rebuilt;
        }
    }
}
