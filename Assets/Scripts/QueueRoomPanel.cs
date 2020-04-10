using Assets.Scripts;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QueueRoomPanel : MonoBehaviour, IPanel
{
    public GameObject Character1;
    public GameObject Character2;
    public GameObject Character3;
    public GameObject Character4;

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
        SetCharacterInQueueRoom();
    }

    public void SetCharacterInQueueRoom()
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
        foreach (var _p in PhotonNetwork.PlayerList)
        {
            var info = $"{_p.NickName}({(int)_p.CustomProperties[PlayerProperty.Money]})";
            if (i == 0) SetCharacter(Character1, info);
            if (i == 1) SetCharacter(Character2, info);
            if (i == 2) SetCharacter(Character3, info);
            if (i == 3) SetCharacter(Character4, info);

            i++;
        }

    }

    public void OnBackClicked()
    {
        PhotonNetwork.LeaveRoom();
    }
}
