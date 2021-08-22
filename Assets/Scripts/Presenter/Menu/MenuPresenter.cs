using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Main.Service;
using Main.View.Menu;
using Main.Model.Menu;
using Main.Data.Menu;
using Main.Data;

namespace Main.Presenter.Menu
{
    public class MenuPresenter : MonoBehaviour
    {
        [SerializeField] MenuUIView uiView;
        [SerializeField] MatchingView matchingView;
        
        MenuModel menuModel;

        CardDataService cardDataService;
        PlayerDataService playerDataService;

        /// <summary>
        /// 初期設定
        /// </summary>
        async void Start()
        {
            cardDataService = CardDataService.Instance;
            playerDataService = PlayerDataService.Instance;

            await SetUpModels();

            SetUpViews();

            Bind();

            SetEvents();

            SetUpdateEvents();

            // 現在のデッキデータを読み込む
            LoadCurrnetDeckData();
        }

        /// <summary>
        /// Modelの設定
        /// </summary>
        async UniTask SetUpModels()
        {
            // MenuModelの生成とセットアップ
            menuModel = new MenuModel();
            await menuModel.Setup();
        }


        /// <summary>
        /// Viewの設定
        /// </summary>
        void SetUpViews()
        {
            // uiViewのセットアップ
            uiView.Setup(menuModel.NewCardData, playerDataService.CurrentDeckNumber);
        }

        /// <summary>
        /// Modelの監視
        /// </summary>
        void Bind()
        {
        }

        /// <summary>
        /// Viewのイベントの設定
        /// </summary>
        void SetEvents()
        {
            // uiViewの監視
            uiView.OnClickAsObservable().Subscribe(OnClick).AddTo(this);
            uiView.OnValueChangedAsObservable().Subscribe(tpl => OnValueChanged(tpl.Item1, tpl.Item2)).AddTo(this);

            // matchingViewの監視
            matchingView.OnMatch.Subscribe(_ => BattleStart()).AddTo(this);
        }

        /// <summary>
        /// Update, FixedUpdateのイベントの設定
        /// </summary>
        void SetUpdateEvents()
        {
        }

        /// <summary>
        /// ボタンクリック時
        /// </summary>
        async void OnClick(ButtonType type)
        {
            switch (type)
            {
                case ButtonType.BattleMenu:
                    await uiView.ChangeMenu(MenuType.Battle);
                    break;
                case ButtonType.DeckMenu:
                    await uiView.ChangeMenu(MenuType.Deck);
                    break;
                case ButtonType.CreateMenu:
                    await uiView.ChangeMenu(MenuType.Create);
                    break;
                case ButtonType.BattleStart:
                    matchingView.PlayOffline();
                    break;
                case ButtonType.OnlineMatch:
                    matchingView.PlayOnline();
                    break;
                case ButtonType.AddCard:
                    var cardData = new CardData(menuModel.NewCardData);
                    AddCardInfoToDeck(cardData);
                    // カード追加ボタンを無効化
                    uiView.SetButtonInteractable(ButtonType.AddCard, false);
                    // セーブボタンを有効化
                    uiView.SetButtonInteractable(ButtonType.SaveDeckData, true);
                    break;
                case ButtonType.LoadDeckData:
                    LoadCurrnetDeckData();
                    break;
                case ButtonType.SaveDeckData:
                    SaveCurrentDeckData();
                    break;
            }
        }

        /// <summary>
        /// 入力フィールド変更時
        /// </summary>
        void OnValueChanged(InputFieldType type, string v)
        {
            switch (type)
            {
                case InputFieldType.ConditionID:
                case InputFieldType.Effect1ID:
                case InputFieldType.Effect2ID:
                    ChangeNewCardData(type, v);
                    break;

                case InputFieldType.DeckNumber:
                    if (int.TryParse(v, out var value))
                    {
                        playerDataService.CurrentDeckNumber = value;
                    }
                    break;

                case InputFieldType.DeckName:
                    menuModel.CurrentDeckData.name = v;
                    break;
            }
        }

        void BattleStart()
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.IsMessageQueueRunning = false;
            
            SceneService.ChangeScene("BattleScene", 1f);
        }

