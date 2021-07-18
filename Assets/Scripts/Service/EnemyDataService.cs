using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UniRx;
using Main.Data;

namespace Main.Service
{
    public class EnemyDataService : SingletonMonoBehaviour<EnemyDataService>
    {
        public List<DeckData> cardDataList;
        public int enemyNumber;

        protected override void Awake()
        {
            base.Awake();

            cardDataList = Enumerable.Repeat(new DeckData {
                name = "test",
                cardList = new List<CardData> {
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
                }
            }, 1)
            .ToList();
        }

        public DeckData GetDeckData()
        {
            return cardDataList[enemyNumber];
        }
    }
}
