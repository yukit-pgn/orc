using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using Main.Data;
using Main.Service;

namespace Main.View.Battle
{
    public class CardExplanation : MonoBehaviour
    {
        [SerializeField] RectTransform frameRT;
        [SerializeField] Image ilustBGImage;
        [SerializeField] Image ilustSingleImage;
        [SerializeField] Image ilustDouble1Image;
        [SerializeField] Image ilustDouble2Image;
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] TextMeshProUGUI explanationText;

        CardData currentCardData;
        bool isUsable;

        public void Setup()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// カード情報を表示する
        /// </summary>
        public void Show(CardData cardData, bool isUsable)
        {
            currentCardData = cardData;
            this.isUsable = isUsable;

            frameRT.localScale = Vector3.zero;
            frameRT.DOScale(Vector3.one, 0.3f);
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 閉じる
        /// </summary>
        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