        /// <summary>
        /// 追加カードデータの情報を変更する
        /// </summary>
        async void ChangeNewCardData(InputFieldType type, string v)
        {
            
            if (int.TryParse(v, out var value))
            {
                var newCardData = menuModel.NewCardData;
                // 条件IDリストを取得
                var conditionIDList = await cardDataService.GetConditionIDList();
                // 効果IDリストを取得
                var effectIDList = await cardDataService.GetEffectIDList();

                switch (type)
                {
                    case InputFieldType.ConditionID:
                        // 追加カードデータの条件を変更
                        newCardData.conditionID = value;
                        break;
                    case InputFieldType.Effect1ID:
                        // 追加カードデータの効果1を変更
                        newCardData.effect1ID = value;
                        break;
                    case InputFieldType.Effect2ID:
                        // 追加カードデータの効果2を変更
                        newCardData.effect2ID = value;
                        break;
                }

                // カードデータが存在するかチェック
                bool enable = 
                    (newCardData.conditionID == 0 || conditionIDList.Contains(newCardData.conditionID)) &&
                    effectIDList.Contains(newCardData.effect1ID) &&
                    (newCardData.effect2ID == 0 || effectIDList.Contains(newCardData.effect2ID)) &&
                    newCardData.effect1ID != newCardData.effect2ID;
                // デッキ内の重複チェック
                bool isDuplicate = menuModel.CurrentDeckData.cardList.Any(newCardData.IsSame);
                // デッキ枚数
                bool isMax = menuModel.CurrentDeckData.cardList.Count >= 16;
                if (enable)
                {
                    // コストを計算
                    int cost = await cardDataService.GetCardCost(newCardData);
                    newCardData.cost = cost;
                    uiView.SetNewCardCost(cost);
                }
                else
                {
                    uiView.SetNewCardCost(-1);
                }

                if (enable && !isDuplicate && !isMax)
                {
                    // カード追加ボタンを有効化
                    uiView.SetButtonInteractable(ButtonType.AddCard, true);
                }
                else
                {
                    // カード追加ボタンを無効化
                    uiView.SetButtonInteractable(ButtonType.AddCard, false);
                }
            }
            else
            {
                    // カード追加ボタンを無効化
                    uiView.SetButtonInteractable(ButtonType.AddCard, false);
                    uiView.SetNewCardCost(-1);
            }
        }

        /// <summary>
        /// デッキにカード情報を追加する
        /// </summary>
        void AddCardInfoToDeck(CardData cardData)
        {
            var cardInfo = uiView.AddCardInfoToDeck();

            cardInfo.Setup(cardData);
            cardInfo.OnDeleteAsObservable().Subscribe(_ =>
            {
                // 破棄
                Destroy(cardInfo.gameObject);
                // カードリストから削除
                menuModel.CurrentDeckData.cardList.Remove(cardData);
                // カード追加ボタンの有効化チェック
                if (cardData.IsSame(menuModel.NewCardData))
                {
                    uiView.SetButtonInteractable(ButtonType.AddCard, true);
                }
                // セーブボタンを有効化
                uiView.SetButtonInteractable(ButtonType.SaveDeckData, true);
            })
            .AddTo(cardInfo);

            // カードリストに追加
            menuModel.CurrentDeckData.cardList.Add(cardData);
        }

        /// <summary>
        /// 現在のデッキデータを読み込む
        /// </summary>
        void LoadCurrnetDeckData()
        {
            var deckData = playerDataService.GetCurrentDeckData();
            // デッキのクリア
            menuModel.CurrentDeckData.cardList.Clear();
            uiView.ClearDeck();
            // デッキ名の読み込み(この操作によりmenuModelのCurrentDeckData.nameも変更される)
            uiView.SetDeckName(deckData.name);
            // デッキにカード情報を追加
            deckData.cardList.ForEach(AddCardInfoToDeck);

            // カード追加ボタンを有効/無効化
            uiView.SetButtonInteractable(ButtonType.AddCard, !menuModel.CurrentDeckData.cardList.Any(menuModel.NewCardData.IsSame));
            // セーブボタンを無効化
            uiView.SetButtonInteractable(ButtonType.SaveDeckData, false);
        }

        /// <summary>
        /// デッキの条件を満たしていれば現在のデッキデータをセーブする
        /// </summary>
        void SaveCurrentDeckData()
        {
            var deckData = menuModel.CurrentDeckData;
            if (deckData.cardList.Count < 8 || deckData.cardList.Count > 16) { return; }

            // データを上書き
            playerDataService.OverwriteCurrentDeckData(deckData);

            // セーブボタンを無効化
            uiView.SetButtonInteractable(ButtonType.SaveDeckData, false);
        }
    }
}
