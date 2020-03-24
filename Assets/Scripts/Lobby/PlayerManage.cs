using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
