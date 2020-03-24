﻿
namespace Assets.Scripts
{
    enum RoomLevel
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
        Deal,
        PersonalRound,
        GiveCard,
        Pass,
        ShowBankCard,
        Settle
    }

    enum BlackJackServerState
    {
        Wait,
        PersonalRound,
        BankerRound,
        Settlement
    }
}
