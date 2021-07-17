using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Data
{
    [System.Serializable]
    public class DeckData
    {
        // デッキ名
        public string name;
        // カードリスト
        public List<CardData> cardList;
    }
}
