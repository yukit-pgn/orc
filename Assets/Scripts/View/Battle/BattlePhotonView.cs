using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Main.Data;

namespace Main.View.Battle
{
    [RequireComponent(typeof(PhotonView))]
    public class BattlePhotonView : MonoBehaviourPunCallbacks
    {
        bool addCardSuccessFlag = false;

        Subject<bool> onStartTurn = new Subject<bool>();
        public IObservable<bool> OnStartTurn { get { return onStartTurn; } }

        Subject<bool> onChangeTurn = new Subject<bool>();
        public IObservable<bool> OnChangeTurn { get { return onChangeTurn; } }

        Subject<List<(int, CardData)>> onEnemyDrawCard = new Subject<List<(int, CardData)>>();
        public IObservable<List<(int, CardData)>> OnEnemyDrawCard { get {return onEnemyDrawCard; } }

        Subject<(bool, int)> onUseCard = new Subject<(bool, int)>();
        public IObservable<(bool, int)> OnUseCard { get { return onUseCard; } }

        Subject<(bool, int, CardData)> onAddCard = new Subject<(bool, int, CardData)>();
        public IObservable<(bool, int, CardData)> OnAddCard { get { return onAddCard; } }

        public void StartTurn(bool isFirst)
        {
            photonView.RPC(nameof(StartTurnRPC), RpcTarget.All, isFirst);
        }

        [PunRPC]
        void StartTurnRPC(bool isFirst)
        {
            onStartTurn.OnNext(isFirst);
        }

        public void ChangeTurn(bool isSender)
        {
            Debug.Log("PhotonView ChangeTurn");
            photonView.RPC(nameof(ChangeTurnRPC), RpcTarget.All, isSender);
        }

        [PunRPC]
        void ChangeTurnRPC(bool isSender, PhotonMessageInfo info)
        {
            bool isMe = (info.Sender == PhotonNetwork.LocalPlayer) ? isSender : !isSender;
            onChangeTurn.OnNext(isMe);
        }

        public void EnemyDrawCard(int[] cardIDs, CardData[] cardData)
        {
            photonView.RPC(nameof(EnemyDrawCardRPC), RpcTarget.Others, cardIDs, cardData);
        }

        [PunRPC]
        void EnemyDrawCardRPC(int[] cardIDs, CardData[] cardData)
        {
            var cardInfos = cardIDs.Select((id, i) => (id, cardData[i])).ToList();
            onEnemyDrawCard.OnNext(cardInfos);
        }

        public void UseCard(bool isSender, int cardID)
        {
            photonView.RPC(nameof(UseCardRPC), RpcTarget.All, isSender, cardID);
        }

        [PunRPC]
        void UseCardRPC(bool isSender, int cardID, PhotonMessageInfo info)
        {
            bool isMe = (info.Sender == PhotonNetwork.LocalPlayer) ? isSender : !isSender;
            onUseCard.OnNext((isMe, cardID));
        }

        public async UniTask AddCard(bool isSender, int cardID, CardData cardData)
        {
            addCardSuccessFlag = false;
            photonView.RPC(nameof(AddCardRPC), RpcTarget.All, isSender, cardID, cardData);
            await UniTask.WaitUntil(() => addCardSuccessFlag);
        }

        [PunRPC]
        void AddCardRPC(bool isSender, int cardID, CardData cardData, PhotonMessageInfo info)
        {
            bool isMe = (info.Sender == PhotonNetwork.LocalPlayer) ? isSender : !isSender;
            onAddCard.OnNext((isMe, cardID, cardData));
        }

        public void AddCardSuccess()
        {
            addCardSuccessFlag = true;
        }
    }
}
