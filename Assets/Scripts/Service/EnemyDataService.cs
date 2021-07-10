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
                    new CardData(10, 0, 1, 0),
                    new CardData(10, 0, 1, 0),
                    new CardData(10, 0, 1, 0),
                    new CardData(10, 0, 1, 0),
                    new CardData(10, 0, 1, 0),
                    new CardData(10, 0, 1, 0),
                    new CardData(10, 0, 1, 0),
                    new CardData(10, 0, 1, 0),
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
