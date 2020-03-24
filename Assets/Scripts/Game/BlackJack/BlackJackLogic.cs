using ExitGames.Client.Photon;
using LitJson;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.BlackJack
{
    class BlackJackLogic
    {
        private const string p0HolderName = "P0Holder";
        private const string p1HolderName = "P1Holder";
        private const string p2HolderName = "P2Holder";
        private const string timerHolderName = "Timer";
        private const string bankerHolderName = "BankerHolder";

        public Action GameStartAct;
        public Action GameEndAct;

        private Dictionary<int, PlayerInfo> InGamePlayer { get; set; }


        private int MyLocation = 0;
        private PlayerInfo MyPlayerInfo { get; set; }

        Transform _gameTransform;

        public BlackJackLogic(Transform transform)
        {
            _gameTransform = transform.Find("GamePanel");
        }

        public void Listens(EventData photonEvent)
        {
            switch ((BlackJackServerEvent)photonEvent.Code)
            {
                case BlackJackServerEvent.Start:
                    Start();
                    break;
                case BlackJackServerEvent.PlayerInfo:
                    SetPlayerInfo(ParserModel<List<PlayerInfo>>(photonEvent.Parameters));
                    break;
                case BlackJackServerEvent.Deal:
                    SetPlayerCardAndShow(ParserModel<int[]>(photonEvent.Parameters));
                    break;
                case BlackJackServerEvent.PersonalRound:
                    EnableBtn(ParserModel<PersonalRoundEvent>(photonEvent.Parameters));
                    break;
                case BlackJackServerEvent.GiveCard:
                    AddExtraCard(ParserModel<GetCardEvent>(photonEvent.Parameters));
                    break;
                case BlackJackServerEvent.Pass:
                    DisableBtn(ParserModel<PassEvent>(photonEvent.Parameters));
                    break;
            }
        }
        private void Start()
        {
            GameStartAct?.Invoke();

            ToggleObj("GetCardButton", false);
            ToggleObj("PassButton", false);
            ToggleObj(p0HolderName, false);
            ToggleObj(p1HolderName, false);
            ToggleObj(p2HolderName, false);
            ToggleObj(bankerHolderName, false);
            ToggleObj(timerHolderName, false);
        }

        private void ToggleObj(string name, bool toggle)
        {
            var obj = _gameTransform.Find(name);

            if(obj != null)
                obj.gameObject.SetActive(toggle);
        }

        private void End()
        {
            GameEndAct?.Invoke();
        }












        private void SetPlayerInfo(List<PlayerInfo> playerInfoList)
        {
            InGamePlayer = playerInfoList.ToDictionary(k => k.Location, v => v);

            var _Players = InGamePlayer.Values.ToList();
            MyLocation = _Players.FirstOrDefault(x => x.ActorNr == PhotonNetwork.LocalPlayer.ActorNumber).Location;
            MyPlayerInfo = _Players[MyLocation];
        }

        private void SetPlayerCardAndShow(int[] baseCards)
        {
            MyPlayerInfo.BaseCards = baseCards;

            foreach (var _p in InGamePlayer)
            {
                showCard(_p.Value);
            }
        }

        private void showCard(PlayerInfo info)
        {
            var newLocation = (info.Location + (3 - MyLocation)) % 3;

            var baseCards = info.BaseCards == null || info.BaseCards.Length == 0 ? "? ?" : string.Join(" ", ConvertCardPoint(info.BaseCards));
            var extraCards = info.ExtraCards == null || info.ExtraCards.Length == 0 ? "" : string.Join(" ", ConvertCardPoint(info.ExtraCards));
            var Txt = $"{baseCards} {extraCards}";

            switch (newLocation)
            {
                case 0:
                    WriteTxt(p0HolderName, Txt);
                    break;
                case 1:
                    WriteTxt(p1HolderName, Txt);
                    break;
                case 2:
                    WriteTxt(p2HolderName, Txt);
                    break;
            }
        }

        private int[] ConvertCardPoint(int[] card)
        {
            return card.Select(x => (x % 13) + 1).ToArray();
        }

        private void WriteTxt(string objName, string val)
        {
            var qpp = _gameTransform.Find("GamePanel").Find(objName).GetComponent<Text>();
            qpp.text = val;
        }

        private void AddExtraCard(GetCardEvent e)
        {
            InGamePlayer[e.Location].ExtraCards = e.ExtraCard;

            showCard(InGamePlayer[e.Location]);
        }

        private void EnableBtn(PersonalRoundEvent e)
        {
            if (MyLocation != e.Location) return;

            var btn1 = _gameTransform.Find("GamePanel").Find("GetCardButton").GetComponent<Button>();
            var btn2 = _gameTransform.Find("GamePanel").Find("PassButton").GetComponent<Button>();

            BlackJackTimer timer = _gameTransform.Find("GamePanel").Find("Timer").GetComponent<BlackJackTimer>();
            timer.StartCount(e.TimeOut / 1000);

            btn1.gameObject.SetActive(true);
            btn2.gameObject.SetActive(true);


        }

        private void DisableBtn(PassEvent e)
        {
            if (MyLocation != e.Location) return;

            var btn1 = _gameTransform.Find("GamePanel").Find("GetCardButton").GetComponent<Button>();
            var btn2 = _gameTransform.Find("GamePanel").Find("PassButton").GetComponent<Button>();

            var txt = _gameTransform.Find("GamePanel").Find("Timer").GetComponent<Text>();

            btn1.gameObject.SetActive(false);
            btn2.gameObject.SetActive(false);
            txt.gameObject.SetActive(false);

        }

        private T ParserModel<T>(Dictionary<byte, object> playerDic)
        {
            var jsonData = (string)playerDic.Values.First();
            return JsonMapper.ToObject<T>(jsonData);
        }
    }
}
