using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Lobby
{
    class PlayerManage
    {
        public static int Wallet
        {
            get
            {
                return (int)PhotonNetwork.LocalPlayer.CustomProperties[PlayerProperty.Money];
            }
            set
            {
                Hashtable props = new Hashtable
                {
                    {PlayerProperty.Money, value}
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            }
        }

        public static void UpdateWallet(Action callBack = null)
        {
            //取餘額
            PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), userInventoryResult =>
            {
                if (userInventoryResult.VirtualCurrency.TryGetValue("PI", out int _myMoney))
                {
                    PlayerManage.Wallet = _myMoney;
                }
                else
                {
                    Debug.LogError("Not found PI Currency.");
                }

                callBack?.Invoke();
            }, (errResult) => {
                Debug.LogError("Get VirtualCurrency fail.");
            }
            );
        }
    }

    public static class PlayerExtension
    {
        public static Player GetPlayerByActorNr(this Player[] PlayerList, int ActorNr)
        {
            return PlayerList.FirstOrDefault(x => x.ActorNumber == ActorNr);
        }

        public static int GetWallet(this Player Player)
        {
            return (int)Player.CustomProperties[PlayerProperty.Money];
        }
    }
}
