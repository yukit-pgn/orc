using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Photon.Pun;
using Main.Model.Battle;
using Main.View.Battle;
using Main.Service;
using Main.Data;
using Main.Data.Battle;

namespace Main.Presenter.Battle
{
    public class BattlePresenter : MonoBehaviour
    {
        [SerializeField] GameObject card_Prefab;
        [SerializeField] Transform myDeckParent;
        [SerializeField] Transform enemyDeckParent;
        [SerializeField] CardExplanation cardExplanation;
        [SerializeField] BattleUIView uiView;

        [SerializeField] Vector3 myDeckPosition;
        [SerializeField] Vector3 enemyDeckPosition;
        [SerializeField] Vector3 myHandPosition;
        [SerializeField] Vector3 enemyHandPosition;
        [SerializeField] float myHandPositionRange;
        [SerializeField] float enemyHandPositionRange;
        [SerializeField] Vector3 myCardOpenPosition;
        [SerializeField] Vector3 enemyCardOpenPosition;
        [SerializeField] Vector3 myCardGraveyardPosition;
        [SerializeField] Vector3 enemyCardGraveyardPosition;

        PlayerDataService playerDataService;
        EnemyDataService enemyDataService;
        CardDataService cardDataService;
        BattleModel myBattleModel;
        BattleModel enemyBattleModel;

        BattlePhotonView photonView;

        List<Card> myDeck = new List<Card>();
        List<Card> myHand = new List<Card>();
        List<Card> enemyDeck = new List<Card>();
        List<Card> enemyHand = new List<Card>();
        List<Card> myGraveyard = new List<Card>();
        List<Card> enemyGraveyard = new List<Card>();

        bool isMyTurn;

        /// <summary>
        /// 初期設定
        /// </summary>
        async void Start()
        {
            playerDataService = PlayerDataService.Instance;
            enemyDataService = EnemyDataService.Instance;
            cardDataService = CardDataService.Instance;

            await SetupPhoton();

            SetUpModels();

            await SetUpViews();

            Bind();

            SetEvents();

            SetUpdateEvents();

            Play();
        }

        async UniTask SetupPhoton()
        {
            // オフラインモード(仮)
            // PhotonNetwork.OfflineMode = true;
            // PhotonNetwork.JoinOrCreateRoom("Room", new Photon.Realtime.RoomOptions(), Photon.Realtime.TypedLobby.Default);
            // await UniTask.WaitUntil(() => PhotonNetwork.CurrentRoom != null);
            Debug.Log("PhotonNetwork.OfflineMode" + PhotonNetwork.OfflineMode);

            PhotonNetwork.IsMessageQueueRunning = true;

            // CardDataの登録
            if (!CardData.IsRegistered)
            {                    
                CardData.Register();
            }

            // CustomPropertiesの初期化
            PhotonCustomPropertiesService.InitBattleProperties();

            // photonViewのセットアップ
            if(PhotonNetwork.IsMasterClient)
            {
                var go = PhotonNetwork.Instantiate("PhotonPrefab/BattlePhotonView", Vector3.zero, Quaternion.identity);
                photonView = go.GetComponent<BattlePhotonView>();
            }
            else
            {
                await UniTask.WaitUntil(() =>
                {
                    photonView = GameObject.FindWithTag("PhotonView")?.GetComponent<BattlePhotonView>();
                    return photonView != null;
                });
            }

            // photonViewの監視
            photonView.OnStartTurn.Subscribe(StartTurn).AddTo(this);
            photonView.OnChangeTurn.Subscribe(ChangeTurn).AddTo(this);
            photonView.OnEnemyDrawCard.Subscribe(EnemyDrawCard).AddTo(this);
            photonView.OnUseCard.Subscribe(tpl => UseCard(tpl.Item1, tpl.Item2)).AddTo(this);
            photonView.OnAddCard.Subscribe(tpl => AddCard(tpl.Item1, tpl.Item2, tpl.Item3).Forget()).AddTo(this);
        }

        /// <summary>
        /// Modelの設定
        /// </summary>
        void SetUpModels()
        {
            myBattleModel = new BattleModel();
            enemyBattleModel = new BattleModel();
        }


        /// <summary>
        /// Viewの設定
        /// </summary>
        async UniTask SetUpViews()
        {
            // cardExplanationのセットアップ
            cardExplanation.Setup();

            // UIViewのセットアップ
            uiView.Setup();
            
            // 自分のデッキを読み込み
            await RefillDeck(true);

            if (PhotonNetwork.OfflineMode)
            {                
                // 相手のデッキを読み込み
                await RefillDeck(false);
            }
        }

