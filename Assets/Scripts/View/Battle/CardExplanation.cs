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
        bool canShow;

        public void Setup()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// カード情報を表示する
        /// </summary>
        public void Show(CardData cardData)
        {
            if (!canShow) { return; }

            ShowCardData(cardData);

            frameRT.localScale = Vector3.zero;
            frameRT.DOScale(Vector3.one, 0.3f);
            gameObject.SetActive(true);
        }

        /// <summary>
        /// カード情報を取得して表示する
        /// </summary>
        async void ShowCardData(CardData cardData)
        {
            var cardDataService = CardDataService.Instance;
            ilustBGImage.sprite = await cardDataService.GetConditionSprite(cardData.conditionID);
            if (cardData.effect2ID == 0)
            {
                // 1効果カード用表示
                ilustSingleImage.gameObject.SetActive(true);
                ilustDouble1Image.gameObject.SetActive(false);
                ilustDouble2Image.gameObject.SetActive(false);
                ilustSingleImage.sprite = await cardDataService.GetEffectSprite(cardData.effect1ID);
            }
            else
            {
                // 2効果カード用表示
                ilustSingleImage.gameObject.SetActive(false);
                ilustDouble1Image.gameObject.SetActive(true);
                ilustDouble2Image.gameObject.SetActive(true);
                ilustDouble1Image.sprite = await cardDataService.GetEffectSprite(cardData.effect1ID);
                ilustDouble2Image.sprite = await cardDataService.GetEffectSprite(cardData.effect2ID);
            }
            nameText.text = await cardDataService.GetCardName(cardData);
            string conditionExplanation = (cardData.conditionID == 0) ? "なし" : (await cardDataService.GetConditionExplanation(cardData.conditionID));
            string effect1Explanation = (cardData.effect1ID == 0) ? "なし" : (await cardDataService.GetEffectExplanation(cardData.effect1ID));
            string effect2Explanation = (cardData.effect2ID == 0) ? "なし" : (await cardDataService.GetEffectExplanation(cardData.effect2ID));
            explanationText.text = $"条件：{conditionExplanation}\n効果1：{effect1Explanation}\n効果2：{effect2Explanation}";
        }

        /// <summary>
        /// 閉じる
        /// </summary>
        public void Close()
        {
            canShow = false;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 表示不可能状態をリセットする
        /// </summary>
        public void ResetCanShow()
        {
            canShow = true;
        }
    }
}
