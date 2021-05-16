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
        public int currentDeckNumber = 1;

        /// <summary>
        /// 初期化時
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            deckList = new List<DeckData>{
                new DeckData {
                    name = "test",
                    cardList = new List<(CardData cardData, int count)> {
                        (new CardData(10, 0, 2, 0), 1),
                        (new CardData(10, 1, 2, 0), 1),
                        (new CardData(10, 2, 2, 0), 1),
                        (new CardData(10, 3, 2, 0), 1),
                        (new CardData(10, 4, 2, 0), 1),
                        (new CardData(10, 5, 2, 0), 1),
                        (new CardData(10, 6, 2, 0), 1),
                    }
                },
                new DeckData {
                    name = "test2",
                    cardList = new List<(CardData cardData, int count)> {
                        (new CardData(10, 0, 1, 0), 1),
                        (new CardData(10, 0, 2, 13), 1),
                        (new CardData(10, 0, 3, 9), 1),
                        (new CardData(10, 0, 4, 0), 1),
                        (new CardData(10, 0, 5, 11), 1),
                        (new CardData(10, 0, 6, 0), 1),
                        (new CardData(10, 0, 7, 0), 1),
                        (new CardData(10, 0, 8, 0), 1),
                        (new CardData(10, 0, 10, 12), 1),
                        (new CardData(10, 0, 14, 0), 1),
                    }
                }
            };
        }

        /// <summary>
        /// 現在のデッキデータを取得
        /// </summary>
        public DeckData GetCurrentDeckData()
        {
            return deckList[currentDeckNumber];
        }
    }
}
