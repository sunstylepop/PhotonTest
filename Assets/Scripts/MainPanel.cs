using Assets.Scripts;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Assets.Scripts.Game.BlackJack.Model;
using Assets.Scripts.Lobby;
using System;
using Assets.Scripts.Game.BlackJack;
using System.Linq;
using System.Collections.Generic;
using Facebook.Unity;
using Assets.Scripts.Auth;

public class MainPanel : MonoBehaviourPunCallbacks, IOnEventCallback
{
    private RoomLevel SelectedRoomLevel { get; set; }
    private TypedLobby SqlLobby = new TypedLobby("myLobby", LobbyType.SqlLobby);
    private bool rePlay = false;


    [Header("Login Panel")]
    public GameObject LoginPanel;
    public InputField PlayerNameInput;

    [Header("Selection Panel5")]
    public GameObject SelectionPanel;
    public Text Name;

    [Header("Queue Room Panel")]
    public GameObject QueueRoomPanel;
    public GameObject Character1;
    public GameObject Character2;
    public GameObject Character3;
    public GameObject Character4;

    [Header("Game Panel")]
    public GameObject GamePanel;


#region UNITY

    public void Awake()
    {
        FB.Init(()=> {
            //if(AccessToken.CurrentAccessToken != null)
            //{
            //    LoginMsg.text = "already login";
            //}
        });

        PlayerNameInput.text = "MM2";
    }

#endregion

#region PUN CALLBACKS

    public override void OnDisconnected(DisconnectCause cause)
    {
        this.SetActivePanel(LoginPanel.name);

        Loginer _loginer = LoginPanel.transform.Find("LoginMsg").GetComponent<Loginer>();
        _loginer.SetLoginMsg("Disconnected");
    }

    public override void OnConnectedToMaster()
    {
        PlayFabClientAPI.GetUserReadOnlyData(new GetUserDataRequest()
        {
            PlayFabId = PhotonNetwork.LocalPlayer.UserId,
            Keys = new[] { "LastGameRoom" }.ToList()
        }, (userDataResult) => {
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
            Debug.LogError("Get UserDataRequest fail.");
        });
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        SetActivePanel(SelectionPanel.name);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        ConnectToLobby();
        SetActivePanel(SelectionPanel.name);
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
        SetActivePanel(QueueRoomPanel.name);

        SetCharacterInQueueRoom();
    }

    public override void OnLeftRoom()
    {
        SetCharacterInQueueRoom();

        SetActivePanel(SelectionPanel.name);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        SetCharacterInQueueRoom();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        SetCharacterInQueueRoom();
    }

    /// <summary> IOnEventCallback Event </summary>
    public void OnEvent(EventData photonEvent)
    {
        if (new[] { BlackJackServerEvent.Start, BlackJackServerEvent.ReJoin }.Contains((BlackJackServerEvent)photonEvent.Code))
        {
            SetActivePanel(GamePanel.name);
        }

        if (GamePanel.activeInHierarchy)
        {
            var _gamePanel = GamePanel.GetComponent<GamePanel>();
            _gamePanel.Listens(photonEvent);
        }
    }

#endregion


#region UI CALLBACKS

    private void JoinGameRoom(RoomLevel level)
    {
        SelectedRoomLevel = level;
        
        if(!RoomManage.CanOpenRoom(level, PlayerManage.Wallet))
        {
            Debug.LogError("You Don't have enough money");
            return;
        }

        var sqlFilter = RoomManage.GetSqlLobbyFilter(level, PlayerManage.Wallet);

        PhotonNetwork.JoinRandomRoom(null, 0, MatchmakingMode.FillRoom, SqlLobby, sqlFilter);

    }

    public void OnRePlayClicked()
    {
        rePlay = true;

        PhotonNetwork.LeaveRoom();
    }

    public void OnJoinLowRoomButtonClicked()
    {
        JoinGameRoom(RoomLevel.Low);
    }

    public void OnJoinMidRoomButtonClicked()
    {
        JoinGameRoom(RoomLevel.Mid);
    }

    public void OnJoinHighRoomButtonClicked()
    {
        JoinGameRoom(RoomLevel.High);
    }

    public void OnLeaveQueueRoomButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnLeaveGameClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnLogoutClicked()
    {
        PhotonNetwork.Disconnect();
    }

    public void OnGuestLoginButtonClicked()
    {
        Login<GuestLogin>();
    }

    public void OnFBLoginButtonClicked()
    {
        Login<FaceBookLogin>();
    }

    public void OnLoginButtonClicked()
    {
        string playerName = PlayerNameInput.text;

        Login<NickNameLogin>(playerName);
    }

    private void Login<T>(string nickName = null) where T : IAuth, new()
    {
        Loginer _loginer = LoginPanel.transform.Find("LoginMsg").GetComponent<Loginer>();
        _loginer.Login<T>(nickName);
    }

#endregion

    private void SetActivePanel(string activePanel)
    {
        LoginPanel.SetActive(activePanel.Equals(LoginPanel.name));
        SelectionPanel.SetActive(activePanel.Equals(SelectionPanel.name));
        QueueRoomPanel.SetActive(activePanel.Equals(QueueRoomPanel.name));
        GamePanel.SetActive(activePanel.Equals(GamePanel.name));
    }

    private void SetCharacterInQueueRoom()
    {
        Character1.SetActive(false);
        Character2.SetActive(false);
        Character3.SetActive(false);
        Character4.SetActive(false);

        Action<GameObject, string> SetCharacter = (c, info) => { 
            var _text = c.transform.Find("Text").GetComponent<Text>();
            _text.text = info;

            c.SetActive(true);
        };

        int i = 0;
        foreach(var _p in PhotonNetwork.PlayerList)
        {
            var info = $"{_p.NickName}({(int)_p.CustomProperties[PlayerProperty.Money]})";
            if (i == 0) SetCharacter(Character1, info);
            if (i == 1) SetCharacter(Character2, info);
            if (i == 2) SetCharacter(Character3, info);
            if (i == 3) SetCharacter(Character4, info);

            i++;
        }

    }

    public void ConnectToLobby()
    {
        PlayerManage.UpdateWallet(() => {
            var _name = PhotonNetwork.LocalPlayer.NickName.Length <= 6 ? PhotonNetwork.LocalPlayer.NickName : PhotonNetwork.LocalPlayer.NickName.Substring(0, 6);
            Name.text = $"{_name}({PlayerManage.Wallet})";
        });

        this.SetActivePanel(SelectionPanel.name);


        if (rePlay)
        {
            rePlay = false;
            JoinGameRoom(SelectedRoomLevel);
        }
        else if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby(SqlLobby);
    }

}
