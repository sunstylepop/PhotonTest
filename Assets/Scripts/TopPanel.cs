using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopPanel : MonoBehaviour
{
    private readonly string connectionStatusMessage = "    Connection Status: ";
    private readonly string pingMessage = "Ping: ";

    public Text ConnectionStatusText;
    public Text PingText;


    // Update is called once per frame
    void Update()
    {
        ConnectionStatusText.text = connectionStatusMessage + PhotonNetwork.NetworkClientState;
        PingText.text = pingMessage + PhotonNetwork.GetPing() + "ms";
    }
}
