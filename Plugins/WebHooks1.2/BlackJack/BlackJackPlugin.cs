using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Scripts;
using Assets.Scripts.Game.BlackJack.Common;
using Newtonsoft.Json;
using Photon.Hive.Operations;
using PlayFab;
using PlayFab.ServerModels;

namespace Photon.Hive.Plugin.WebHooks
{
    class BlackJackPlayerData
    {
        public int ActorNr { get; set; }
        public int Location { get; set; }
        public string UserId { get; set; }
        public int[] Card { get; set; }
        public int[] ExtraCard { get; set; }

        public bool IsPass { get; set; }

        public int Point { get; set; }
        public BlackJackCardType CardType { get; set; }

        public int Profit { get; set; }
    }

    public class BlackJackPlugin : PluginBase
    {
        private BlackJackServerState curState { get; set; }
        private int[] cardAry { get; set; }
        private object Timer { get; set; }
        private bool IsSettle{ get; set; }

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

            PlayFabSettings.staticSettings.TitleId = "960FC";
            PlayFabSettings.staticSettings.DeveloperSecretKey = "W4A4A6E9XQ4IQJF4BRDPIMR3KQXPCXFUWQXD884GU7RDWD7O4M";   //API Secret Keys
        }

        #region Public Methods and Operators

        public override void OnCloseGame(ICloseGameCallInfo info)
        {
            if (curState != BlackJackServerState.Wait)
            {
                if(!IsSettle) SettlementProc();

                SetUserRoomTag(null);
            }

            info.Continue();
        }

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


