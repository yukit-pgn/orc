using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Data
{
    /// <summary>
    /// 条件データ
    /// </summary>
    public class ConditionData
    {
        // ID
        public int id;
        // 減少コスト
        public int cost;
        // 名前
        public string name;
        // 説明テキスト
        public string explanation;

        // コンストラクタ
        public ConditionData(int id, int cost, string name, string explanation)
        {
            this.id = id;
            this.cost = cost;
            this.name = name;
            this.explanation = explanation;
        }
    }
}
