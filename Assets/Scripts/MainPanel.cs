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

public class MainPanel : MonoBehaviourPunCallbacks, IOnEventCallback
{
    private RoomLevel SelectedRoomLevel { get; set; }
    private TypedLobby SqlLobby = new TypedLobby("myLobby", LobbyType.SqlLobby);


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
        //PhotonNetwork.AutomaticallySyncScene = true;

        PlayerNameInput.text = "MM2";
    }

    #endregion

    #region PUN CALLBACKS

    public override void OnDisconnected(DisconnectCause cause)
    {
        this.SetActivePanel(LoginPanel.name);
    }

    public override void OnConnectedToMaster()
    {
        this.SetActivePanel(SelectionPanel.name);
        Name.text = $"{PhotonNetwork.LocalPlayer.NickName}({PlayerManage.Wallet})";

        if(!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        SetActivePanel(SelectionPanel.name);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
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
        if ((BlackJackServerEvent)photonEvent.Code == BlackJackServerEvent.Start)
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

    public void OnLoginButtonClicked()
    {
        string playerName = PlayerNameInput.text;

        var request = new LoginWithCustomIDRequest { CustomId = playerName, CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, (loginResult) =>
        {
            //取餘額
            PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), userInventoryResult =>
            {
                if (userInventoryResult.VirtualCurrency.TryGetValue("PI", out int _myMoney))
                {
                    PlayerManage.Wallet = _myMoney;
                }
                else
                {
                    Debug.LogError("Not found PI Currency.");
                }
            }, (errResult) => {
                Debug.LogError("Get VirtualCurrency fail.");
            }
            );

            //新建帳號需修改DisplayName
            if (loginResult.NewlyCreated)
            {
                PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest { DisplayName = playerName }, null, (ue) =>
                {
                    Debug.LogError("update DisplayName fail.");
                });
            }

            //使用playfab登入photon
            PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest() { PhotonApplicationId = "d322b054-8fe5-4ced-9205-f709981ff085" }, (xr) =>
            {
                var customAuth = new AuthenticationValues { AuthType = CustomAuthenticationType.Custom };

                customAuth.AddAuthParameter("username", loginResult.PlayFabId);
                customAuth.AddAuthParameter("token", xr.PhotonCustomAuthenticationToken);

                PhotonNetwork.AuthValues = customAuth;
                PhotonNetwork.ConnectUsingSettings();
            },
            (xe) =>
            {
                Debug.LogError("Photon Authentication fail.");
            });

        }, (e) =>
        {
            Debug.LogError("Login Playfab fail.");
        });
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

}
