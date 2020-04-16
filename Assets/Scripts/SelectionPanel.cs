using Assets.Scripts;
using Assets.Scripts.Game;
using Assets.Scripts.Lobby;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionPanel : MonoBehaviour, IPanel
{
    public Text Name;
    public GameObject RoomContent;
    public GameObject RoomPrefab;

    private BlackJackRoom LastJoinRoom { get; set; }
    private List<GameObject> RoomListEntries = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Init()
    {
        if (GamePanel.rePlay)
        {
            GamePanel.rePlay = false;
            JoinGameRoom(LastJoinRoom);
        }
        else if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby(SystemManage.SqlLobby);

            StartCoroutine(OnUpdateRoomEvent());

            PlayerManage.UpdateInventory(showLobbyPlayerInfo); //更新金額
            PlayerManage.UpdateProfit(showLobbyPlayerInfo); //更新個人資料
        }
    }

    public void OnLogoutButtonClicked()
    {
        PhotonNetwork.Disconnect();
    }

    public void OnFriendButtonClicked()
    {
        GetComponentInParent<MainPanel>().SetActivePanel(SysPanel.FriendPanel);
    }

    public void OnStatisticsButtonClicked()
    {
        GetComponentInParent<MainPanel>().SetActivePanel(SysPanel.StatisticsPanel);
    }

    public void OnBagButtonClicked()
    {
        GetComponentInParent<MainPanel>().SetActivePanel(SysPanel.BagPanel);
    }

    public void OnStoreButtonClicked()
    {
        GetComponentInParent<MainPanel>().SetActivePanel(SysPanel.StorePanel);
    }

    public void showLobbyPlayerInfo()
    {
        var _name = PhotonNetwork.LocalPlayer.NickName.Length <= 6 ? PhotonNetwork.LocalPlayer.NickName : PhotonNetwork.LocalPlayer.NickName.Substring(0, 6);

        decimal total = (PlayerManage.win + PlayerManage.loss + PlayerManage.tie);
        decimal rate = 0;
        if (total > 0)
        {
            rate = Math.Round((PlayerManage.win / total), 4) * 100;
        }


        Name.text = $"暱稱: {_name}\n";
        Name.text += $"餘額: {PlayerManage.Wallet}\n";
        Name.text += $"勝: {PlayerManage.win}\n";
        Name.text += $"敗: {PlayerManage.loss}\n";
        Name.text += $"平: {PlayerManage.tie}\n";
        Name.text += $"勝率: {rate.ToString("0.##")}%";
    }

    public IEnumerator OnUpdateRoomEvent()
    {
        if(SystemManage.BlackJackRooms != null)
        {
            foreach (var g in RoomListEntries)
            {
                Destroy(g.gameObject);
            }
            RoomListEntries.Clear();

            foreach (var r in SystemManage.BlackJackRooms)
            {

                GameObject entry = Instantiate(RoomPrefab);
                entry.transform.SetParent(RoomContent.transform);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<BlackJackRoomPrefab>().Initialize(r, JoinGameRoom);

                RoomListEntries.Add(entry);
            }
        }
        else
        {
            yield return new WaitForSeconds(1);
            StartCoroutine(OnUpdateRoomEvent());
        }
    }


    public void CreateGameRoom()
    {
        if (LastJoinRoom == null) return;

        string roomName = "Room " + UnityEngine.Random.Range(1000, 10000);

        RoomOptions options = BlackJackRoomManage.GetRoomOption(LastJoinRoom);

        PhotonNetwork.CreateRoom(roomName, options, SystemManage.SqlLobby);
    }

    private void JoinGameRoom(BlackJackRoom room)
    {
        if (room == null)
        {
            ModalHelper.WarningMessage("發生錯誤, 找不到該房間資訊!", Init);
            return;
        }

        if (PlayerManage.Wallet < room.RequirMoney)
        {
            ModalHelper.WarningMessage("餘額小於房間金錢限制!", Init);
            return;
        }

        LastJoinRoom = room;

        var sqlFilter = BlackJackRoomManage.GetSqlLobbyFilter(room);
        PhotonNetwork.JoinRandomRoom(null, 0, MatchmakingMode.FillRoom, SystemManage.SqlLobby, sqlFilter);
    }

}
