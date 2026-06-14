using System;
using System.Collections.Generic;
using System.Linq;

namespace TOAHEX
{
    public static class EquipIndexDatabase
    {
        private static readonly string[][] CharWeaponSubCategories = new string[8][]
        {
            null,
            new[] { "剣" },
            new[] { "杖" },
            new[] { "杖" },
            new[] { "槍" },
            new[] { "剣" },
            new[] { "弓" },
            new[] { "剣" },
        };

        private static readonly string[][] CharArmorSubCategories = new string[8][]
        {
            null,
            new[] { "鎧" },
            new[] { "ローブ" },
            new[] { "クローク" },
            new[] { "ガード" },
            new[] { "鎧" },
            new[] { "ガード" },
            new[] { "ローブ" },
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
