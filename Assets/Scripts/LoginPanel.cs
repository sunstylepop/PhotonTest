using Assets.Scripts.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    private InputField PlayerNameInput;

    void Start()
    {
        PlayerNameInput = transform.Find("PlayerNameInput").GetComponent<InputField>();
        PlayerNameInput.text = "MM2";
    }

    public void OnGuestLoginButtonClicked()
    {
        Login<GuestLogin>();
    }

    public void OnFBLoginButtonClicked()
    {
        Login<FaceBookLogin>();
    }

    public void OnLoginButtonClicked()
    {
        string playerName = PlayerNameInput.text;

        Login<NickNameLogin>(playerName);
    }

    public void ResetLoginMsg()
    {
        var LoginMsg = transform.Find("LoginMsg").GetComponent<Text>();
        LoginMsg.text = "";
    }

    private void Login<T>(string nickName = null) where T : IAuth, new()
    {
        Loginer _loginer = transform.Find("LoginMsg").GetComponent<Loginer>();
        _loginer.Login<T>(nickName);
    }
}
