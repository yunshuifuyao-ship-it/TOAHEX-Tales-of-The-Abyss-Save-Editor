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
                // C·コア全30种 = 道具ID 92-121（存档实测：汉化档卢克初期装92ストレ，
                // TotA15 七角色装 113/114/118/119/120/121，阿修未装备=0）
                d[92] = new KyouritsufuData(92, "ストレ", "傲慢");
                d[93] = new KyouritsufuData(93, "ノーレ", "严格");
                d[94] = new KyouritsufuData(94, "アルカ", "轻松");
                d[95] = new KyouritsufuData(95, "ノーレド", "调皮");
                d[96] = new KyouritsufuData(96, "シルド", "华丽");
                d[97] = new KyouritsufuData(97, "スピリト", "古怪");
                d[98] = new KyouritsufuData(98, "ストイル", "生动");
                d[99] = new KyouritsufuData(99, "ノービレ", "高贵");
                d[100] = new KyouritsufuData(100, "フォルストレ", "加强");
                d[101] = new KyouritsufuData(101, "フォルノーレ", "神秘");
                d[102] = new KyouritsufuData(102, "フォルシルド", "相同");
                d[103] = new KyouritsufuData(103, "フォルスピリト", "活力");
                d[104] = new KyouritsufuData(104, "フォルバルラ", "稳重");
                d[105] = new KyouritsufuData(105, "フォルアルカ", "全力");
                d[106] = new KyouritsufuData(106, "メジストレ", "突强");
                d[107] = new KyouritsufuData(107, "メジノーレ", "甜美");
                d[108] = new KyouritsufuData(108, "メジシルト", "坚硬");
                d[109] = new KyouritsufuData(109, "メジアルカ", "粗暴");
                d[110] = new KyouritsufuData(110, "メジバルラ", "和谐");
                d[111] = new KyouritsufuData(111, "マルカート", "强调");
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
            // 按 ID 升序：0(未装备)居首，其余 92-121 顺序稳定
            return _data.Value.Values.OrderBy(x => x.Id).ToList();
        }
    }
}
