using Assets.Scripts;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using ExitGames.Client.Photon;
using Assets.Scripts.Game.BlackJack.Model;
using Assets.Scripts.Lobby;
using System;
using System.Linq;
using System.Collections.Generic;
using Facebook.Unity;

public class MainPanel : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [Header("Login Panel")]
    public GameObject LoginPanelObj;

    [Header("Selection Panel")]
    public GameObject SelectionPanelObj;

    [Header("Freind Panel")]
    public GameObject FriendPanelObj;

    [Header("Bag Panel")]
    public GameObject BagPanelObj;

    [Header("Store Panel")]
    public GameObject StorePanelObj;

    [Header("Statistics Panel")]
    public GameObject StatisticsPanelObj;

    [Header("Queue Room Panel")]
    public GameObject QueueRoomPanelObj;

    [Header("Game Panel")]
    public GameObject GamePanelObj;

    [Header("Modal Panel")]
    public GameObject ModalObj;


    private Dictionary<SysPanel, GameObject> _sysPanel = new Dictionary<SysPanel, GameObject>();
    private RoomLevel SelectedRoomLevel { get; set; }
    private TypedLobby SqlLobby = new TypedLobby("myLobby", LobbyType.SqlLobby);


    #region UNITY

    public void Awake()
    {
        FB.Init(()=> {
            //if(AccessToken.CurrentAccessToken != null)
            //{
            //    LoginMsg.text = "already login";
            //}
        });

        _sysPanel.Add(SysPanel.LoginPanel, LoginPanelObj);
        _sysPanel.Add(SysPanel.SelectionPanel, SelectionPanelObj);
        _sysPanel.Add(SysPanel.QueueRoomPanel, QueueRoomPanelObj);
        _sysPanel.Add(SysPanel.GamePanel, GamePanelObj);
        _sysPanel.Add(SysPanel.StatisticsPanel, StatisticsPanelObj);
        _sysPanel.Add(SysPanel.FriendPanel, FriendPanelObj);
        _sysPanel.Add(SysPanel.BagPanel, BagPanelObj);
        _sysPanel.Add(SysPanel.StorePanel, StorePanelObj);

        ModalHelper.ModalInit(ModalObj);
    }

#endregion

#region PUN CALLBACKS

    public override void OnDisconnected(DisconnectCause cause)
    {
        SetActivePanel(SysPanel.LoginPanel);

        this.LoginPanelObj.GetComponent<LoginPanel>().ResetLoginMsg();
    }

    public override void OnConnectedToMaster()
    {
        PlayFabClientAPI.GetUserReadOnlyData(new GetUserDataRequest()
        {
            PlayFabId = PhotonNetwork.LocalPlayer.UserId,
            Keys = new[] { "LastGameRoom" }.ToList()
        }, (userDataResult) => {
            //判斷是否有遊戲尚未結束、未結束連回該遊戲房間
            var haveObj = userDataResult.Data.TryGetValue("LastGameRoom", out var userDataRecord);
            if(haveObj && !string.IsNullOrEmpty(userDataRecord.Value))
            {
                PhotonNetwork.RejoinRoom(userDataRecord.Value);
            }
            else
            {
                ConnectToLobby();
            }
        }, (errResult) => {
            ModalHelper.WarningMessage("Get UserDataRequest fail. at OnConnectedToMaster.");
        });
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        SetActivePanel(SysPanel.SelectionPanel);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        ConnectToLobby();
        SetActivePanel(SysPanel.SelectionPanel);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        string roomName = "Room " + UnityEngine.Random.Range(1000, 10000);

        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 4,
            CustomRoomProperties = RoomManage.GetRoomProperty(SelectedRoomLevel),
            CustomRoomPropertiesForLobby = RoomManage.GetRoomPropertiesForLobby(),
            PlayerTtl = -1, //永久等待玩家重新連入
            Plugins = new string[] { "BlackJackPlugin" }
        };

        PhotonNetwork.CreateRoom(roomName, options, SqlLobby);
    }

    public override void OnJoinedRoom()
    {
        SetActivePanel(SysPanel.QueueRoomPanel);
    }

    public override void OnLeftRoom()
    {
        SetActivePanel(SysPanel.SelectionPanel);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        QueueRoomPanelObj.GetComponent<QueueRoomPanel>().SetCharacterInQueueRoom();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        QueueRoomPanelObj.GetComponent<QueueRoomPanel>().SetCharacterInQueueRoom();
    }

    public override void OnFriendListUpdate(List<Photon.Realtime.FriendInfo> friendList)
    {
        if (FriendPanelObj.activeInHierarchy)
        {
            var _freindPanel = FriendPanelObj.GetComponent<FreindPanel>();
            _freindPanel.SetOnlineState(friendList);
        }
    }

    /// <summary> IOnEventCallback Event </summary>
    public void OnEvent(EventData photonEvent)
    {
        if (new[] { BlackJackServerEvent.Start, BlackJackServerEvent.ReJoin }.Contains((BlackJackServerEvent)photonEvent.Code))
        {
            SetActivePanel(SysPanel.GamePanel);
        }

        if (GamePanelObj.activeInHierarchy)
        {
            var _gamePanel = GamePanelObj.GetComponent<GamePanel>();
            _gamePanel.Listens(photonEvent);
        }
    }

#endregion



    public void SetActivePanel(SysPanel activePanel)
    {
        foreach (var obj in _sysPanel.Values)
        {
            if (obj != null) obj.SetActive(false);
        }

        if(_sysPanel.TryGetValue(activePanel, out GameObject _obj) && _obj != null)
        {
            _obj.SetActive(true);

            var _panel = _obj.GetComponent<IPanel>();
            _panel?.Init();
        }
    }

    private void ConnectToLobby()
    {
        SetActivePanel(SysPanel.SelectionPanel);


        if (GamePanel.rePlay)
        {
            GamePanel.rePlay = false;
            JoinGameRoom(SelectedRoomLevel);
        }
        else if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby(SqlLobby);
    }

    public void JoinGameRoom(RoomLevel level)
    {
        SelectedRoomLevel = level;

        if (!RoomManage.CanOpenRoom(level, PlayerManage.Wallet))
        {
            ModalHelper.WarningMessage("You Don't have enough money.");
            return;
        }

        var sqlFilter = RoomManage.GetSqlLobbyFilter(level, PlayerManage.Wallet);

        PhotonNetwork.JoinRandomRoom(null, 0, MatchmakingMode.FillRoom, SqlLobby, sqlFilter);

    }

}
