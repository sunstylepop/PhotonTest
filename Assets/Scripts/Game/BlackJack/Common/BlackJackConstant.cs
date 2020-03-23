
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
        Start = 0,
        GetCard,
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
