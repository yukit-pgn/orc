using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Main.Data;

namespace Main.Service
{
    /// <summary>
    /// プレイヤーデータを管理するクラス
    /// </summary>
    public class PlayerDataService : SingletonMonoBehaviour<PlayerDataService>
    {
        // 所持カードリスト
        public List<CardData> ownedCardList;
        // デッキリスト
        public List<DeckData> deckList;
        // 現在選択中のデッキ
        public int currentDeckNumber = 0;

        public DeckData GetCurrentDeckData()
        {
            return deckList[currentDeckNumber];
        }
    }
}
