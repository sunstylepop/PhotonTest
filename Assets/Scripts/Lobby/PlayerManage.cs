using ExitGames.Client.Photon;
using Photon.Pun;
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
        //public static void SetWallet(int w)
        //{
        //    Hashtable props = new Hashtable
        //    {
        //        {PlayerProperty.Money, w}
        //    };
        //    PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        //}

        //public static int GetWallet()
        //{
        //    return (int)PhotonNetwork.LocalPlayer.CustomProperties[PlayerProperty.Money];
        //}
    }
}
