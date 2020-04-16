using Assets.Scripts;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

public class BlackJackRoomPrefab : MonoBehaviour
{
    public Text RoomName;
    public Text RoomContent;

    private BlackJackRoom _room;
    Action<BlackJackRoom> _ckickAct;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(BlackJackRoom room, Action<BlackJackRoom> ckickAct)
    {
        _room = room;
        _ckickAct = ckickAct;

        RoomName.text = _room.Name;
        RoomContent.text = $"賭注: {room.Bet}\n";
        RoomContent.text += $"餘額限制: {room.RequirMoney}";
    }

    public void OnButtonClick()
    {
        _ckickAct?.Invoke(_room);
    }


}
