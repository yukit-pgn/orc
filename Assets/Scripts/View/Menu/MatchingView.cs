using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;

namespace Main.View.Menu
{
    public class MatchingView : MonoBehaviourPunCallbacks
    {
        Subject<Unit> onMatch = new Subject<Unit>();
        public IObservable<Unit> OnMatch { get { return onMatch; } }

        public void PlayOffline()
        {
            PhotonNetwork.OfflineMode = true;

            if (PhotonNetwork.CurrentRoom != null)
            {
                onMatch.OnNext(Unit.Default);
            }
        }

        public void PlayOnline()
        {
            PhotonNetwork.OfflineMode = false;
            PhotonNetwork.ConnectUsingSettings();
        }

        /// <summary>
        /// マスターサーバー接続時
        /// </summary>
        public override void OnConnectedToMaster()
        {
            var roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;

            PhotonNetwork.JoinRandomOrCreateRoom(roomOptions:roomOptions);
        }

        /// <summary>
        /// ルーム参加時
        /// </summary>
        public override void OnJoinedRoom()
        {
            WaitOtherPlayer();
        }

        async void WaitOtherPlayer()
        {
            await UniTask.WaitUntil(() => PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers);

            onMatch.OnNext(Unit.Default);
        }
    }
}
