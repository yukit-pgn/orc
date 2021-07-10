using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using Main.Data;

namespace Main.View.Menu
{

    public class CardInfo : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI costText;
        [SerializeField] TextMeshProUGUI conditionIDText;
        [SerializeField] TextMeshProUGUI effect1IDText;
        [SerializeField] TextMeshProUGUI effect2IDText;
        [SerializeField] Button deleteButton;

        public CardData CardData { get; private set; }

        Subject<Unit> OnDelete = new Subject<Unit>();
        
        /// <summary>
        /// セットアップ
        /// </summary>
        public void Setup(CardData cardData)
        {
            CardData = cardData;
            
            // パラメータを表示
            costText.text = cardData.cost.ToString();
            conditionIDText.text = cardData.conditionID.ToString();
            effect1IDText.text = cardData.effect1ID.ToString();
            effect2IDText.text = cardData.effect2ID.ToString();

            // 削除ボタンを監視
            deleteButton.OnClickAsObservable().Subscribe(OnDelete.OnNext).AddTo(this);
        }

        /// <summary>
        /// 削除ボタンクリック時イベントを取得
        /// </summary>
        public IObservable<Unit> OnDeleteAsObservable()
        {
            return OnDelete;
        }
    }
}
