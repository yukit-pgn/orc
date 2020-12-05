using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Data
{
    /// <summary>
    /// カードデータ
    /// </summary>
    public class CardData
    {
        // コスト
        public int cost;
        // 条件ID
        public int conditionID;
        // 効果1ID
        public int effect1ID;
        // 効果2ID
        public int effect2ID;

        // コンストラクタ
        public CardData(int cost, int conditionID, int effect1ID, int effect2ID)
        {
            this.cost = cost;
            this.conditionID = conditionID;
            this.effect1ID = effect1ID;
            this.effect2ID = effect2ID;
        }
    }
}
