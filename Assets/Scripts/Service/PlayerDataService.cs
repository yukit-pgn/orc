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
        List<CardData> ownedCardList;
        // デッキリスト
        List<DeckData> deckList;
        // 現在選択中のデッキ
        int currentDeckNumber = 0;
        public int CurrentDeckNumber
        {
            get
            {
                return currentDeckNumber;
            }
            set
            {
                currentDeckNumber = Mathf.Clamp(value, 0, deckList.Count - 1);
            }
        }

        List<CardData> defaultCardList = new List<CardData> {
            new CardData(8, 0, 1, 0),
            new CardData(26, 0, 3, 0),
            new CardData(44, 0, 2, 0),
            new CardData(10, 0, 4, 0),
            new CardData(30, 0, 5, 0),
            new CardData(0, 0, 6, 0),
            new CardData(10, 0, 7, 0),
            new CardData(30, 0, 8, 0),
            new CardData(26, 0, 9, 0),
            new CardData(8, 0, 10, 0),
            new CardData(10, 0, 11, 0),
            new CardData(4, 0, 12, 0),
            new CardData(4, 0, 13, 0),
            new CardData(30, 0, 14, 0),
        };

        /// <summary>
        /// 初期化時
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            deckList = new List<DeckData>();
            for (int i = 0; i < 10; i++)
            {
                deckList.Add(new DeckData {
                    name = $"デッキ{i}",
                    cardList = defaultCardList
                });
            }
        }

        /// <summary>
        /// 現在のデッキデータを取得
        /// </summary>
        public DeckData GetCurrentDeckData()
        {
            return deckList[CurrentDeckNumber];
        }

        /// <summary>
        /// 現在のデッキデータを上書き
        /// </summary>
        public void OverwriteCurrentDeckData(DeckData deckData)
        {
            deckList[CurrentDeckNumber].name = deckData.name;
            deckList[currentDeckNumber].cardList = deckData.cardList.Select(c => new CardData(c)).ToList();
        }
    }
}