        /// <summary>
        /// Modelの監視
        /// </summary>
        void Bind()
        {
            myBattleModel.GetHP().Subscribe(uiView.SetMyHP).AddTo(this);
            myBattleModel.GetMP().Subscribe(v => uiView.SetMyMP(v, myBattleModel.MaxMP)).AddTo(this);
            enemyBattleModel.GetHP().Subscribe(uiView.SetEnemyHP).AddTo(this);
            enemyBattleModel.GetMP().Subscribe(v => uiView.SetEnemyMP(v, enemyBattleModel.MaxMP)).AddTo(this);
        }

        /// <summary>
        /// Viewのイベントの設定
        /// </summary>
        void SetEvents()
        {
            // UIViewの監視
            uiView.OnClickAsObservable().Subscribe(type =>
            {
                switch (type)
                {
                    case ButtonType.TurnEnd:
                        if (isMyTurn)
                        {
                            Debug.Log("Button ChangeTurn");
                            uiView.SetButtonActive(ButtonType.TurnEnd, false);
                            photonView.ChangeTurn(false);
                        }
                        break;
                }
            })
            .AddTo(this);
        }

        /// <summary>
        /// Update, FixedUpdateのイベントの設定
        /// </summary>
        void SetUpdateEvents()
        {
        }

        /// <summary>
        /// プレイ開始の流れ
        /// </summary>
        async void Play()
        {
            await UniTask.Delay(1000);
            // 手札を補充
            if (PhotonNetwork.OfflineMode)
            {                    
                await UniTask.WhenAll(DrawCard(true, 4), DrawCard(false, 4));
            }
            else
            {
                await DrawCard(true, 4);
            }
            // 先攻決め
            isMyTurn = PhotonCustomPropertiesService.IsFirstPlayer;

            if (PhotonNetwork.IsMasterClient)
            {
                photonView.StartTurn(true);
            }
        }

        /// <summary>
        /// ターン開始
        /// </summary>
        async void StartTurn(bool isFirst)
        {
            var battleModel = isMyTurn ? myBattleModel : enemyBattleModel;
            battleModel.TurnStart();

            if (!isFirst)
            {
                // 先攻1ターン目でなければカードをひく
                await DrawCard(isMyTurn, 1);
            }

            if (isMyTurn)
            {
                // 自分のターンであればカードを選択可能にする
                SetMyHandSelectable(true);
                // ターンエンドボタンを有効化
                uiView.SetButtonActive(ButtonType.TurnEnd, true);
            }
            else if (PhotonNetwork.OfflineMode)
            {
                AI();
            }
        }

        /// <summary>
        /// ターン切り替え
        /// </summary>
        void ChangeTurn(bool isMe)
        {
            Debug.Log("Change Turn");
            isMyTurn = isMe;
            // カードを選択不能にする
            SetMyHandSelectable(false);

            // 次のターンを開始する
            StartTurn(false);
        }

        /// <summary>
        /// デッキを補充する
        /// </summary>
        async UniTask RefillDeck(bool isMe)
        {
            var deckData = isMe ? playerDataService.GetCurrentDeckData() : enemyDataService.GetDeckData();
            var pos = isMe ? myDeckPosition : enemyDeckPosition;
            var deckParent = isMe ? myDeckParent : enemyDeckParent;
            var deck = isMe ? myDeck : enemyDeck;
            var battleModel = isMe ? myBattleModel : enemyBattleModel;

            // var cardDataList = new List<CardData>();
            // deckData.cardList.ForEach(c => cardDataList.Add(c));
            // // シャッフル
            // cardDataList = cardDataList.OrderBy(c => Guid.NewGuid()).ToList();
            var cardDataList = deckData.cardList.OrderBy(c => Guid.NewGuid()).ToList();
            // 山札を生成
            foreach (var cardData in cardDataList)
            {
                cardData.cost = await cardDataService.GetCardCost(cardData); // コストの修正
                var cardGO = Instantiate(card_Prefab, pos, Quaternion.identity, deckParent);
                var card = cardGO.GetComponent<Card>();
                await card.Setup(battleModel.CardIDGenerator(), cardData);
                if (isMe)
                {
                    SetMyCardEvents(card);
                }
                deck.Add(card);
            }
        }

