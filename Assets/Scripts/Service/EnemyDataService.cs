using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Main.Data;

namespace Main.Service
{
    public class EnemyDataService : SingletonMonoBehaviour<EnemyDataService>
    {
        public List<DeckData> cardDataList;
        public int enemyNumber;

        public DeckData GetDeckData()
        {
            return cardDataList[enemyNumber];
        }
    }
}
