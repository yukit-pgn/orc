using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Photon.Pun;

namespace Main.Service
{
    public static class PhotonCustomPropertiesService
    {
        static readonly string IS_MASTER_CLIENT_FIRST = "IsMasterClientFirst";

        // 先攻か後攻か
        public static bool IsFirstPlayer
        {
            get
            {
                var isMasterClientFirst = PhotonNetwork.CurrentRoom?.CustomProperties[IS_MASTER_CLIENT_FIRST] as bool?;
                if (isMasterClientFirst.HasValue)
                {
                    return isMasterClientFirst == PhotonNetwork.IsMasterClient;
                }
                else
                {
                    Debug.LogError("\"" + IS_MASTER_CLIENT_FIRST + "\"が取得できませんでした");
                    return false;
                }
            }
        }

        /// <summary>
        /// プロパティの初期化
        /// </summary>
        public static void InitBattleProperties()
        {
            var hashtable = new ExitGames.Client.Photon.Hashtable();
            hashtable[IS_MASTER_CLIENT_FIRST] = true;
            PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
        }
    }
}
