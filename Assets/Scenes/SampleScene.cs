﻿using Photon.Pun;
using Photon.Realtime;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleScene : MonoBehaviourPunCallbacks
{
    //[Header("Login Panel")]
    //public GameObject LoginPanel;

    //public InputField PlayerNameInput;

    //[Header("Selection Panel5")]
    //public GameObject SelectionPanel;

    //[Header("Create Room Panel")]
    //public GameObject CreateRoomPanel;

    //public InputField RoomNameInputField;
    //public InputField MaxPlayersInputField;

    //[Header("Join Random Room Panel")]
    //public GameObject JoinRandomRoomPanel;

    //[Header("Room List Panel")]
    //public GameObject RoomListPanel;

    //public GameObject RoomListContent;
    //public GameObject RoomListEntryPrefab;

    //[Header("Inside Room Panel")]
    //public GameObject InsideRoomPanel;

    //public GameObject GamePanel;

    //public Button StartGameButton;
    //public GameObject PlayerListEntryPrefab;

    //private Dictionary<string, RoomInfo> cachedRoomList;
    //private Dictionary<string, GameObject> roomListEntries;
    //private Dictionary<int, GameObject> playerListEntries;

    //private Logic twentyOneLogic;

    #region UNITY

    public void Awake()
    {
        var request = new LoginWithCustomIDRequest { CustomId = "", CreateAccount = true };
        //PhotonNetwork.AutomaticallySyncScene = true;

        //cachedRoomList = new Dictionary<string, RoomInfo>();
        //roomListEntries = new Dictionary<string, GameObject>();

        //PlayerNameInput.text = "Player " + Random.Range(1000, 10000);

        //twentyOneLogic = new Logic(transform);
        //twentyOneLogic.GameStartAct = () => { SetActivePanel(GamePanel.name); };

    }

    #endregion

    //#region PUN CALLBACKS

    //public override void OnConnectedToMaster()
    //{
    //    this.SetActivePanel(SelectionPanel.name);
    //}

    //public override void OnRoomListUpdate(List<RoomInfo> roomList)
    //{
    //    ClearRoomListView();

    //    UpdateCachedRoomList(roomList);
    //}

    //public override void OnLeftLobby()
    //{
    //    cachedRoomList.Clear();

    //    ClearRoomListView();
    //}

    //public override void OnCreateRoomFailed(short returnCode, string message)
    //{
    //    SetActivePanel(SelectionPanel.name);
    //}

    //public override void OnJoinRoomFailed(short returnCode, string message)
    //{
    //    SetActivePanel(SelectionPanel.name);
    //}

    //public override void OnJoinRandomFailed(short returnCode, string message)
    //{
    //    string roomName = "Room " + Random.Range(1000, 10000);

    //    RoomOptions options = new RoomOptions { MaxPlayers = 8, Plugins = new string[] { "MyPlugin" } };

    //    PhotonNetwork.CreateRoom(roomName, options, null);
    //}

    //public override void OnJoinedRoom()
    //{
    //    SetActivePanel(InsideRoomPanel.name);

    //    if (playerListEntries == null)
    //    {
    //        playerListEntries = new Dictionary<int, GameObject>();
    //    }

    //    foreach (Player p in PhotonNetwork.PlayerList)
    //    {
    //        GameObject entry = Instantiate(PlayerListEntryPrefab);
    //        entry.transform.SetParent(InsideRoomPanel.transform);
    //        entry.transform.localScale = Vector3.one;
    //        entry.GetComponent<PlayerListEntry>().Initialize(p.ActorNumber, p.NickName);

    //        object isPlayerReady;
    //        if (p.CustomProperties.TryGetValue(AsteroidsGame.PLAYER_READY, out isPlayerReady))
    //        {
    //            entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
    //        }

    //        playerListEntries.Add(p.ActorNumber, entry);
    //    }

    //    StartGameButton.gameObject.SetActive(CheckPlayersReady());

    //    Hashtable props = new Hashtable
    //        {
    //            {AsteroidsGame.PLAYER_LOADED_LEVEL, false}
    //        };
    //    PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    //}

    //public override void OnLeftRoom()
    //{
    //    SetActivePanel(SelectionPanel.name);

    //    foreach (GameObject entry in playerListEntries.Values)
    //    {
    //        Destroy(entry.gameObject);
    //    }

    //    playerListEntries.Clear();
    //    playerListEntries = null;
    //}

    //public override void OnPlayerEnteredRoom(Player newPlayer)
    //{
    //    GameObject entry = Instantiate(PlayerListEntryPrefab);
    //    entry.transform.SetParent(InsideRoomPanel.transform);
    //    entry.transform.localScale = Vector3.one;
    //    entry.GetComponent<PlayerListEntry>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

    //    playerListEntries.Add(newPlayer.ActorNumber, entry);

    //    StartGameButton.gameObject.SetActive(CheckPlayersReady());
    //}

    //public override void OnPlayerLeftRoom(Player otherPlayer)
    //{
    //    Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
    //    playerListEntries.Remove(otherPlayer.ActorNumber);

    //    StartGameButton.gameObject.SetActive(CheckPlayersReady());
    //}

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


    //#endregion


    //#region UI CALLBACKS

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

    //public void OnJoinRandomRoomButtonClicked()
    //{
    //    SetActivePanel(JoinRandomRoomPanel.name);

    //    PhotonNetwork.JoinRandomRoom();
    //}

    //public void OnLeaveGameButtonClicked()
    //{
    //    PhotonNetwork.LeaveRoom();
    //}

    //public void OnLoginButtonClicked()
    //{
    //    //string playerName = PlayerNameInput.text;

    //    //var request = new LoginWithCustomIDRequest { CustomId = playerName, CreateAccount = true };
    //    //PlayFabClientAPI.LoginWithCustomID(request, (t)=> {
    //    //    if (t.NewlyCreated)
    //    //    {
    //    //        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest { DisplayName = playerName }, null, null);

    //    //        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "getPhotonToken" }, (t)=> {
    //    //            var xx = t.Request;
    //    //        },(e)=> { 

    //    //        });
    //    //    }
    //    //}, (e)=> {
    //    //    Debug.LogError("Login Playfab fail.");
    //    //});

    //    //if (!playerName.Equals(""))
    //    //{
    //    //    PhotonNetwork.LocalPlayer.NickName = playerName;
    //    //    PhotonNetwork.ConnectUsingSettings();
    //    //}
    //    //else
    //    //{
    //    //    Debug.LogError("Player Name is invalid.");
    //    //}
    //}

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

    //#endregion

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

    //private void ClearRoomListView()
    //{
    //    foreach (GameObject entry in roomListEntries.Values)
    //    {
    //        Destroy(entry.gameObject);
    //    }

    //    roomListEntries.Clear();
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

    //private void SetActivePanel(string activePanel)
    //{
    //    LoginPanel.SetActive(activePanel.Equals(LoginPanel.name));
    //    SelectionPanel.SetActive(activePanel.Equals(SelectionPanel.name));
    //    CreateRoomPanel.SetActive(activePanel.Equals(CreateRoomPanel.name));
    //    JoinRandomRoomPanel.SetActive(activePanel.Equals(JoinRandomRoomPanel.name));
    //    RoomListPanel.SetActive(activePanel.Equals(RoomListPanel.name));    // UI should call OnRoomListButtonClicked() to activate this
    //    InsideRoomPanel.SetActive(activePanel.Equals(InsideRoomPanel.name));
    //    GamePanel.SetActive(activePanel.Equals(GamePanel.name));
    //}

    //private void UpdateCachedRoomList(List<RoomInfo> roomList)
    //{
    //    foreach (RoomInfo info in roomList)
    //    {
    //        // Remove room from cached room list if it got closed, became invisible or was marked as removed
    //        if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
    //        {
    //            if (cachedRoomList.ContainsKey(info.Name))
    //            {
    //                cachedRoomList.Remove(info.Name);
    //            }

    //            continue;
    //        }

    //        // Update cached room info
    //        if (cachedRoomList.ContainsKey(info.Name))
    //        {
    //            cachedRoomList[info.Name] = info;
    //        }
    //        // Add new room info to cache
    //        else
    //        {
    //            cachedRoomList.Add(info.Name, info);
    //        }
    //    }
    //}

}
