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
                if(PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PlayerProperty.Money, out object m))
                {
                    return (int)m;
                }

                return 0;
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

        public static int win { get; set; }
        public static int loss { get; set; }
        public static int tie { get; set; }

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

        public static void UpdateProfit(Action callBack = null)
        {

            PlayFabClientAPI.GetUserReadOnlyData(new GetUserDataRequest() {
                Keys = new[] { "win", "loss", "tie" }.ToList()
            }, userDataResult =>
            {
                if (userDataResult.Data.TryGetValue("win", out var winObj))
                {
                    PlayerManage.win = int.Parse(winObj.Value);
                }

                if (userDataResult.Data.TryGetValue("loss", out var lossObj))
                {
                    PlayerManage.loss = int.Parse(lossObj.Value);
                }

                if (userDataResult.Data.TryGetValue("tie", out var tieObj))
                {
                    PlayerManage.tie = int.Parse(tieObj.Value);
                }



                callBack?.Invoke();
            }, (errResult) => {
                Debug.LogError("Get UserReadOnlyData fail.");
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
