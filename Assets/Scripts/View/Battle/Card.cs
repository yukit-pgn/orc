using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;
using TMPro;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Main.Data;
using Main.Service;
using Main.Extension;

namespace Main.View.Battle
{
    public class Card : MonoBehaviour
    {
        [SerializeField] GameObject cardFront;
        [SerializeField] GameObject cardBack;
        [SerializeField] SpriteRenderer ilustBGSR;
        [SerializeField] SpriteRenderer ilustSingleSR;
        [SerializeField] SpriteRenderer ilustDouble1SR;
        [SerializeField] SpriteRenderer ilustDouble2SR;
        [SerializeField] TextMeshPro nameText;
        [SerializeField] TextMeshPro costText;

        CardDataService cardDataService;

        // ロングタップ時イベント
        Subject<CardData> OnLongTap = new Subject<CardData>();
        // ドラッグ時イベント
        Subject<Unit> OnDrag = new Subject<Unit>();
        // タッチリリース時イベント
        Subject<(CardData, Vector3)> OnRelease = new Subject<(CardData, Vector3)>();

        // カードデータ
        public CardData CardData { get; private set; }
        // 選択可能か
        public bool Selectable { get; set; }

        /// <summary>
        /// セットアップ
        /// </summary>
        public async UniTask Setup(CardData cardData, bool front = false)
        {
            // 表裏の設定
            cardFront.SetActive(front);
            cardBack.SetActive(!front);

            // 表示設定
            this.CardData = cardData;
            cardDataService = CardDataService.Instance;
            ilustBGSR.sprite = await cardDataService.GetConditionSprite(cardData.conditionID);
            if (cardData.effect2ID == 0)
            {
                // 1効果カード用表示
                ilustSingleSR.gameObject.SetActive(true);
                ilustDouble1SR.gameObject.SetActive(false);
                ilustDouble2SR.gameObject.SetActive(false);
                ilustSingleSR.sprite = await cardDataService.GetEffectSprite(cardData.effect1ID);
            }
            else
            {
                // 2効果カード用表示
                ilustSingleSR.gameObject.SetActive(false);
                ilustDouble1SR.gameObject.SetActive(true);
                ilustDouble2SR.gameObject.SetActive(true);
                ilustDouble1SR.sprite = await cardDataService.GetEffectSprite(cardData.effect1ID);
                ilustDouble2SR.sprite = await cardDataService.GetEffectSprite(cardData.effect2ID);
            }
            nameText.text = await cardDataService.GetCardName(cardData);
            costText.text = (await cardDataService.GetCardCost(cardData)).ToString();

            var trigger = GetComponent<MyObservableEventTrigger>();
            // ロングタップ
            trigger.OnLongTapAsObservable(0.5f).Subscribe(LongTap).AddTo(this);
            // 押し込み
            trigger.OnPointerDownAsObservable().Subscribe(PointerDown).AddTo(this);
            // ドラッグ
            trigger.OnDragAsObservable().Subscribe(Drag).AddTo(this);
            // 離す
            trigger.OnPointerUpAsObservable().Subscribe(PointerUp).AddTo(this);
        }

        /// <summary>
        /// スクリーン座標をワールド座標に変換する
        /// </summary>
        Vector3 ScreenToWorldPoint(Vector2 screenPos)
        {
            return Camera.main.ScreenToWorldPoint((Vector3)screenPos + Vector3.forward * 20);
        }

        /// <summary>
        /// ロングタップ時処理
        /// </summary>
        void LongTap(PointerEventData pointer)
        {
            if (!Selectable) return;

            OnLongTap.OnNext(CardData);
        }

        /// <summary>
        /// 押し込み時処理
        /// </summary>
        void PointerDown(PointerEventData pointer)
        {
            if (!Selectable) return;

            // 拡大
            transform.DOScale(Vector3.one * 1.5f, 0.3f);
            // レイヤーを最前列に
            SetSortingOrder(100);
        }

        /// <summary>
        /// ドラッグ時処理
        /// </summary>
        void Drag(PointerEventData pointer)
        {
            if (!Selectable) return;

            transform.position = ScreenToWorldPoint(pointer.position);
            OnDrag.OnNext(Unit.Default);
        }

        /// <summary>
        /// タッチリリース時処理
        /// </summary>
        void PointerUp(PointerEventData pointer)
        {
            if (!Selectable) return;

            transform.DOScale(Vector3.one, 0.3f);
            OnRelease.OnNext((CardData, ScreenToWorldPoint(pointer.position)));
        }

        /// <summary>
        /// 移動
        /// </summary>
        public async UniTask Move(Vector3 pos, float duration)
        {
            if (duration <= 0f)
            {
                transform.position = pos;
            }
            else
            {
                transform.DOMove(pos, duration);
                await UniTask.Delay((int)(duration * 1000));
            }
        }

        /// <summary>
        /// 裏返す
        /// </summary>
        public async UniTask TurnOver(bool front, float duration)
        {
            transform.DORotate(Vector3.down * 90, duration / 2f).SetEase(Ease.InQuad);
            await UniTask.Delay((int)(duration * 500));
            
            // 表・裏の表示
            cardFront.SetActive(front);
            cardBack.SetActive(!front);

            transform.rotation = Quaternion.AngleAxis(90, Vector3.up);
            transform.DORotateQuaternion(Quaternion.AngleAxis(-1, Vector3.up), duration / 2f);
            await UniTask.Delay((int)(duration * 500));
        }

        /// <summary>
        /// ソーティングオーダーを変更する
        /// </summary>
        public void SetSortingOrder(int order)
        {
            GetComponent<SortingGroup>().sortingOrder = order;
        }

        /// <summary>
        /// ロングタップ時イベントを取得
        /// </summary>
        public IObservable<CardData> OnLongTapAsObservable()
        {
            return OnLongTap;
        }

        /// <summary>
        /// ドラッグ時イベントを取得
        /// </summary>
        public IObservable<Unit> OnDragAsObservable()
        {
            return OnDrag;
        }

        /// <summary>
        /// タッチリリース時イベントを取得
        /// </summary>
        public IObservable<(CardData, Vector3)> OnReleaseAsObservable()
        {
            return OnRelease;
        }
    }
}