        async void EnemyDrawCard(List<(int, CardData)> cardInfos)
        {
            var cards = new List<Card>();
            foreach(var info in cardInfos)
            {
                var cardGO = Instantiate(card_Prefab, enemyDeckPosition, Quaternion.identity, enemyDeckParent);
                var card = cardGO.GetComponent<Card>();
                await card.Setup(info.Item1, info.Item2);
                cards.Add(card);
            }

            await DrawCard(false, cards);
        }

        /// <summary>
        /// デッキからカードをドローする
        /// </summary>
        async UniTask DrawCard(bool isMe, int count)
        {
            if (!isMe && !PhotonNetwork.OfflineMode) { return; }
            if (count <= 0) { return; }

            var deck = isMe ? myDeck : enemyDeck;
            if (deck == null) { return; }

            if (deck.Count < count)
            {
                await RefillDeck(isMe);
            }

            var cards = deck.GetRange(0, count);
            deck.RemoveRange(0, count);

            if (isMe && !PhotonNetwork.OfflineMode)
            {
                photonView.EnemyDrawCard(cards.Select(c => c.CardID).ToArray(), cards.Select(c => c.CardData).ToArray());
            }

            await DrawCard(isMe, cards);
        }

        /// <summary>
        /// デッキからカードをドローする
        /// </summary>
        async UniTask DrawCard(bool isMe, List<Card> cards)
        {
            if (cards == null) { return; }

            var hand = isMe ? myHand : enemyHand;
            var pos = isMe ? myCardOpenPosition : enemyCardOpenPosition;
            if (hand == null) { return; }

            var tasks = new List<UniTask>();
            for (int i = 0; i < cards.Count; i++)
            {
                // var task1 = cards[i].TurnOver(true, 0.5f);
                // var task2 = cards[i].Move(new Vector3(-3f + 3f * i, 0f, 0f), 0.5f);

                // 自分のカードであれば表に返す
                if (isMe) tasks.Add(cards[i].TurnOver(true, 0.5f));
                tasks.Add(cards[i].Move(pos + Vector3.right * (-1.5f * (cards.Count - 1) + 3f * i), 0.5f));

                await UniTask.Delay(250);
            }
            await UniTask.WhenAll(tasks);

            await UniTask.Delay(1000);

            hand.AddRange(cards);

            await ArrangeHand(isMe);
        }

        /// <summary>
        /// 手札の位置と描画順を調整する
        /// </summary>
        async UniTask ArrangeHand(bool isMe)
        {
            var hand = isMe ? myHand : enemyHand;
            var pos = isMe ? myHandPosition : enemyHandPosition;
            var range = isMe ? myHandPositionRange : enemyHandPositionRange;
            var count = hand.Count;
            var tasks = new List<UniTask>();
            for (int i = 0; i < count; i++)
            {
                if (count == 1)
                {
                    tasks.Add(hand[i].Move(pos, 0.3f));
                }
                else if (count <= 3)
                {
                    tasks.Add(hand[i].Move(pos + new Vector3(range * (i - 0.5f * (count - 1)) / 2, 0, -0.01f * count), 0.3f));
                }
                else
                {
                    tasks.Add(hand[i].Move(pos + new Vector3(range * ((float)i / (count - 1) - 0.5f), 0, -0.01f * count), 0.3f));
                }
                hand[i].SetSortingOrder(i);
            }
            await UniTask.WhenAll(tasks);
        }

        /// <summary>
        /// 手札のカードを使用可能・不可能にする
        /// </summary>
        void SetMyHandSelectable(bool value)
        {
            myHand.ForEach(c => c.Selectable = value);
        }

        /// <summary>
        /// 自分のCardのイベント設定
        /// </summary>
        void SetMyCardEvents(Card c)
        {
            c.OnLongTapAsObservable().Subscribe(data =>
            {
                cardExplanation.Show(data);
            })
            .AddTo(this);

            c.OnDragAsObservable().Subscribe(_ =>
            {
                cardExplanation.Close();
            })
            .AddTo(this);

            c.OnReleaseAsObservable().Subscribe(tpl =>
            {
                cardExplanation.ResetCanShow();

                if (tpl.Item2.y > 0f && tpl.Item1.CardData.cost <= myBattleModel.GetMP().Value && CheckCondition(true, tpl.Item1.CardData.conditionID))
                {
                    SetMyHandSelectable(false);
                    photonView.UseCard(true, tpl.Item1.CardID);
                }
                else
                {
                    ArrangeHand(true).Forget();
                }
            })
            .AddTo(this);
        }

