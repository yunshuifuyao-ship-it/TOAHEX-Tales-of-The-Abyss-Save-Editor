using System;
using System.Collections.Generic;
using System.Linq;

namespace TOAHEX
{
    public static class ADSkillDatabase
    {
        private static Dictionary<int, string> _datNames;

        public static void LoadFromDat(string filePath)
        {
            _datNames = DatDataLoader.LoadAcsData(filePath);

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
                d[0] = ("無し", "无");
                d[1] = ("バックステップ", "后跳");
                d[2] = ("リカバリング", "缓冲");
                d[3] = ("クリティカルガード", "前冲防御");
                d[4] = ("マジックガード", "魔法防御");
                d[5] = ("フリーラン", "自由奔跑");
                d[6] = ("オーバーリミッツ", "OVL");
                d[7] = ("アロース", "蓄能");
                d[8] = ("スペシャル", "秘技");
                d[9] = ("イフィシェント", "效率提升");
                d[10] = ("パワーチャージ", "力量充填");
                d[11] = ("クリティカル", "暴击");
                d[12] = ("フェイント", "欺敌物攻");
                d[13] = ("コンボプラス", "追加攻击");
                d[14] = ("コンボプラス2", "追加攻击2");
                d[15] = ("コンボプラス3", "追加攻击3");
                d[16] = ("コンボプラス4", "追加攻击4");
                d[17] = ("パッシブセーフ", "冲击保护");
                d[18] = ("ウェルガード", "加强防御");
                d[19] = ("バックガード", "背后防御");
                d[20] = ("インバリッド・アタック", "攻击无效化");
                d[21] = ("イミュニティ", "免疫");
                d[22] = ("エンデューロ", "坚毅");
                d[23] = ("マジッククリティカル", "魔法暴击");
                d[24] = ("エンドラッキー", "幸运结尾");
                d[25] = ("マジックフェイント", "欺敌魔法");
                d[26] = ("スペルラッキー", "幸运咏唱");
                d[27] = ("スピードスペル", "快速咏唱");
                d[28] = ("マジックチャージ", "魔力充填");
                d[29] = ("インバリッド・マジック", "魔法无效化");
                d[30] = ("レジスト", "抵抗");
                d[31] = ("アンエレメンタル", "反元素");
                d[32] = ("マジックウェルガード", "加强魔防");
                d[33] = ("エフェクティブ", "效果增幅");
                d[34] = ("ライフアップ", "生命上升");
                d[35] = ("メンタルヒール", "精神治愈");
                d[36] = ("ハピネスシング", "喜悦奖励");
                d[37] = ("ライフヒール", "生命治愈");
                d[38] = ("メンタルアップ", "精神上升");
                d[39] = ("ＨＰリカバー", "HP恢复");
                d[40] = ("ＴＰリカバー", "TP恢复");
                d[41] = ("ＥＸＰプラス", "EXP增加");
                d[42] = ("ダッシュ", "冲刺");
                d[43] = ("エスケープ", "逃跑");
                d[44] = ("カムバック", "加快回归");
                d[45] = ("グッドジョブ", "优秀后援");
                d[46] = ("リミッター", "延长极限");
                d[47] = ("ランディング", "平稳着地");
                d[48] = ("ハイターン", "灵活转向");
                d[49] = ("エリアルジャンプ", "凌空跳跃");
                d[50] = ("リカバリーアタック", "缓冲攻击");
                d[51] = ("ハイコンボ", "落地追击");
                d[52] = ("カウンターコンボ", "反击追击");
                d[53] = ("クロスカウンター", "交叉反击");
                d[54] = ("カウンター", "反击");
                d[55] = ("インエレメンタル", "元素增强");
                d[56] = ("フラッシュ", "闪耀瞬间");
                d[57] = ("ステップアウェイ", "安全退后");
                d[58] = ("ラッキープール", "幸运池");
                d[59] = ("リコール", "复活");
                d[60] = ("ペインリフレクト", "伤害反射");
                d[61] = ("アクシデンタル", "偶然制术");
                d[62] = ("ルーズレスソウル", "坚定之魂");
                d[63] = ("メンタルサプライ", "受击提振");
                d[64] = ("スキルガード", "技能防御");
                d[65] = ("ライフリバース", "生命回溯");
                d[66] = ("エンジェルコール", "天使复活");
                d[67] = ("グローリー", "名誉荣光");
                d[68] = ("スペルボルテージ", "咏唱加压");
                d[69] = ("スペルキープ", "接续咏唱");
                d[70] = ("リダクション", "消耗削减");
                d[71] = ("リズム", "掌控节奏");
                d[72] = ("サプレスガード", "抑制防御");
                d[73] = ("アイテムユーザー", "擅使道具");
                d[74] = ("ピコハンリベンジ", "气锤报复");
                d[75] = ("オートメディスン", "自动恢复");
                d[76] = ("キャンセラー", "切招");
                d[77] = ("アイテムゲッター", "擅获道具");
                d[78] = ("ローバーアイテム", "卷取道具");
                d[79] = ("ステータスガード", "能力值防御");
                d[80] = ("グレイス", "优雅咏唱");
                d[81] = ("ターンレス", "无敌连招");
                d[82] = ("リアガーダー", "殿后");
                d[83] = ("コンディションガード", "病理防御");
                d[84] = ("バックアクション", "后撤行动");
                d[85] = ("アイテムスロー", "道具投掷");
                d[86] = ("ステップロング", "长距离后跳");
                d[87] = ("スペルエンド", "魔法僵硬缩减");

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
