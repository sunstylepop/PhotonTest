
using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class PlayerInfo
    {
        /// <summary>
        /// 遊戲位置
        /// </summary>
        public int Location { get; set; }

        /// <summary>
        /// server端入房遊戲編號
        /// </summary>
        public int ActorNr { get; set; }

        /// <summary>
        /// 暱稱
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// 基本手牌
        /// </summary>
        public int[] BaseCards { get; set; }

        /// <summary>
        /// 額外手牌
        /// </summary>
        public int[] ExtraCards { get; set; }

        public BlackJackCardType CardType { get; set; }

        public int Profit { get; set; }

        public string CardTypeStr { 
            get 
            {
                if (CardType == BlackJackCardType.BlackJack) return "Black Jack";
                if (CardType == BlackJackCardType.FiveCard) return "過五關";
                if (CardType == BlackJackCardType.OverTwentyOne) return "爆!!";
                return "";
            }
        }
    }

    public class PersonalRoundEvent
    {
        public int Location { get; set; }
        public int TimeOut { get; set; }
    }

    public class GetCardEvent
    {
        public BlackJackCardType CradType { get; set; }
        public int Location { get; set; }
        public int[] BaseCards { get; set; }
        public int[] ExtraCard { get; set; }
    }

    public class PassEvent
    {
        public int Location { get; set; }
    }

    public class BankerCardEvent
    {
        public BlackJackCardType CardType { get; set; }
        public int[] BaseCards { get; set; }
        public int[] ExtraCard { get; set; }
    }

    public class SettlementEvent
    {
        public PlayerInfo Banker { get; set; }

        public List<PlayerInfo> PlayerList { get; set; }
    }

    public class ReJoinEvent
    {
        public BankerCardEvent Banker { get; set; }

        public List<PlayerInfo> PlayerList { get; set; }

        public PersonalRoundEvent PersonalRound { get; set; }

        public SettlementEvent Settle { get; set; }
    }
}