        /// <summary>
        /// カードの条件を満たしているか確認する
        /// </summary>
        /// <param name="conditionID"></param>
        /// <returns></returns>
        bool CheckCondition(bool isMe, int conditionID)
        {
            var attackerModel = isMe ? myBattleModel : enemyBattleModel;
            var defenderModel = isMe ? enemyBattleModel : myBattleModel;
            var attackerHand = isMe ? myHand : enemyHand;
            var defenderHand = isMe ? enemyHand : myHand;

            switch (conditionID)
            {
                case 0:
                    return true;
                case 1:
                    return attackerModel.GetHP().Value > 10;
                case 2:
                    return attackerModel.GetHP().Value <= 50;
                case 3:
                    return attackerModel.GetHP().Value >= 50;
                case 4:
                    return attackerHand.Count >= 2;
                case 5:
                    return attackerHand.Count == 1;
                case 6:
                    return defenderHand.Count <= 2;
                default:
                    return false;
            }
        }

        /// <summary>
        /// カードを使用する
        /// </summary>
        async void UseCard(bool isMe, int cardID)
        {
            var attackerModel = isMe ? myBattleModel : enemyBattleModel;
            // var defenderModel = isMe ? enemyBattleModel : myBattleModel;
            var openPos = isMe ? myCardOpenPosition : enemyCardOpenPosition;
            var graveyardPos = isMe ? myCardGraveyardPosition : enemyCardGraveyardPosition;
            var hand = isMe ? myHand : enemyHand;
            var graveyard = isMe ? myGraveyard : enemyGraveyard;
            if (attackerModel == null || hand == null || graveyard == null) { return; }

            var card = hand.Find(c => c.CardID == cardID);
            if (card == null)
            {
                Debug.LogError("指定のIDを持つカードが見つかりませんでした");
                return;
            }
            var cardData = card.CardData;

            // ターン中使用カード数を加算
            attackerModel.IncreaseUsedHandCount();

            // 手札から削除し墓地に加える
            hand.Remove(card);
            graveyard.Add(card);

            // 手札の位置を調整
            ArrangeHand(isMe).Forget();

            // カードを表示
            card.Move(openPos, 0.3f).Forget();
            if (!isMe)
            {
                // 相手のカードであれば表に返す
                card.TurnOver(true, 0.3f).Forget();
            }
            await UniTask.Delay(800);

            // カードを墓地に
            card.Move(graveyardPos, 0.3f).Forget();

            // MPを消費
            attackerModel.UseMp(cardData.cost);

            // 条件を処理
            await ProcessCondition(isMe, cardData.conditionID);

            // 効果1を処理する
            await ProcessEffect(isMe, cardData.effect1ID);

            // 効果2を処理する
            await ProcessEffect(isMe, cardData.effect2ID);

            // 勝敗を判定する
            var finish = CheckGame();

            // 勝敗が決していればリターン
            if (finish) { return; }

            if (isMyTurn)
            {
                // if (myBattleModel.UsedHandCount )
                SetMyHandSelectable(true);
            }
            else if (PhotonNetwork.OfflineMode)
            {
                AI();
            }
        }

        /// <summary>
        /// 条件を処理する
        /// </summary>
        async UniTask ProcessCondition(bool isMe, int conditionID)
        {
            var attackerModel = isMe ? myBattleModel : enemyBattleModel;
            // var defenderModel = isMe ? enemyBattleModel : myBattleModel;
            switch (conditionID)
            {
                case 1:
                    attackerModel.AddHP(-10);
                    break;
                case 4:
                    await DiscardCard(isMe, 1);
                    break;
            }
        }

