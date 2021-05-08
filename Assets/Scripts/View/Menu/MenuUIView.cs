using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Main.Data.Menu;

namespace Main.View.Menu
{
    public class MenuUIView : MonoBehaviour
    {
        [SerializeField] Button battleMenuButton;
        [SerializeField] Button deckMenuButton;
        [SerializeField] Button createMenuButton;
        [SerializeField] RectTransform battleMenuRT;
        [SerializeField] RectTransform deckMenuRT;
        [SerializeField] RectTransform createMenuRT;

        MenuType activeMenu = MenuType.Battle;

        // ボタンクリック時イベント
        Subject<ButtonType> OnClick = new Subject<ButtonType>();

        /// <summary>
        /// セットアップ
        /// </summary>
        public void Setup()
        {
            // 表示・非表示設定
            battleMenuRT.gameObject.SetActive(true);
            deckMenuRT.gameObject.SetActive(false);
            createMenuRT.gameObject.SetActive(false);

            // ボタンのクリックを監視
            battleMenuButton.OnClickAsObservable().Subscribe(_ => OnClick.OnNext(ButtonType.BattleMenu)).AddTo(this);
            deckMenuButton.OnClickAsObservable().Subscribe(_ => OnClick.OnNext(ButtonType.DeckMenu)).AddTo(this);
            createMenuButton.OnClickAsObservable().Subscribe(_ => OnClick.OnNext(ButtonType.CreateMenu)).AddTo(this);
        }

        /// <summary>
        /// メニューを切り替える
        /// </summary>
        public async UniTask ChangeMenu(MenuType type)
        {
            // 切り替える対象が今出ているものと同じならリターン
            if (type == activeMenu) { return; }

            // 定数
            var duration = 0.5f;

            // スライドアウトとインを同時に
            await UniTask.WhenAll(
                SlideOutMenu(activeMenu, duration, activeMenu > type),
                SlideInMenu(type, duration, type > activeMenu)
            );

            // 情報の更新
            activeMenu = type;
        }

        /// <summary>
        /// メニューをスライドアウトする
        /// </summary>
        async UniTask SlideOutMenu(MenuType type, float duration, bool isRight)
        {
            // スライドアウトする方向
            var direction = isRight ? Vector2.right : Vector2.left;
            // スライドアウトする対象
            var rt = MenuTypeToRT(type);

            // 移動
            rt.DOAnchorPos(direction * 1080, duration);
            await UniTask.Delay((int)(duration * 1000));

            // 非表示
            rt.gameObject.SetActive(false);
        }

        /// <summary>
        /// メニューをスライドアウトする
        /// </summary>
        async UniTask SlideInMenu(MenuType type, float duration, bool isFromRight)
        {
            // スライドインしてくる方向
            var direction = isFromRight ? Vector2.right : Vector2.left;
            // スライドインする対象
            var rt = MenuTypeToRT(type);

            // 表示
            rt.gameObject.SetActive(true);
            // 初期位置
            rt.anchoredPosition = direction * 1080;

            // 移動
            rt.DOAnchorPos(Vector2.zero, duration);
            await UniTask.Delay((int)(duration * 1000));
        }
        
        /// <summary>
        /// MenuTypeから対応するRectTransformを取得する
        /// </summary>
        RectTransform MenuTypeToRT(MenuType type)
        {
            switch (type)
            {
                case MenuType.Battle: return battleMenuRT;
                case MenuType.Deck:   return deckMenuRT;
                case MenuType.Create: return createMenuRT;
                default:              return null;
            }
        }

        /// <summary>
        /// ボタンクリック時イベントを取得
        /// </summary>
        public IObservable<ButtonType> OnClickAsObservable()
        {
            return OnClick;
        }
    }
}
