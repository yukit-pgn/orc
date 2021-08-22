using System.Collections;
using System.Collections.Generic;
using System;
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

        Subject<List<(Guid, CardData)>> onEnemyDrawCard = new Subject<List<(Guid, CardData)>>();
        public IObservable<List<(Guid, CardData)>> OnEnemyDrawCard { get {return onEnemyDrawCard; } }

        Subject<(bool, Guid)> onUseCard = new Subject<(bool, Guid)>();
        public IObservable<(bool, Guid)> OnUseCard { get { return onUseCard; } }

        Subject<(bool, Guid, CardData)> onAddCard = new Subject<(bool, Guid, CardData)>();
        public IObservable<(bool, Guid, CardData)> OnAddCard { get { return onAddCard; } }

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
            photonView.RPC(nameof(ChangeTurnRPC), RpcTarget.All, isSender);
        }

        [PunRPC]
        void ChangeTurnRPC(bool isSender, PhotonMessageInfo info)
        {
            bool isMe = (info.Sender == PhotonNetwork.LocalPlayer) ? isSender : !isSender;
            onChangeTurn.OnNext(isMe);
        }

        public void EnemyDrawCard(List<(Guid, CardData)> cardInfos)
        {
            photonView.RPC(nameof(EnemyDrawCardRPC), RpcTarget.Others, cardInfos);
        }

        [PunRPC]
        void EnemyDrawCardRPC(List<(Guid, CardData)> cardInfos)
        {
            onEnemyDrawCard.OnNext(cardInfos);
        }

        public void UseCard(bool isSender, Guid cardID)
        {
            photonView.RPC(nameof(UseCardRPC), RpcTarget.All, isSender, cardID);
        }

        [PunRPC]
        void UseCardRPC(bool isSender, Guid cardID, PhotonMessageInfo info)
        {
            bool isMe = (info.Sender == PhotonNetwork.LocalPlayer) ? isSender : !isSender;
            onUseCard.OnNext((isMe, cardID));
        }

        public async UniTask AddCard(bool isSender, Guid cardID, CardData cardData)
        {
            addCardSuccessFlag = false;
            photonView.RPC(nameof(AddCardRPC), RpcTarget.All, isSender, cardID, cardData);
            await UniTask.WaitUntil(() => addCardSuccessFlag);
        }

        [PunRPC]
        void AddCardRPC(bool isSender, Guid cardID, CardData cardData, PhotonMessageInfo info)
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
