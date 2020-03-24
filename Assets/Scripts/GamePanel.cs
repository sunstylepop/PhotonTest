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
    private Dictionary<int, PlayerInfo> InGamePlayer { get; set; }
    private int MyLocation = 0;
    private PlayerInfo MyPlayerInfo { get; set; }



    public GameObject BankerHolder;
    public GameObject P0Holder;
    public GameObject P1Holder;
    public GameObject P2Holder;
    public GameObject TimerHolder;
    public GameObject ButtonHolder;



    // Update is called once per frame
    void Update()
    {

        foreach(var _p in InGamePlayer)
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
                    gameObject.transform.Find("info").GetComponent<Text>().text = $"{playerData.NickName}({playerData.GetWallet()})";

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
            case BlackJackServerEvent.PersonalRound:
                CountDownRound(ParserModel<PersonalRoundEvent>(photonEvent.Parameters));
                break;
            case BlackJackServerEvent.GiveCard:
                AddExtraCard(ParserModel<GetCardEvent>(photonEvent.Parameters));
                break;
            case BlackJackServerEvent.Pass:
                DisableButtonHolder(ParserModel<PassEvent>(photonEvent.Parameters));
                break;
        }
    }







    public void OnGetCardButtonClicked()
    {
        PhotonNetwork.RaiseEvent((byte)BlackJackClientEvent.GetCard, null, null, SendOptions.SendReliable);
    }

    public void OnPassButtonClicked()
    {
        PhotonNetwork.RaiseEvent((byte)BlackJackClientEvent.Pass, null, null, SendOptions.SendReliable);
    }








    private void Init()
    {
        BankerHolder.SetActive(false);
        P0Holder.SetActive(false);
        P1Holder.SetActive(false);
        P2Holder.SetActive(false);
        TimerHolder.SetActive(false);
        ButtonHolder.SetActive(false);

        InGamePlayer = new Dictionary<int, PlayerInfo>();
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
        if (MyLocation == e.Location)
        {
            ButtonHolder.SetActive(true);
        }

        BlackJackTimer timer = TimerHolder.GetComponent<BlackJackTimer>();
        timer.StartCount(e.TimeOut / 1000);
        TimerHolder.SetActive(true);
    }

    private void AddExtraCard(GetCardEvent e)
    {
        InGamePlayer[e.Location].ExtraCards = e.ExtraCard;
    }

    private void DisableButtonHolder(PassEvent e)
    {
        ButtonHolder.SetActive(false);
        TimerHolder.SetActive(false);
    }

}
