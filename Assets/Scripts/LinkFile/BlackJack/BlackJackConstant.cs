
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class BlackJackConfig
    {
        public static Dictionary<string, RoomFilterType> roomFilter = new Dictionary<string, RoomFilterType>()
        {
            { nameof(BlackJackRoom.ID), RoomFilterType.C0 },
            { nameof(BlackJackRoom.GameCode), RoomFilterType.C1 },
        };
    }

    public enum RoomLevel
    {
        Low = 0,
        Mid,
        High
    }


    enum BlackJackClientEvent
    {
        GetCard = 0,
        Pass
    }

    enum BlackJackServerEvent
    {
        Start = 0,
        PlayerInfo,
        SpecialType,
        Deal,
        PersonalRound,
        GiveCard,
        Pass,
        ShowBankCard,
        Settle,
        ReJoin
    }

    enum BlackJackServerState
    {
        Wait,
        PersonalRound,
        BankerRound,
        Settlement
    }

    public enum BlackJackCardType
    {
        None,
        OverTwentyOne,
        BlackJack,
        FiveCard
    }
}
