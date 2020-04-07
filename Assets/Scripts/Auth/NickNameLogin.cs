using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Auth
{
    class NickNameLogin : IAuth
    {

        public void Auth(string customId, Action<string> AuthSuccessCallback, Action<PlayFabError> AuthFailCallback)
        {
            //customId等於NickName

            var request = new LoginWithCustomIDRequest { CustomId = customId, CreateAccount = true };

            PlayFabClientAPI.LoginWithCustomID(request, (loginResult) =>
            {

                //新建帳號需修改DisplayName
                if (loginResult.NewlyCreated)
                {
                    PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest { DisplayName = customId }, (xe)=> {
                        var xxx = xe;
                    }, (ue) =>
                    {
                        Debug.LogError("update DisplayName fail.");
                    });
                }

                AuthSuccessCallback(loginResult.PlayFabId);

            }, AuthFailCallback);
        }
    }
}
