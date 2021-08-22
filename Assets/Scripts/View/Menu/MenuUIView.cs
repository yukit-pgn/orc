using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Main.Data;
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
        [Header("BattleMenu")]
        [SerializeField] Button battleStartButton;
        [SerializeField] Button onlineMatchButton;
        [Header("DeckMenu")]
        [SerializeField] GameObject cardInfo_Prefab;
        [SerializeField] Transform cardInfoParent;
        [SerializeField] TextMeshProUGUI costText;
        [SerializeField] InputField conditionIDInputField;
        [SerializeField] InputField effect1IDInputField;
        [SerializeField] InputField effect2IDInputField;
        [SerializeField] Button addButton;
        [SerializeField] InputField deckNumberInputField;
        [SerializeField] Button loadDeckButton;
        [SerializeField] Button saveDeckButton;
        [SerializeField] InputField deckNameInputField;

        MenuType activeMenu = MenuType.Battle;

        // ボタンクリック時イベント
        Subject<ButtonType> OnClick = new Subject<ButtonType>();
        // 入力フィールド変更時イベント
        Subject<(InputFieldType, string)> OnValueChanged = new Subject<(InputFieldType, string)>();

        /// <summary>
        /// セットアップ
        /// </summary>
        public void Setup(CardData cardData, int currentDeckNumber)
        {
            // 表示・非表示設定
            battleMenuRT.gameObject.SetActive(true);
            deckMenuRT.gameObject.SetActive(false);
            createMenuRT.gameObject.SetActive(false);

            // ボタンのクリックを監視
            battleMenuButton.OnClickAsObservable().Subscribe(_ => OnClick.OnNext(ButtonType.BattleMenu)).AddTo(this);
            deckMenuButton.OnClickAsObservable().Subscribe(_ => OnClick.OnNext(ButtonType.DeckMenu)).AddTo(this);
            createMenuButton.OnClickAsObservable().Subscribe(_ => OnClick.OnNext(ButtonType.CreateMenu)).AddTo(this);
            battleStartButton.OnClickAsObservable().Subscribe(_ => OnClick.OnNext(ButtonType.BattleStart)).AddTo(this);
            onlineMatchButton.OnClickAsObservable().Subscribe(_ => OnClick.OnNext(ButtonType.OnlineMatch)).AddTo(this);
            addButton.OnClickAsObservable().Subscribe(_ => OnClick.OnNext(ButtonType.AddCard)).AddTo(this);
            loadDeckButton.OnClickAsObservable().Subscribe(_ => OnClick.OnNext(ButtonType.LoadDeckData)).AddTo(this);
            saveDeckButton.OnClickAsObservable().Subscribe(_ => OnClick.OnNext(ButtonType.SaveDeckData)).AddTo(this);

            // 追加カードデータの初期値を設定
            costText.text = cardData.cost.ToString();
            conditionIDInputField.text = cardData.conditionID.ToString();
            effect1IDInputField.text = cardData.effect1ID.ToString();
            effect2IDInputField.text = cardData.effect2ID.ToString();

            // デッキ情報の初期値を設定
            deckNumberInputField.text = currentDeckNumber.ToString();

            // 入力フィールドを監視
            conditionIDInputField.OnValueChangedAsObservable().Subscribe(v => OnValueChanged.OnNext((InputFieldType.ConditionID, v))).AddTo(this);
            effect1IDInputField.OnValueChangedAsObservable().Subscribe(v => OnValueChanged.OnNext((InputFieldType.Effect1ID, v))).AddTo(this);
            effect2IDInputField.OnValueChangedAsObservable().Subscribe(v => OnValueChanged.OnNext((InputFieldType.Effect2ID, v))).AddTo(this);
            deckNumberInputField.OnValueChangedAsObservable().Subscribe(v => OnValueChanged.OnNext((InputFieldType.DeckNumber, v))).AddTo(this);
            deckNameInputField.OnValueChangedAsObservable().Subscribe(v => OnValueChanged.OnNext((InputFieldType.DeckName, v))).AddTo(this);
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
        /// ボタンのinteractableを設定する
        /// </summary>
        public void SetButtonInteractable(ButtonType type, bool interactable)
        {
            switch(type)
            {
                case ButtonType.AddCard: addButton.interactable = interactable; break;
                case ButtonType.SaveDeckData: saveDeckButton.interactable = interactable; break;
            }
        }

        /// <summary>
        /// 追加カードのコスト表示を設定する
        /// </summary>
        public void SetNewCardCost(int cost)
        {
            costText.text = (cost < 0) ? "--" : cost.ToString();
        }

        /// <summary>
        /// デッキにカード情報を追加する
        /// </summary>
        public CardInfo AddCardInfoToDeck()
        {
            var go = Instantiate(cardInfo_Prefab, cardInfoParent);

            return go.GetComponent<CardInfo>();
        }

        /// <summary>
        /// デッキのカード情報をクリアする
        /// </summary>
        public void ClearDeck()
        {
            foreach (Transform t in cardInfoParent.transform)
            {
                Destroy(t.gameObject);
            }
        }

        /// <summary>
        /// デッキ名を設定する
        /// </summary>
        public void SetDeckName(string name)
        {
            deckNameInputField.text = name;
        }

        /// <summary>
        /// ボタンクリック時イベントを取得
        /// </summary>
        public IObservable<ButtonType> OnClickAsObservable()
        {
            return OnClick;
        }

        /// <summary>
        /// 入力フィールド変更時イベントを取得
        /// </summary>
        public IObservable<(InputFieldType, string)> OnValueChangedAsObservable()
        {
            return OnValueChanged;
        }
    }
}
