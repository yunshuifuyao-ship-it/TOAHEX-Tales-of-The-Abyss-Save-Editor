using System;
using System.Collections.Generic;
using System.Linq;

namespace TOAHEX
{
    public static class KyouritsufuDatabase
    {
        public class KyouritsufuData
        {
            public int Id { get; }
            public string JpName { get; }
            public string CnName { get; }
            public string Name => LanguageConfig.Current == Language.JP ? JpName : CnName;

            public KyouritsufuData(int id, string jpName, string cnName)
            {
                Id = id;
                JpName = jpName;
                CnName = cnName;
            }
        }

        private static readonly Lazy<Dictionary<int, KyouritsufuData>> _data =
            new Lazy<Dictionary<int, KyouritsufuData>>(() =>
            {
                var d = new Dictionary<int, KyouritsufuData>();
                d[0] = new KyouritsufuData(0, "なし", "未装备");
                d[112] = new KyouritsufuData(112, "ストレッシード", "激烈");
                d[113] = new KyouritsufuData(113, "ノーレシード", "庄重");
                d[114] = new KyouritsufuData(114, "バルラッシード", "充沛");
                d[115] = new KyouritsufuData(115, "レープハフド", "活泼");
                d[116] = new KyouritsufuData(116, "シルシード", "急板");
                d[117] = new KyouritsufuData(117, "パルラント", "宣叙");
                d[118] = new KyouritsufuData(118, "ラルガメンテ", "宽广");
                d[119] = new KyouritsufuData(119, "ルナティート", "疯狂");
                d[120] = new KyouritsufuData(120, "グランディオーツ", "磅礴");
                d[121] = new KyouritsufuData(121, "トゥッティ", "齐奏");
                return d;
            });

        public static KyouritsufuData GetById(int id)
        {
            return _data.Value.TryGetValue(id, out var entry) ? entry : null;
        }

        public static string GetName(int id)
        {
            return _data.Value.TryGetValue(id, out var entry) ? entry.Name : "";
        }

        public static List<KyouritsufuData> GetAll()
        {
            return _data.Value.Values.ToList();
        }
    }
}
