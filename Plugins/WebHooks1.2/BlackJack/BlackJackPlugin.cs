using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Scripts;
using Newtonsoft.Json;

namespace Photon.Hive.Plugin.WebHooks
{
    class BlackJackPlayerData
    {
        public int ActorNr { get; set; }
        public int Location { get; set; }
        public string UserId { get; set; }
        public string Nickname { get; set; }
        public int[] Card { get; set; }
        public int[] ExtraCard { get; set; }

        public bool IsPass { get; set; }
    }

    public class BlackJackPlugin : PluginBase
    {
        private BlackJackServerState curState { get; set; }
        private int[] cardAry { get; set; }
        private object Timer { get; set; }

        private int secPerRound = 5000; //每回合考慮時間

        private int RoundPtr = -1;
        private BlackJackPlayerData Banker { get; set; }
        private Dictionary<string, BlackJackPlayerData> InGamePlayer { get; set; }

        public override string Name
        {
            get
            {
                return "BlackJackPlugin";
            }
        }

        public BlackJackPlugin()
        {
            this.UseStrictMode = true;
            curState = BlackJackServerState.Wait;
        }

        #region Public Methods and Operators

        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            //base.OnRaiseEvent(info);

            switch ((BlackJackClientEvent)info.Request.EvCode)
            {
                case BlackJackClientEvent.GetCard:
                    PlayerGetCard(info.UserId);
                    break;
                case BlackJackClientEvent.Pass:
                    PlayerPass(info.UserId);
                    break;
            }
          
        }

        public override void OnJoin(IJoinGameCallInfo info)
        {
            base.OnJoin(info);
            if (this.PluginHost.GameActors.Count == 2)
            {
                if (curState != BlackJackServerState.Wait) return;

                curState = BlackJackServerState.PersonalRound;
                BroadcastEvent(BlackJackServerEvent.Start, null);

                //1.洗牌
                Random rnd = new Random();
                cardAry = Enumerable.Range(0, 52).OrderBy(n => rnd.Next()).ToArray();

                InGamePlayer = new Dictionary<string, BlackJackPlayerData>();

                int l = 0;
                foreach (var actor in PluginHost.GameActors)
                {
                    var _data = new BlackJackPlayerData();
                    _data.Card = cardAry.Where((item, index) => index >= l * 2 && index < (l * 2) + 2).ToArray();
                    _data.ActorNr = actor.ActorNr;
                    _data.Location = l++;
                    _data.UserId = actor.UserId;
                    _data.Nickname = actor.Nickname;

                    InGamePlayer.Add(_data.UserId, _data);
                }

                cardAry = cardAry.Skip(l * 2).ToArray();

                //2.發送遊戲內玩家資訊
                var _playerInfo = InGamePlayer.Select(v => new PlayerInfo()
                {
                    Location = v.Value.Location,
                    ActorNr = v.Value.ActorNr,
                    Nickname = v.Value.Nickname
                }).ToList();

                BroadcastEvent(BlackJackServerEvent.PlayerInfo, _playerInfo);

                //3.獨自發送牌給各玩家
                foreach (var _p in InGamePlayer)
                {
                    SendEvent(_p.Value.ActorNr, BlackJackServerEvent.Deal, _p.Value.Card);
                }


                //4.依序詢問各玩家是否要牌
                PersonalRoundLoop();
            }


        }

        #endregion











        private void PlayerGetCard(string UserId)
        {
            if (InGamePlayer[UserId].Location != RoundPtr) return;  //不是本次回合

            PluginHost.StopTimer(Timer);

            if (InGamePlayer[UserId].ExtraCard == null) InGamePlayer[UserId].ExtraCard = new int[0];
            InGamePlayer[UserId].ExtraCard = InGamePlayer[UserId].ExtraCard.Union(new int[] { cardAry[0] }).ToArray();
            cardAry = cardAry.Skip(1).ToArray();

            BroadcastEvent(BlackJackServerEvent.GiveCard, new GetCardEvent() { Location = RoundPtr, ExtraCard = InGamePlayer[UserId].ExtraCard });

            PersonalRoundLoop();
        }

        private void PlayerPass(string UserId)
        {
            if (InGamePlayer[UserId].Location != RoundPtr) return;  //不是本次回合

            PluginHost.StopTimer(Timer);
            InGamePlayer[UserId].IsPass = true;

            BroadcastEvent(BlackJackServerEvent.Pass, new PassEvent() { Location = RoundPtr });

            PersonalRoundLoop();
        }

        private void PersonalRoundLoop()
        {
            var _player = InGamePlayer.Values.ToList().FirstOrDefault(x => !x.IsPass);
            RoundPtr = -1;

            if (_player != null)
            {
                RoundPtr = _player.Location;
                BroadcastEvent(BlackJackServerEvent.PersonalRound, new PersonalRoundEvent() { Location = RoundPtr, TimeOut = secPerRound });    //todo: 秒數倒數方式還要再調整
                Timer = PluginHost.CreateOneTimeTimer(()=> { PlayerPass(_player.UserId); }, secPerRound);
            }
            else
            {
                curState = BlackJackServerState.BankerRound;
                BankerRoundLoop();
            }
        }

        private void BankerRoundLoop()
        {

        }

        private void BroadcastEvent(BlackJackServerEvent EvCode, object objData)
        {
            var _json = JsonConvert.SerializeObject(objData);
            var _dic = new Dictionary<byte, object>() { { 0, _json } };
            this.PluginHost.BroadcastEvent(
                    target: ReciverGroup.All,
                    senderActor: 0,
                    targetGroup: 0,
                    data: _dic,
                    evCode: (byte)EvCode,
                    cacheOp: 0);
        }

        private void SendEvent(int Actor, BlackJackServerEvent EvCode, object objData)
        {
            var _json = JsonConvert.SerializeObject(objData);
            var _dic = new Dictionary<byte, object>() { { 0, _json } };
            this.PluginHost.BroadcastEvent(
                    target: ReciverGroup.Others,
                    senderActor: Actor,
                    targetGroup: 0,
                    data: _dic,
                    evCode: (byte)EvCode,
                    cacheOp: 0);
        }
    }
}
