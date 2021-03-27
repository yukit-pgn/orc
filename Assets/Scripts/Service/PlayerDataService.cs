using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
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

        protected override void Awake()
        {
            base.Awake();

            deckList = Enumerable.Repeat(new DeckData {
                name = "test",
                cardList = new List<(CardData cardData, int count)> {
                    (new CardData(10, 0, 1, 0), 10)
                }
            }, 1)
            .ToList();
        }

        public DeckData GetCurrentDeckData()
        {
            return deckList[currentDeckNumber];
        }
    }
}
