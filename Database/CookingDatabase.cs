using System;
using System.Collections.Generic;
using System.Linq;

namespace TOAHEX
{
    public static class CookingDatabase
    {
        private static Dictionary<int, string> _datNames;

        public static void LoadFromDat(string filePath)
        {
            _datNames = DatDataLoader.LoadCkdData(filePath);

            if (_data.IsValueCreated && _datNames != null)
            {
                foreach (var kv in _datNames)
                {
                    if (_data.Value.TryGetValue(kv.Key, out var existing) && !string.IsNullOrEmpty(kv.Value))
                    {
                        _data.Value[kv.Key] = (kv.Value, existing.cn);
                    }
                }
            }
        }

        private static readonly Lazy<Dictionary<int, (string jp, string cn)>> _data =
            new Lazy<Dictionary<int, (string jp, string cn)>>(() =>
            {
                var d = new Dictionary<int, (string jp, string cn)>();
                d[0] = ("おにぎり", "饭团");
                d[1] = ("パスタ", "茄汁面");
                d[2] = ("サンドイッチ", "三明治");
                d[3] = ("からあげ", "炸鸡块");
                d[4] = ("炒飯", "炒饭");
                d[5] = ("ラーメン", "拉面");
                d[6] = ("ピザ", "比萨");
                d[7] = ("サラダ", "沙拉");
                d[8] = ("カレー", "咖喱");
                d[9] = ("おそば", "荞麦面");
                d[10] = ("ケーキ", "蛋糕");
                d[11] = ("たまご丼", "鸡蛋盖饭");
                d[12] = ("おすし", "寿司");
                d[13] = ("うどん", "乌冬面");
                d[14] = ("トースト", "烤吐司");
                d[15] = ("オニオンスープ", "洋葱汤");
                d[16] = ("グラタン", "奶汁焗饭");
                d[17] = ("フルーツミックス", "水果捞");
                d[18] = ("シチュー", "炖菜");
                d[19] = ("おでん", "关东煮");

                if (_datNames != null)
                {
                    var keys = new List<int>(d.Keys);
                    foreach (var id in keys)
                    {
                        if (_datNames.TryGetValue(id, out var datName) && !string.IsNullOrEmpty(datName))
                        {
                            d[id] = (datName, d[id].cn);
                        }
                    }
                }

                return d;
            });

        public static (string jp, string cn) GetById(int id)
        {
            return _data.Value.TryGetValue(id, out var entry) ? entry : ("???", "???");
        }

        public static string GetName(int id)
        {
            return _data.Value.TryGetValue(id, out var entry)
                ? (LanguageConfig.Current == Language.JP ? entry.jp : entry.cn)
                : "???";
        }

        public static List<(int id, string jp, string cn)> GetAll()
        {
            return _data.Value.Select(kv => (kv.Key, kv.Value.jp, kv.Value.cn)).ToList();
        }

        public static List<(int id, string jp, string cn)> Search(string keyword)
        {
            if (string.IsNullOrEmpty(keyword)) return new List<(int, string, string)>();
            var kw = keyword.ToLowerInvariant();
            return _data.Value
                .Where(kv => kv.Value.jp.ToLowerInvariant().Contains(kw) || kv.Value.cn.ToLowerInvariant().Contains(kw))
                .Select(kv => (kv.Key, kv.Value.jp, kv.Value.cn))
                .ToList();
        }
    }
}
