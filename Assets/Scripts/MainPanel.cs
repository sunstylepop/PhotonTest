using Assets.Scripts;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Assets.Scripts.Game.BlackJack.Model;

public class MainPanel : MonoBehaviourPunCallbacks
{
    private int MyMoney { get; set; }
    private RoomLevel SelectedRoomLevel { get; set; }
    private TypedLobby SqlLobby = new TypedLobby("myLobby", LobbyType.SqlLobby);


    [Header("Login Panel")]
    public GameObject LoginPanel;
    public InputField PlayerNameInput;

    [Header("Selection Panel5")]
    public GameObject SelectionPanel;
    public Text Name;

    //[Header("Create Room Panel")]
    //public GameObject CreateRoomPanel;

    //public InputField RoomNameInputField;
    //public InputField MaxPlayersInputField;

    [Header("Queue Room Panel")]
    public GameObject QueueRoomPanel;
    public GameObject Character1;
    public GameObject Character2;
    public GameObject Character3;
    public GameObject Character4;

    //[Header("Room List Panel")]
    //public GameObject RoomListPanel;

    //public GameObject RoomListContent;
    //public GameObject RoomListEntryPrefab;

    //[Header("Inside Room Panel")]
    //public GameObject InsideRoomPanel;

    //public GameObject GamePanel;

    //public Button StartGameButton;
    //public GameObject PlayerListEntryPrefab;

    //private Dictionary<string, GameObject> roomListEntries;
    //private Dictionary<int, GameObject> playerListEntries;

    //private Logic twentyOneLogic;

    #region UNITY