            info.Cancel();
        }

        public override void OnJoin(IJoinGameCallInfo info)
        {
            base.OnJoin(info);

            if(InGamePlayer != null && InGamePlayer.ContainsKey(info.UserId))
            {
                var rejoinPlayer = InGamePlayer[info.UserId];

                var rejoinData = new ReJoinEvent()
                {
                    PlayerList = InGamePlayer.Select(v => new PlayerInfo()
                    {
                        Location = v.Value.Location,
                        ActorNr = v.Value.ActorNr,
                        BaseCards = v.Value.UserId.Equals(info.UserId) || v.Value.CardType != BlackJackCardType.None ? v.Value.Card : null,
                        ExtraCards = v.Value.ExtraCard,
                        CardType = v.Value.CardType
                    }).ToList()
                };

                if (curState == BlackJackServerState.PersonalRound)
                {
                    rejoinData.PersonalRound = new PersonalRoundEvent() { Location = RoundPtr, TimeOut = secPerRound };
                }
                
                if(curState == BlackJackServerState.BankerRound || curState == BlackJackServerState.Settlement)
                {
                    rejoinData.Banker = new BankerCardEvent() { BaseCards = Banker.Card, ExtraCard = Banker.ExtraCard, CardType = Banker.CardType };
                }

                if (curState == BlackJackServerState.Settlement)
                {
                    var _settleInfo = new SettlementEvent();
                    _settleInfo.Banker = new PlayerInfo() { Profit = Banker.Profit };
                    _settleInfo.PlayerList = InGamePlayer.Select(v => new PlayerInfo()
                    {
                        Location = v.Value.Location,
                        ActorNr = v.Value.ActorNr,
                        Profit = v.Value.Profit
                    }).ToList();

                    rejoinData.Settle = _settleInfo;
                }

                SendEvent(rejoinPlayer.ActorNr, BlackJackServerEvent.ReJoin, rejoinData);
            }
            else if (this.PluginHost.GameActors.Count == 2)
            {
                if (curState != BlackJackServerState.Wait) return;

                //更改房間屬性
                PluginHost.SetProperties(actorNr: 0, properties: new Hashtable() { 
                    { (byte)GameParameter.IsOpen, false }, 
                    { (byte)GameParameter.IsVisible, false } 
                }, expected: null, broadcast: true);

                curState = BlackJackServerState.PersonalRound;
                BroadcastEvent(BlackJackServerEvent.Start, null);

                //1.洗牌
                Random rnd = new Random();
                cardAry = Enumerable.Range(0, 52).OrderBy(n => rnd.Next()).ToArray();

                InGamePlayer = new Dictionary<string, BlackJackPlayerData>();

                //2. 設置牌給玩家
                int l = 0;
                foreach (var actor in PluginHost.GameActors)
                {
                    var _data = new BlackJackPlayerData();
                    _data.Card = GetCardFromAry(2);
                    _data.ActorNr = actor.ActorNr;
                    _data.Location = l++;
                    _data.UserId = actor.UserId;

                    (_data.Point, _data.CardType) = BlackJackLogic.CalculatResult(_data.Card, _data.ExtraCard);
                    if (_data.CardType != BlackJackCardType.None) _data.IsPass = true;

                    InGamePlayer.Add(_data.UserId, _data);
                }

                SetUserRoomTag(info.Request.GameId);

                //3. 設置牌給莊家
                Banker = new BlackJackPlayerData() { Card = GetCardFromAry(2) };
                (Banker.Point, Banker.CardType) = BlackJackLogic.CalculatResult(Banker.Card, Banker.ExtraCard);

                //4.發送遊戲內玩家資訊
                var _playerInfo = InGamePlayer.Select(v => new PlayerInfo()
                {
                    Location = v.Value.Location,
                    ActorNr = v.Value.ActorNr
                }).ToList();

                BroadcastEvent(BlackJackServerEvent.PlayerInfo, _playerInfo);

                //5.獨自發送牌給各玩家
                foreach (var _p in InGamePlayer)
                {
                    SendEvent(_p.Value.ActorNr, BlackJackServerEvent.Deal, _p.Value.Card);
                }

                //6. 特殊牌型已勝利
                var _specialTypeInfo = InGamePlayer.Where(x=>x.Value.CardType == BlackJackCardType.BlackJack).Select(v => new PlayerInfo()
                {
                    CardType = v.Value.CardType,
                    BaseCards = v.Value.Card,
                    Location = v.Value.Location,
                    ActorNr = v.Value.ActorNr
                }).ToList();
                BroadcastEvent(BlackJackServerEvent.SpecialType, _specialTypeInfo);


                //7.依序詢問各玩家是否要牌
                PersonalRoundLoop();
            }


        }

        public override void BeforeSetProperties(IBeforeSetPropertiesCallInfo info)
        {
            if(info.Request.ActorNumber == 0)
            {
                //禁止更動房間任何屬性
                info.Cancel();
                return;
            }

            info.Continue();
        }

        #endregion








        private int[] GetCardFromAry(int count)
        {
            var _tmp = cardAry.Where((item, index) => index < count).ToArray();
            cardAry = cardAry.Skip(count).ToArray();

            return _tmp;
        }


        private void PlayerGetCard(string UserId)
        {
            var curPlayer = InGamePlayer[UserId];
            if (curPlayer.Location != RoundPtr) return;  //不是本次回合

            PluginHost.StopTimer(Timer);

            if (curPlayer.ExtraCard == null) curPlayer.ExtraCard = new int[0];
            curPlayer.ExtraCard = curPlayer.ExtraCard.Union(GetCardFromAry(1)).ToArray();

            //判斷該玩家遊戲結果
            (curPlayer.Point, curPlayer.CardType) = BlackJackLogic.CalculatResult(curPlayer.Card, curPlayer.ExtraCard);

            if (curPlayer.CardType != BlackJackCardType.None)
            {
                curPlayer.IsPass = true;
            }

            BroadcastEvent(BlackJackServerEvent.GiveCard, new GetCardEvent() { 
                CradType = curPlayer.CardType,
                Location = RoundPtr, 
                BaseCards = curPlayer.CardType != BlackJackCardType.None ? curPlayer.Card : null,
                ExtraCard = curPlayer.ExtraCard 
            });

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
            (Banker.Point, Banker.CardType) = BlackJackLogic.CalculatResult(Banker.Card, Banker.ExtraCard);

            BroadcastEvent(BlackJackServerEvent.ShowBankCard, new BankerCardEvent() { BaseCards = Banker.Card, ExtraCard = Banker.ExtraCard, CardType = Banker.CardType });


            //特殊牌型或者大於16點就不再要牌
            if (InGamePlayer.All(x => x.Value.CardType != BlackJackCardType.None) || Banker.CardType != BlackJackCardType.None || Banker.Point >= 16)
            {
                curState = BlackJackServerState.Settlement;
                PluginHost.CreateOneTimeTimer(SettlementProc, 3000);   //3秒後進入結算
            }
            else
            {
                if (Banker.ExtraCard == null) Banker.ExtraCard = new int[0];
                Banker.ExtraCard = Banker.ExtraCard.Union(GetCardFromAry(1)).ToArray();

                PluginHost.CreateOneTimeTimer(BankerRoundLoop, 1500);   //每1.5秒要一次牌
            }
        }

        private async void SettlementProc()
        {
            var Antes = (int)PluginHost.CustomGameProperties["C1"];

            //計算
            foreach (var _p in InGamePlayer)
            {
                //計算爆掉
                if (_p.Value.CardType == BlackJackCardType.OverTwentyOne)
                {
                    _p.Value.Profit -= Antes;
                    continue;
                }

                //玩家有特殊牌型或莊家爆掉直接給錢
                if (_p.Value.CardType != BlackJackCardType.None || Banker.CardType == BlackJackCardType.OverTwentyOne)
                {
                    _p.Value.Profit = Antes;
                    continue;
                }

                //莊家特殊牌型直接扣玩家錢
                if (Banker.CardType != BlackJackCardType.None)
                {
                    _p.Value.Profit -= Antes;
                    continue;
                }

                //比大小
                if(_p.Value.Point > Banker.Point)
                {
                    _p.Value.Profit = Antes;
                }
                else if (_p.Value.Point < Banker.Point)
                {
                    _p.Value.Profit -= Antes;
                }
            }

            Banker.Profit = InGamePlayer.Values.Sum(x => x.Profit) * -1;


            var _settleInfo = new SettlementEvent();
            _settleInfo.Banker = new PlayerInfo() { Profit = Banker.Profit };
            _settleInfo.PlayerList = InGamePlayer.Select(v => new PlayerInfo()
            {
                Location = v.Value.Location,
                ActorNr = v.Value.ActorNr,
                Profit = v.Value.Profit
            }).ToList();

            var result = await PlayFabServerAPI.ExecuteCloudScriptAsync(new ExecuteCloudScriptServerRequest()
            {
                PlayFabId = InGamePlayer.First().Value.UserId,
                FunctionName = "BlackJackSettle",
                FunctionParameter = InGamePlayer.Select(x => new { PlayFabId = x.Value.UserId, Profit = x.Value.Profit }).ToList()
            });

            if(result.Error != null)
            {
                PluginHost.LogError(result.Error);
            }

            BroadcastEvent(BlackJackServerEvent.Settle, _settleInfo);

            IsSettle = true;
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
                    recieverActors: new List<int>(new[] { Actor }),
                    senderActor: 0,
                    data: _dic,
                    evCode: (byte)EvCode,
                    cacheOp: 0);
        }

        private async void SetUserRoomTag(string roomName)
        {
            var r = await PlayFabServerAPI.ExecuteCloudScriptAsync(new ExecuteCloudScriptServerRequest()
            {
                PlayFabId = InGamePlayer.First().Value.UserId,
                FunctionName = "SetUserRoomTag",
                FunctionParameter = new
                {
                    Players = InGamePlayer.Values.Select(x => x.UserId).ToList(),
                    RoomName = roomName
                }
            });
        }
    }
}
