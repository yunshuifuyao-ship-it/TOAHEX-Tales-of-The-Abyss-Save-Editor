using System;
using System.Collections.Generic;

namespace TOAHEX
{
    public static class TitleDatabase
    {
        private static readonly string[][] _titlesJp = new string[8][];
        private static readonly string[][] _titlesCn = new string[8][];
        private static readonly int[] _counts = new int[8];

        static TitleDatabase()
        {
            _titlesJp[1] = new string[]
            {
                "公爵子息", "恐れる者", "親善大使", "レプリカドール", "パッセージコマンダー",
                "迷える青年", "ローレライの剣士", "タルブレイカー", "捻出投資家", "義賊の子分",
                "タオラー", "ワイルドセイバー", "クッキンガー", "ファブレ子爵", "バガボンド息子",
                "ドラゴンバスター", "ソードオブソード", "アビスレッド", "ベルセルク", "タクティカルリーダー",
                "ドラゴンバスター？"
            };
            _titlesCn[1] = new string[]
            {
                "公爵之子", "害怕的人", "亲善大使", "复制品人偶", "变奏操控者",
                "迷惘的青年", "罗蕾莱的剑士", "木桶破坏者", "筹钱投资家", "义贼的手下",
                "毛巾达人", "狂放剑士", "下厨人", "法布雷子爵", "漂泊之子",
                "屠龙者", "剑士中的剑士", "深渊红战士", "狂战士", "战术领导者",
                "屠龙者？"
            };
            _counts[1] = 21;

            _titlesJp[2] = new string[]
            {
                "謎の侵入者", "師匠の妹", "強き娘", "譜歌の理解者", "譜を読みし者",
                "女性響士", "成り行きウェイトレス", "お姉さん", "旋律奉仕者", "グランドシェフ",
                "クールレディ", "レンタルビューティー", "魔界の花", "おすましメイド", "パーフェクトヒーラー",
                "アビスブラック", "モンコレレディ"
            };
            _titlesCn[2] = new string[]
            {
                "谜之入侵者", "师父之妹", "坚强女性", "谱歌的理解者", "读谱之人",
                "女性响士", "顺其自然服务员", "姐姐", "侍奉旋律者", "伟大主厨",
                "冷静淑女", "租借泳装美女", "魔界之花", "端庄女仆", "完美治疗者",
                "深渊黑战士", "收集怪物女士"
            };
            _counts[2] = 17;

            _titlesJp[3] = new string[]
            {
                "大佐", "ネクロマンサー", "フォミクリー発案者", "見通す人", "ツンデレおじさん？",
                "皇帝の親友", "さながらギャンブラー", "料理研究家", "法の番人", "悪の譜術使い？",
                "リゾートキング", "ドクトルマンボ", "バトルマスター", "アビスブルー", "アイテムコレクター"
            };
            _titlesCn[3] = new string[]
            {
                "大佐", "死灵使", "音复制技术提出者", "洞察强者", "傲娇大叔？",
                "皇帝的亲友", "宛如赌徒", "料理研究家", "护法之人", "坏谱术使？",
                "度假村之王", "曼波医生", "战斗大师", "深渊蓝战士", "道具收集家"
            };
            _counts[3] = 15;

            _titlesJp[4] = new string[]
            {
                "導師の付き人", "元付き人", "スパイ", "最後の導師守護役", "大人な子ども（？）",
                "プッシュプルガール", "リトルビッグシェフ", "ませてぃっく", "チャイルデッシュ", "子どもじゃないモン",
                "ねこねここねこ", "リトルデビっ子", "ジェノサイドプリティ", "大料理長様", "アビスピンク"
            };
            _titlesCn[4] = new string[]
            {
                "导师的随从", "原随从", "间谍", "最后的导师守护员", "小大人(？)",
                "推箱女孩", "小大厨", "小小专家", "孩子气", "我才不是小孩子",
                "猫猫小猫猫", "小恶魔", "屠夫小可爱", "大料理长", "深渊粉战士"
            };
            _counts[4] = 15;

            _titlesJp[5] = new string[]
            {
                "護衛剣士", "理解ある幼なじみ", "マルクト貴族", "マブダチ", "シグムント流兵法家",
                "ガンバリスト", "クールコック", "ブレードテイマー", "海のサル", "スケベ大魔王",
                "憩いの配膳者", "ロマンチェイサー", "ゴールデンナイト", "アビスオレンジ", "譜業隣人",
                "スマートスタイル"
            };
            _titlesCn[5] = new string[]
            {
                "护卫剑士", "善解人意的竹马", "玛尔库特贵族", "铁哥们", "西格蒙特流兵法家",
                "加油鼓劲者", "酷哥厨师", "剑刃调整师", "救生猿", "好色大魔王",
                "放松配餐人", "追逐浪漫者", "黄金骑士", "深渊橙战士", "谱业邻人",
                "别致风格"
            };
            _counts[5] = 16;

            _titlesJp[6] = new string[]
            {
                "キムラスカ王女", "偽りの姫君", "ランバルディアの子", "モテモテプリンセス", "見てみたガール",
                "マルクトの星", "ラビリンスガール", "ウィルインペリアル", "南国の蝶", "カラミティシェフ",
                "アビスグリーン", "コロシアムプリンセス", "愛国姫", "漫遊冒険娘"
            };
            _titlesCn[6] = new string[]
            {
                "齐姆拉斯卡公主", "假公主", "兰巴尔迪亚之子", "超受欢迎公主", "满心好奇女孩",
                "玛尔库特之星", "迷宫女孩", "王权意志", "南国之蝶", "灾星主厨",
                "深渊绿战士", "斗技场公主", "爱国者", "漫游冒险女"
            };
            _counts[6] = 14;

            _titlesJp[7] = new string[]
            {
                "六神将", "母想い", "アビスシルバー"
            };
            _titlesCn[7] = new string[]
            {
                "六神将", "念母心", "深渊银战士"
            };
            _counts[7] = 3;
        }

        public static string GetTitleName(int slotIndex, int saveIndex)
        {
            if (slotIndex < 1 || slotIndex > 7 || saveIndex < 1 || saveIndex > _counts[slotIndex])
                return "???";
            return _titlesJp[slotIndex][saveIndex - 1];
        }

        public static string GetTitleNameCn(int slotIndex, int saveIndex)
        {
            if (slotIndex < 1 || slotIndex > 7 || saveIndex < 1 || saveIndex > _counts[slotIndex])
                return "???";
            return LanguageConfig.Current == Language.JP
                ? _titlesJp[slotIndex][saveIndex - 1]
                : _titlesCn[slotIndex][saveIndex - 1];
        }

        public static int GetTitleCount(int slotIndex)
        {
            if (slotIndex < 1 || slotIndex > 7) return 0;
            return _counts[slotIndex];
        }

        public static int GetDefaultTitleIndex(int slotIndex)
        {
            return 0;
        }

        public class TitleEntry
        {
            public int Index;
            public string Jp;
            public string Cn;
            public TitleEntry(int index, string jp, string cn) { Index = index; Jp = jp; Cn = cn; }
        }

        public static List<TitleEntry> GetAllTitles(int slotIndex)
        {
            var list = new List<TitleEntry>();
            if (slotIndex < 1 || slotIndex > 7) return list;
            for (int i = 0; i < _counts[slotIndex]; i++)
                list.Add(new TitleEntry(i, _titlesJp[slotIndex][i], _titlesCn[slotIndex][i]));
            return list;
        }
    }
}
