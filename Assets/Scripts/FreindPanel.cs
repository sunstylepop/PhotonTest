using Assets.Scripts;
using Assets.Scripts.Game.BlackJack.Model;
using Assets.Scripts.Lobby;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using PlayFabFriendInfo = PlayFab.ClientModels.FriendInfo;
using FriendInfo = Photon.Realtime.FriendInfo;

public class FreindPanel : MonoBehaviour, IPanel
{
    public GameObject FriendPrefab;
    public GameObject FriendContent;
    public InputField NewFriendNameInput;

    private Dictionary<string, GameObject> FriendListEntries = new Dictionary<string, GameObject>();

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
        GetAndShowFriendList();
    }

    public void OnBackToLobbyButtonClicked()
    {
        GetComponentInParent<MainPanel>().SetActivePanel(SysPanel.SelectionPanel);
    }

    public void OnAddNewFriendButtonClicked()
    {
        if (string.IsNullOrEmpty(NewFriendNameInput.text)) return;

        PlayFabClientAPI.AddFriend(new AddFriendRequest() { FriendTitleDisplayName = NewFriendNameInput.text },
            (r) => {
                ModalHelper.WarningMessage("成功將 [" + NewFriendNameInput.text + "] 加入為好友");
                GetAndShowFriendList();
            }, (e) => {
                ModalHelper.WarningMessage("加入好友失敗!!");
            });
    }

    public void SetOnlineState(List<FriendInfo> friendList)
    {
        foreach (var f in friendList)
        {
            if (FriendListEntries.TryGetValue(f.UserId, out var entry))
            {
                entry.GetComponent<FriendPrefab>().SetOnlineState(f.IsOnline);
            }
        }
    }











    private void GetAndShowFriendList()
    {
        //初始化隱藏朋友detail資料
        var friendInfoBlock = transform.Find("friendInfoBlock");
        friendInfoBlock.gameObject.SetActive(false);

        //清除舊的朋友資料
        foreach (var g in FriendListEntries)
        {
            Destroy(g.Value.gameObject);
        }
        FriendListEntries.Clear();

        //重新抓取朋友資料
        PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest(), (x) =>
        {
            foreach (var p in x.Friends)
            {
                var _name = p.TitleDisplayName.Length <= 6 ? p.TitleDisplayName : p.TitleDisplayName.Substring(0, 6);

                GameObject entry = Instantiate(FriendPrefab);
                entry.transform.SetParent(FriendContent.transform);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<FriendPrefab>().Initialize(_name, p.FriendPlayFabId, friendInfoBlock, GetAndShowFriendList);


                FriendListEntries.Add(p.FriendPlayFabId, entry);
            }

            if (x.Friends.Count > 0)
                PhotonNetwork.FindFriends(x.Friends.Select(f => f.FriendPlayFabId).ToArray());

        }, (e) =>
        {
            ModalHelper.WarningMessage("Get FriendsList fail");
        });
    }

}
