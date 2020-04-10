using Assets.Scripts;
using Assets.Scripts.Lobby;
using ExitGames.Client.Photon;
using LitJson;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{
    private PlayerInfo Banker { get; set; }
    private Dictionary<int, PlayerInfo> InGamePlayer { get; set; }
    private int MyLocation = 0;
    private PlayerInfo MyPlayerInfo { get; set; }
    public static bool rePlay { get; set; }



    public GameObject BankerHolder;
    public GameObject P0Holder;
    public GameObject P1Holder;
    public GameObject P2Holder;
    public GameObject TimerHolder;
    public GameObject ButtonHolder;
    public GameObject MenuHolder;



    // Update is called once per frame
    void Update()
    {
        //莊家
        BankerHolder.SetActive(true);
        var bankerBaseCards = Banker.BaseCards == null || Banker.BaseCards.Length == 0 ? "? ?" : string.Join(" ", ConvertCardPoint(Banker.BaseCards));
        var bankerExtraCards = Banker.ExtraCards == null || Banker.ExtraCards.Length == 0 ? "" : string.Join(" ", ConvertCardPoint(Banker.ExtraCards));
        BankerHolder.transform.Find("Cards").GetComponent<Text>().text = $"{bankerBaseCards} {bankerExtraCards}";

        BankerHolder.transform.Find("CardTypeText").GetComponent<Text>().text = $"{Banker.CardTypeStr}";
        var bankerProfitStr = Banker.Profit != 0 ? Banker.Profit.ToString() : "";
        BankerHolder.transform.Find("ProfitText").GetComponent<Text>().text = $"{bankerProfitStr}";


        //玩家
        foreach (var _p in InGamePlayer)
        {
            var newLocation = (_p.Value.Location + (3 - MyLocation)) % 3;

            GameObject gameObject = null;
            switch (newLocation)
            {
                case 0:
                    gameObject = P0Holder;
                    break;
                case 1:
                    gameObject = P1Holder;
                    break;
                case 2:
                    gameObject = P2Holder;
                    break;
            }

            if(gameObject != null)
            {
                //設定牌
                var baseCards = _p.Value.BaseCards == null || _p.Value.BaseCards.Length == 0 ? "? ?" : string.Join(" ", ConvertCardPoint(_p.Value.BaseCards));
                var extraCards = _p.Value.ExtraCards == null || _p.Value.ExtraCards.Length == 0 ? "" : string.Join(" ", ConvertCardPoint(_p.Value.ExtraCards));
                gameObject.transform.Find("Cards").GetComponent<Text>().text = $"{baseCards} {extraCards}";

                //設定名子、金錢
                var playerData = PhotonNetwork.PlayerList.GetPlayerByActorNr(_p.Value.ActorNr);
                if(playerData != null)
                {
                    var _name = playerData.NickName.Length <= 6 ? playerData.NickName : playerData.NickName.Substring(0, 6);
                    gameObject.transform.Find("info").GetComponent<Text>().text = $"{_name}({playerData.GetWallet()})";
                }

                //顯示牌型
                gameObject.transform.Find("CardTypeText").GetComponent<Text>().text = $"{_p.Value.CardTypeStr}";

                //顯示輸贏
                var profitStr = _p.Value.Profit != 0 ? _p.Value.Profit.ToString() : "";
                gameObject.transform.Find("ProfitText").GetComponent<Text>().text = $"{profitStr}";

                gameObject.SetActive(true);
            }
        }
    }

    public void Listens(EventData photonEvent)
    {
        switch ((BlackJackServerEvent)photonEvent.Code)
        {
            case BlackJackServerEvent.Start:
                Init();
                break;
            case BlackJackServerEvent.PlayerInfo:
                SetPlayerInfo(ParserModel<List<PlayerInfo>>(photonEvent.Parameters));
                break;
            case BlackJackServerEvent.Deal:
                SetPlayerCardAndShow(ParserModel<int[]>(photonEvent.Parameters));
                break;
            case BlackJackServerEvent.SpecialType:
                SetSpecialTypeCard(ParserModel<List<PlayerInfo>>(photonEvent.Parameters));
                break;
            case BlackJackServerEvent.PersonalRound:
                CountDownRound(ParserModel<PersonalRoundEvent>(photonEvent.Parameters));
                break;
            case BlackJackServerEvent.GiveCard:
                AddExtraCard(ParserModel<GetCardEvent>(photonEvent.Parameters));
                break;
            case BlackJackServerEvent.Pass:
                DisableButtonHolder(ParserModel<PassEvent>(photonEvent.Parameters));
                break;
            case BlackJackServerEvent.ShowBankCard:
                ShowBankCard(ParserModel<BankerCardEvent>(photonEvent.Parameters));
                break;
            case BlackJackServerEvent.Settle:
                SettlementProc(ParserModel<SettlementEvent>(photonEvent.Parameters));
                break;
            case BlackJackServerEvent.ReJoin:
                ReJoin(ParserModel<ReJoinEvent>(photonEvent.Parameters));
                break;
        }
    }





    public void OnLeaveGameClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnGetCardButtonClicked()
    {
        PhotonNetwork.RaiseEvent((byte)BlackJackClientEvent.GetCard, null, null, SendOptions.SendReliable);
    }

    public void OnPassButtonClicked()
    {
        PhotonNetwork.RaiseEvent((byte)BlackJackClientEvent.Pass, null, null, SendOptions.SendReliable);
    }

    public void OnRePlayClicked()
    {
        rePlay = true;

        PhotonNetwork.LeaveRoom();
    }






    private void Init()
    {
        BankerHolder.SetActive(false);
        P0Holder.SetActive(false);
        P1Holder.SetActive(false);
        P2Holder.SetActive(false);
        TimerHolder.SetActive(false);
        ButtonHolder.SetActive(false);
        MenuHolder.SetActive(false);

        Banker = new PlayerInfo();
        InGamePlayer = new Dictionary<int, PlayerInfo>();
    }

    private void SetSpecialTypeCard(List<PlayerInfo> playerInfoList)
    {
        foreach(var _p in playerInfoList)
        {
            InGamePlayer[_p.Location].BaseCards = _p.BaseCards;
            InGamePlayer[_p.Location].CardType = _p.CardType;
        }
    }

    private void SetPlayerInfo(List<PlayerInfo> playerInfoList)
    {
        InGamePlayer = playerInfoList.ToDictionary(k => k.Location, v => v);

        var _Players = InGamePlayer.Values.ToList();
        MyLocation = _Players.FirstOrDefault(x => x.ActorNr == PhotonNetwork.LocalPlayer.ActorNumber).Location;
        MyPlayerInfo = _Players[MyLocation];
    }

    private T ParserModel<T>(Dictionary<byte, object> playerDic)
    {
        var jsonData = (string)playerDic.Values.First();
        return JsonMapper.ToObject<T>(jsonData);
    }

    private int[] ConvertCardPoint(int[] card)
    {
        return card.Select(x => (x % 13) + 1).ToArray();
    }

    private void SetPlayerCardAndShow(int[] baseCards)
    {
        MyPlayerInfo.BaseCards = baseCards;
    }

    private void CountDownRound(PersonalRoundEvent e)
    {
        ButtonHolder.SetActive(MyLocation == e.Location);

        BlackJackTimer timer = TimerHolder.GetComponent<BlackJackTimer>();
        timer.StartCount(e.TimeOut / 1000);
        TimerHolder.SetActive(true);
    }

    private void AddExtraCard(GetCardEvent e)
    {
        InGamePlayer[e.Location].CardType = e.CradType;
        if(e.BaseCards != null) InGamePlayer[e.Location].BaseCards = e.BaseCards;
        InGamePlayer[e.Location].ExtraCards = e.ExtraCard;
    }

    private void DisableButtonHolder(PassEvent e)
    {
        ButtonHolder.SetActive(false);
        TimerHolder.SetActive(false);
    }

    private void ShowBankCard(BankerCardEvent e)
    {
        ButtonHolder.SetActive(false);
        TimerHolder.SetActive(false);

        Banker.CardType = e.CardType;
        Banker.BaseCards = e.BaseCards;
        Banker.ExtraCards = e.ExtraCard;
    }

    private void SettlementProc(SettlementEvent e)
    {
        Banker.Profit = e.Banker.Profit;

        foreach(var _p in e.PlayerList)
        {
            InGamePlayer[_p.Location].Profit = _p.Profit;
        }

        PlayerManage.Wallet += InGamePlayer[MyLocation].Profit;

        MenuHolder.SetActive(true);
    }

    private void ReJoin(ReJoinEvent e)
    {
        Init();
        SetPlayerInfo(e.PlayerList);

        if(e.PersonalRound != null) CountDownRound(e.PersonalRound);
        if(e.Banker != null) ShowBankCard(e.Banker);
        if(e.Settle != null) SettlementProc(e.Settle);
    }

}
