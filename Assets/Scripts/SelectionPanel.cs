﻿using Assets.Scripts;
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
        PlayerManage.GetAllStroeItem();                 //取得所有物品
        PlayerManage.UpdateInventory(showLobbyPlayerInfo); //更新金額
        PlayerManage.UpdateProfit(showLobbyPlayerInfo); //更新個人資料
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





    private void JoinGameRoom(RoomLevel level)
    {
        GetComponentInParent<MainPanel>().JoinGameRoom(level);
    }

}