    public void Awake()
    {
        //PhotonNetwork.AutomaticallySyncScene = true;

        //roomListEntries = new Dictionary<string, GameObject>();

        PlayerNameInput.text = "MM2";

        //twentyOneLogic = new Logic(transform);
        //twentyOneLogic.GameStartAct = () => { SetActivePanel(GamePanel.name); };

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
        Name.text = PhotonNetwork.LocalPlayer.NickName;

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
        string roomName = "Room " + Random.Range(1000, 10000);

        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 4,
            CustomRoomProperties = RoomManage.GetRoomProperty(SelectedRoomLevel),
            CustomRoomPropertiesForLobby = RoomManage.GetRoomPropertiesForLobby(),
            Plugins = new string[] { "MyPlugin" }
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

    //public override void OnMasterClientSwitched(Player newMasterClient)
    //{
    //    if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
    //    {
    //        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    //    }
    //}

    //public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    //{
    //    if (playerListEntries == null)
    //    {
    //        playerListEntries = new Dictionary<int, GameObject>();
    //    }

    //    GameObject entry;
    //    if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
    //    {
    //        object isPlayerReady;
    //        if (changedProps.TryGetValue(AsteroidsGame.PLAYER_READY, out isPlayerReady))
    //        {
    //            entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
    //        }
    //    }

    //    StartGameButton.gameObject.SetActive(CheckPlayersReady());
    //}


    #endregion


    #region UI CALLBACKS

    //public void OnGetCardButtonClicked()
    //{
    //    PhotonNetwork.RaiseEvent((byte)TwentyOneClientEvent.GetCard, null, null, SendOptions.SendReliable);
    //}

    //public void OnPassButtonClicked()
    //{
    //    PhotonNetwork.RaiseEvent((byte)TwentyOneClientEvent.Pass, null, null, SendOptions.SendReliable);
    //}

    //public void OnEvent(EventData photonEvent)
    //{
    //    if (twentyOneLogic != null)
    //        twentyOneLogic.Listens(photonEvent);
    //}

    //public void OnBackButtonClicked()
    //{
    //    if (PhotonNetwork.InLobby)
    //    {
    //        PhotonNetwork.LeaveLobby();
    //    }

    //    SetActivePanel(SelectionPanel.name);
    //}

    //public void OnCreateRoomButtonClicked()
    //{
    //    string roomName = RoomNameInputField.text;
    //    roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;

    //    byte maxPlayers;
    //    byte.TryParse(MaxPlayersInputField.text, out maxPlayers);
    //    maxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 8);

    //    RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers, Plugins = new string[] { "MyPlugin" } };

    //    PhotonNetwork.CreateRoom(roomName, options, null);
    //}

    private void JoinGameRoom(RoomLevel level)
    {
        SelectedRoomLevel = level;
        
        if(!RoomManage.CanOpenRoom(level, MyMoney))
        {
            Debug.LogError("You Don't have enough money");
            return;
        }

        var sqlFilter = RoomManage.GetSqlLobbyFilter(level, MyMoney);

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

    public void OnLoginButtonClicked()
    {
        string playerName = PlayerNameInput.text;

        var request = new LoginWithCustomIDRequest { CustomId = playerName, CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, (loginResult) =>
        {
            //取餘額
            PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), userInventoryResult => 
            {
                if(userInventoryResult.VirtualCurrency.TryGetValue("PI", out int _myMoney))
                {
                    MyMoney = _myMoney;
                }
            }, (y) => { 
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

    //public void OnRoomListButtonClicked()
    //{
    //    if (!PhotonNetwork.InLobby)
    //    {
    //        PhotonNetwork.JoinLobby();
    //    }

    //    SetActivePanel(RoomListPanel.name);
    //}

    //public void OnStartGameButtonClicked()
    //{
    //    PhotonNetwork.CurrentRoom.IsOpen = false;
    //    PhotonNetwork.CurrentRoom.IsVisible = false;

    //    //PhotonNetwork.LoadLevel("DemoAsteroids-GameScene");

    //    PhotonNetwork.RaiseEvent((byte)TwentyOneClientEvent.Start, null, null, SendOptions.SendReliable);
    //}

    #endregion

    //private bool CheckPlayersReady()
    //{
    //    if (!PhotonNetwork.IsMasterClient)
    //    {
    //        return false;
    //    }

    //    foreach (Player p in PhotonNetwork.PlayerList)
    //    {
    //        object isPlayerReady;
    //        if (p.CustomProperties.TryGetValue(AsteroidsGame.PLAYER_READY, out isPlayerReady))
    //        {
    //            if (!(bool)isPlayerReady)
    //            {
    //                return false;
    //            }
    //        }
    //        else
    //        {
    //            return false;
    //        }
    //    }

    //    return true;
    //}

    //public void LocalPlayerPropertiesUpdated()
    //{
    //    StartGameButton.gameObject.SetActive(CheckPlayersReady());
    //}

    //public void BackToLoginPage()
    //{
    //    PhotonNetwork.Disconnect();
    //    SetActivePanel(LoginPanel.name);
    //}

    private void SetActivePanel(string activePanel)
    {
        LoginPanel.SetActive(activePanel.Equals(LoginPanel.name));
        SelectionPanel.SetActive(activePanel.Equals(SelectionPanel.name));
        QueueRoomPanel.SetActive(activePanel.Equals(QueueRoomPanel.name));
        //RoomListPanel.SetActive(activePanel.Equals(RoomListPanel.name));    // UI should call OnRoomListButtonClicked() to activate this
        //InsideRoomPanel.SetActive(activePanel.Equals(InsideRoomPanel.name));
        //GamePanel.SetActive(activePanel.Equals(GamePanel.name));
    }

    private void SetCharacterInQueueRoom()
    {
        Character1.SetActive(false);
        Character2.SetActive(false);
        Character3.SetActive(false);
        Character4.SetActive(false);

        for(var i = 0; i< PhotonNetwork.PlayerList.Length; i++)
        {
            if(i == 0) Character1.SetActive(true);
            if(i == 1) Character2.SetActive(true);
            if(i == 2) Character3.SetActive(true);
            if(i == 3) Character4.SetActive(true);
        }

    }

}
