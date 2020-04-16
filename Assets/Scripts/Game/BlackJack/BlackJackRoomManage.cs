using ExitGames.Client.Photon;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game
{
    public class BlackJackRoomManage : RoomManageBase<BlackJackRoomManage>
    {
        protected override Dictionary<string, RoomFilterType> roomFilter => BlackJackConfig.roomFilter;

        protected override RoomOptions DefaultRoomOption => new RoomOptions
        {
            MaxPlayers = 4,
            PlayerTtl = -1, //永久等待玩家重新連入
            Plugins = new string[] { "BlackJackPlugin" }
        };
    }
}
