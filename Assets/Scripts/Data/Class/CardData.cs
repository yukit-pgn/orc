using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Data
{
    [System.Serializable]
    /// <summary>
    /// カードデータ
    /// </summary>
    public class CardData
    {
        public static bool IsRegistered { get; private set; }
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

        public CardData(CardData original)
        {
            this.cost = original.cost;
            this.conditionID = original.conditionID;
            this.effect1ID = original.effect1ID;
            this.effect2ID = original.effect2ID;
        }

        public CardData()
        {}

        public bool IsSame(CardData cardData)
        {
            return this.conditionID == cardData.conditionID &&
                this.effect1ID == cardData.effect1ID &&
                this.effect2ID == cardData.effect2ID;
        }

        public static object Deserialize(byte[] data)
        {
            return new CardData(data[0], data[1], data[2], data[3]);
        }

        public static byte[] Serialize(object cardData)
        {
            var c = (CardData)cardData;
            return new byte[] { (byte)c.cost, (byte)c.conditionID, (byte)c.effect1ID, (byte)c.effect2ID };
        }

        public static void Register()
        {
            IsRegistered = true;
            ExitGames.Client.Photon.PhotonPeer.RegisterType(typeof(CardData), 1, Serialize, Deserialize);
        }
    }
}
