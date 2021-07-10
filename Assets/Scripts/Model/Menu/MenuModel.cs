using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using Main.Data;
using Main.Service;

namespace Main.Model.Menu
{
    public class MenuModel
    {
        CardDataService cardDataService;

        public CardData NewCardData { get; set; }
        public DeckData CurrentDeckData { get; set; }

        /// <summary>
        /// セットアップ
        /// </summary>
        public async UniTask Setup()
        {
            cardDataService = CardDataService.Instance;
            NewCardData = new CardData(0, 0, 1, 0);
            NewCardData.cost = await cardDataService.GetCardCost(NewCardData);
            CurrentDeckData = new DeckData();
            CurrentDeckData.cardList = new List<CardData>();
        }
    }
}
