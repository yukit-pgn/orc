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
        [SerializeField] Button useButton;
        [SerializeField] Button backButton;
        [SerializeField] RectTransform frameRT;
        [SerializeField] SpriteRenderer ilustBGSR;
        [SerializeField] SpriteRenderer ilustSingleSR;
        [SerializeField] SpriteRenderer ilustDouble1SR;
        [SerializeField] SpriteRenderer ilustDouble2SR;
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] TextMeshProUGUI explanationText;

        Button bgButton;
        CardData currentCardData;
        bool isUsable;

        Subject<CardData> OnUse = new Subject<CardData>();
        Subject<Unit> OnBack = new Subject<Unit>();

        void Awake()
        {
            useButton.OnClickAsObservable().Subscribe(_ => Use()).AddTo(this);
            backButton.OnClickAsObservable().Subscribe(_ => Back()).AddTo(this);
            bgButton = GetComponent<Button>();
            bgButton.OnClickAsObservable().Subscribe(_ => Back()).AddTo(this);
            bgButton.enabled = false;
            gameObject.SetActive(false);
            useButton.gameObject.SetActive(false);
            backButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// カードが使用可能であれば使用する
        /// </summary>
        void Use()
        {
            if (isUsable)
            {
                OnUse.OnNext(currentCardData);
                Close();
            }
        }

        /// <summary>
        /// カードを使用せずに戻る
        /// </summary>
        void Back()
        {
            OnBack.OnNext(Unit.Default);
            Close();
        }

        /// <summary>
        /// カード情報を表示する
        /// </summary>
        public async void Show(CardData cardData, bool isUsable)
        {
            currentCardData = cardData;
            this.isUsable = isUsable;

            frameRT.localScale = Vector3.zero;
            frameRT.DOScale(Vector3.one, 0.3f)
            .OnComplete(() => 
            {
                useButton.gameObject.SetActive(true);
                backButton.gameObject.SetActive(true);
                bgButton.enabled = true;
            });
            gameObject.SetActive(true);
        }

        async void Close()
        {
            useButton.gameObject.SetActive(false);
            backButton.gameObject.SetActive(false);
            bgButton.enabled = false;
            frameRT.localScale = Vector3.one;
            frameRT.DOScale(Vector3.zero, 0.1f);
            await UniTask.Delay(100);
            gameObject.SetActive(false);
        }

        public IObservable<CardData> OnUseAsObservable()
        {
            return OnUse;
        }

        public IObservable<Unit> OnBackAsObservable()
        {
            return OnBack;
        }
    }
}
