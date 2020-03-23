using ExitGames.Client.Photon;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.BlackJack.Model
{
    class RoomManage
    {
        public static Hashtable GetRoomProperty(RoomLevel level)
        {
            Hashtable props = null;
            switch (level)
            {
                case RoomLevel.High:
                    props = BlackJackRoomProperty.HighRoom.StripToStringKeys();
                    break;
                case RoomLevel.Mid:
                    props = BlackJackRoomProperty.MidRoom.StripToStringKeys();
                    break;
                case RoomLevel.Low:
                    props = BlackJackRoomProperty.LowRoom.StripToStringKeys();
                    break;
            }

            return props;
        }

        public static string[] GetRoomPropertiesForLobby()
        {
            return new string[] { BlackJackRoomProperty.LEVEL, BlackJackRoomProperty.MIN_COST };
        }

        public static string GetSqlLobbyFilter(RoomLevel level, int cost)
        {
            return $"{BlackJackRoomProperty.LEVEL} = {(int)level} AND {BlackJackRoomProperty.MIN_COST} <= {cost}";
        }

        public static bool CanOpenRoom(RoomLevel level, int UserCost)
        {
            var props = GetRoomProperty(level);
            return (int)props[BlackJackRoomProperty.MIN_COST] <= UserCost;
        }
    }
}