        /// <summary>
        /// 効果を処理する
        /// </summary>
        /// <returns></returns>
        async UniTask ProcessEffect(bool isMe, int effectID)
        {
            var attackerModel = isMe ? myBattleModel : enemyBattleModel;
            var defenderModel = isMe ? enemyBattleModel : myBattleModel;
            switch (effectID)
            {
                case 1:
                    var attackInfo = attackerModel.CulcAttack(10, AttributeType.Red);
                    defenderModel.RecieveDamage(attackInfo);
                    break;
                case 2:
                    attackInfo = attackerModel.CulcAttack(20, AttributeType.Red);
                    defenderModel.RecieveDamage(attackInfo);
                    break;
                case 3:
                    attackInfo = attackerModel.CulcAttack(30, AttributeType.Red);
                    defenderModel.RecieveDamage(attackInfo);
                    break;
                case 4:
                    attackInfo = attackerModel.CulcAttack(10, AttributeType.Blue);
                    defenderModel.RecieveDamage(attackInfo);
                    break;
                case 5:
                    attackInfo = attackerModel.CulcAttack(20, AttributeType.Blue);
                    defenderModel.RecieveDamage(attackInfo);
                    break;
                case 6:
                    attackInfo = attackerModel.CulcAttack(5, AttributeType.Blue);
                    defenderModel.RecieveDamage(attackInfo);
                    break;
                case 7:
                    attackInfo = attackerModel.CulcAttack(10, AttributeType.Green);
                    defenderModel.RecieveDamage(attackInfo);
                    break;
                case 8:
                    attackInfo = attackerModel.CulcAttack(20, AttributeType.Green);
                    defenderModel.RecieveDamage(attackInfo);
                    break;
                case 9:
                    attackerModel.AddHP(20);
                    break;
                case 10:
                    attackerModel.AddHP(10);
                    break;
                case 11:
                    await DrawCard(isMe, 1);
                    break;
                case 12:
                    if (isMe || PhotonNetwork.OfflineMode)
                    {                           
                        var cardData = new CardData(0, 0, 9, 0);
                        cardData.cost = await cardDataService.GetCardCost(cardData); 
                        await photonView.AddCard(isMe, attackerModel.CardIDGenerator(), cardData);
                    }
                    break;
                case 13:
                    if (isMe || PhotonNetwork.OfflineMode)
                    {                           
                        var cardData = new CardData(0, 0, 2, 0);
                        cardData.cost = await cardDataService.GetCardCost(cardData); 
                        await photonView.AddCard(isMe, attackerModel.CardIDGenerator(), cardData);
                    }
                    break;
                case 14:
                    await DrawCard(isMe, 2);
                    break;
            }
        }

        /// <summary>
        /// 手札を捨てる
        /// </summary>
        async UniTask DiscardCard(bool isMe, int count)
        {
            // To Do
        }

        /// <summary>
        /// 特定のカードを手札に加える
        /// </summary>
        async UniTask AddCard(bool isMe, int cardID, CardData cardData)
        {
            var hand = isMe ? myHand : enemyHand;
            var openPos = isMe ? myCardOpenPosition : enemyCardOpenPosition;
            var handPos = isMe ? myHandPosition : enemyHandPosition;

            var cardGO = Instantiate(card_Prefab, openPos, Quaternion.identity, myDeckParent);
            var card = cardGO.GetComponent<Card>();
            await card.Setup(cardID, cardData, true);

            await UniTask.Delay(1000);
            
            if (isMe)
            {
                // 自分のであればイベントを設定
                SetMyCardEvents(card);
            }
            else
            {
                // 相手のであれば裏返す
                await card.TurnOver(false, 0.3f);
            }

            hand.Add(card);

            await ArrangeHand(isMe);

            photonView.AddCardSuccess();
        }

        /// <summary>
        /// 勝敗を判定する
        /// </summary>
        bool CheckGame()
        {
            if (myBattleModel.GetHP().Value <= 0)
            {
                Finish(false);
                return true;
            }

            if (enemyBattleModel.GetHP().Value <= 0)
            {
                Finish(true);
                return true;
            }

            return false;
        }

        /// <summary>
        /// ゲーム終了フェーズ
        /// </summary>
        async void Finish(bool winnerIsMe)
        {
            uiView.SetButtonActive(ButtonType.TurnEnd, false);

            if (winnerIsMe)
            {
                await uiView.Win();
            }
            else
            {
                await uiView.Lose();
            }

            await UniTask.Delay(2000);

            SceneService.ChangeScene("MenuScene", 1f);
        }

        /// <summary>
        /// 敵AIの行動
        /// </summary>
        async void AI()
        {
            if (!PhotonNetwork.OfflineMode) { return; }

            await UniTask.Delay(UnityEngine.Random.Range(500, 1000));

            if (enemyHand.Count > 0 && enemyBattleModel.UsedHandCount < 2)
            {
                var card = enemyHand[UnityEngine.Random.Range(0, enemyHand.Count)];

                if(card.CardData.cost <= enemyBattleModel.GetMP().Value && CheckCondition(false, card.CardData.conditionID))
                {
                    photonView.UseCard(false, card.CardID);
                }
                else
                {
                    photonView.ChangeTurn(true);
                }
            }
            else
            {
                photonView.ChangeTurn(true);
            }
        }
    }
}
