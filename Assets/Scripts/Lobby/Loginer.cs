using Assets.Scripts.Auth;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loginer : MonoBehaviour
{
    public Text LoginMsg;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLoginMsg(string msg)
    {
        LoginMsg.text = msg;
    }

    public void Login<T>(string nickName = null) where T : IAuth, new()
    {
        T _t = new T();

        LoginMsg.text = "Auth....";
        _t.Auth(nickName, AuthSuccessCallback, AuthFailCallback);
    }

    private void AuthFailCallback(PlayFabError error)
    {
        LoginMsg.text = "Auth Fail";

        if(error != null)
            Debug.LogError("Auth Fail" + error.ErrorMessage);
    }

    private void AuthSuccessCallback(string UserID)
    {
        LoginMsg.text = "Auth Success";

        //使用playfab登入photon
        PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest() { PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime }, (xr) =>
        {
            var customAuth = new AuthenticationValues { AuthType = CustomAuthenticationType.Custom };

            customAuth.AddAuthParameter("username", UserID);
            customAuth.AddAuthParameter("token", xr.PhotonCustomAuthenticationToken);

            PhotonNetwork.AuthValues = customAuth;
            PhotonNetwork.ConnectUsingSettings();

            LoginMsg.text = "Login....";
        },
        (xe) =>
        {
            LoginMsg.text = "Login Fail";
            Debug.LogError("Photon Authentication fail.");
        });
    }

}
