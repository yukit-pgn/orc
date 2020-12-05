using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Data
{
    /// <summary>
    /// 効果データ
    /// </summary>
    public class EffectData
    {
        // ID
        public int id;
        // コスト
        public int cost;
        // 名前
        public string name;
        // 説明テキスト
        public string explanation;

        // コンストラクタ
        public EffectData(int id, int cost, string name, string explanation)
        {
            this.id = id;
            this.cost = cost;
            this.name = name;
            this.explanation = explanation;
        }
    }
}
