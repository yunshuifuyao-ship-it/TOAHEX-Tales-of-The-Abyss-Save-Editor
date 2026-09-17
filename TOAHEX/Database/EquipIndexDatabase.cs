using System;
using System.Collections.Generic;
using System.Linq;

namespace TOAHEX
{
    public static class EquipIndexDatabase
    {
        // 武器分类（存档实测 TotA15：杰德装295音素长枪=枪系、阿妮丝装254吉尼亚斯之杖=杖系）
        private static readonly string[][] CharWeaponSubCategories = new string[8][]
        {
            null,
            new[] { "剣", "剑" },       // [1] 卢克
            new[] { "杖" },             // [2] 缇娅
            new[] { "槍", "枪" },       // [3] 杰德（原误配"杖"）
            new[] { "杖" },             // [4] 阿妮丝（原误配"槍"）
            new[] { "剣", "剑" },       // [5] 凯
            new[] { "弓" },             // [6] 娜塔莉亚
            new[] { "剣", "剑" },       // [7] 阿修
        };

        // 防具分类（存档实测 TotA15：缇娅377虹光护具=护甲系、杰德408音素长袍=长袍系、阿妮丝393王后外套=斗篷系）
        private static readonly string[][] CharArmorSubCategories = new string[8][]
        {
            null,
            new[] { "鎧", "铠" },           // [1] 卢克
            new[] { "ガード", "护甲" },     // [2] 缇娅（原误配"ローブ"）
            new[] { "ローブ", "长袍" },     // [3] 杰德（原误配"クローク"）
            new[] { "クローク", "斗篷" },   // [4] 阿妮丝（原误配"ガード"）
            new[] { "鎧", "铠" },           // [5] 凯
            new[] { "ガード", "护甲" },     // [6] 娜塔莉亚
            new[] { "ローブ", "长袍" },     // [7] 阿修
        };

        private static readonly int[] AccessoryTypeCodes = new int[] { 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C };

        public static List<ItemData> GetEquipItemsForSlot(int charIndex, int slotIndex)
        {
            if (charIndex < 1 || charIndex > 7) return new List<ItemData>();

            if (slotIndex == 0)
            {
                return GetWeaponsForChar(charIndex);
            }
            else if (slotIndex == 1)
            {
                return GetArmorForChar(charIndex);
            }
            else
            {
                return GetAccessories();
            }
        }

        public static List<ItemData> GetWeaponsForChar(int charIndex)
        {
            if (charIndex < 1 || charIndex > 7) return new List<ItemData>();
            var subCats = CharWeaponSubCategories[charIndex];
            return ItemDatabase.GetByCategoryAndSubCategory("武器", subCats);
        }

        public static List<ItemData> GetArmorForChar(int charIndex)
        {
            if (charIndex < 1 || charIndex > 7) return new List<ItemData>();
            var subCats = CharArmorSubCategories[charIndex];
            return ItemDatabase.GetByCategoryAndSubCategory("防具", subCats);
        }

        public static List<ItemData> GetAccessories()
        {
            return ItemDatabase.GetByTypeCodes(AccessoryTypeCodes);
        }

        public static string GetItemNameByItemId(int itemId)
        {
            var item = ItemDatabase.GetById(itemId);
            return item != null ? item.Name : "";
        }
    }
}
