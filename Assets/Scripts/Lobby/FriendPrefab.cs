using Assets.Scripts;
using Assets.Scripts.Game.BlackJack.Model;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

public class FriendPrefab : MonoBehaviour
{
    public Image StateImg;
    public Text NameTxt;
    public Transform friendInfoBlock;

    private bool IsQuery = false;
    private int win = 0;
    private int loss = 0;
    private int tie = 0;
    private bool OnlineState = false;
    private string UserID;
    private Action ReGetFriendFunc;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(string Name, string userId, Transform transform, Action GetAndShowFriendList)
    {
        ReGetFriendFunc = GetAndShowFriendList;
        UserID = userId;
        friendInfoBlock = transform;
        NameTxt.text = Name;
        StateImg.color = new Color(0.6698113f, 0.6698113f, 0.6698113f, 1);
    }

    public void SetOnlineState(bool s)
    {
        OnlineState = s;

        if (s)
            StateImg.color = new Color(0.0849059f, 0.5660378f, 0.07742971f, 1);
        else
            StateImg.color = new Color(0.6698113f, 0.6698113f, 0.6698113f, 1);
    }

    public void OnRowClick()
    {
        //顯示detail內容
        Action showDetail = () =>
        {
            decimal total = win + loss + tie;
            decimal rate = 0;
            if (total > 0)
            {
                rate = Math.Round((win / total), 4) * 100;
            }

            friendInfoBlock.Find("name").GetComponent<Text>().text = "暱稱: " + NameTxt.text;
            friendInfoBlock.Find("Rate").GetComponent<Text>().text = "勝率: " + rate.ToString("0.##") + $"(贏:{win} 輸:{loss} 平:{tie})";
            friendInfoBlock.gameObject.SetActive(true);
        };


        if (!IsQuery)
        {
            PlayFabClientAPI.GetUserReadOnlyData(new PlayFab.ClientModels.GetUserDataRequest()
            {
                Keys = new List<string>() { "win", "loss", "tie" },
                PlayFabId = UserID
            }, (r) => {
                IsQuery = true;

                if (r.Data.TryGetValue("win", out var winObj))
                {
                    win = int.Parse(winObj.Value);
                }

                if (r.Data.TryGetValue("loss", out var lossObj))
                {
                    loss = int.Parse(lossObj.Value);
                }

                if (r.Data.TryGetValue("tie", out var tieObj))
                {
                    tie = int.Parse(tieObj.Value);
                }

                showDetail();

            }, (e) => {
                ModalHelper.WarningMessage("Get UserDataRequest fail.");
            });

            return;
        }

        showDetail();

    }

    public void OnRemoveClicked()
    {
        PlayFabClientAPI.RemoveFriend(new PlayFab.ClientModels.RemoveFriendRequest() { FriendPlayFabId = UserID }, (r) => {
            ReGetFriendFunc?.Invoke();
        }, (e) => {
            ModalHelper.WarningMessage("RemoveFriend fail.");
        });
    }

}
