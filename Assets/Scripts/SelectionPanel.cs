using Assets.Scripts;
using Assets.Scripts.Game.BlackJack.Model;
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

    }

    public void OnLogoutButtonClicked()
    {
        PhotonNetwork.Disconnect();
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
        //PlayFabClientAPI.UnlockContainerInstance(new UnlockContainerInstanceRequest() { CatalogVersion = "main", ContainerItemInstanceId = "15E57E5032FA17BE", KeyItemInstanceId = "F38E65D447661047" }, (r) =>
        //{
        //    var xx = r;
        //}, (e) =>
        //{
        //    Debug.LogError(e.ErrorMessage);
        //});


        //PlayFabClientAPI.ConsumeItem(new ConsumeItemRequest() { ItemInstanceId = "48FE0599EA0FEE36", ConsumeCount = 1 }, (r) =>
        //{
        //    var xx = r;
        //}, (e) =>
        //{
        //    Debug.LogError(e.ErrorMessage);
        //});

        GetComponentInParent<MainPanel>().SetActivePanel(SysPanel.BagPanel);
    }

    public void OnStoreButtonClicked()
    {
        //PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest() { CatalogVersion = "main" }, (r) =>
        //{
        //    var xx = r;
        //}, (e) =>
        //{

        //});

        //PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest() { CatalogVersion = "main", ItemId = "TwoRndPack", VirtualCurrency = "PI", Price = 1 }, (r) =>
        //{
        //    var xx = r;
        //}, (e) =>
        //{
        //    Debug.LogError(e.ErrorMessage);
        //});

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





    private void JoinGameRoom(RoomLevel level)
    {
        GetComponentInParent<MainPanel>().JoinGameRoom(level);
    }
}
