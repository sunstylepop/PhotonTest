using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Auth
{
    class GuestLogin : IAuth
    {

        public void Auth(string customId, Action<string> AuthSuccessCallback, Action<PlayFabError> AuthFailCallback)
        {
            var Identifier = SystemInfo.deviceUniqueIdentifier;
            var request = new LoginWithCustomIDRequest { CustomId = Identifier, CreateAccount = true };

            PlayFabClientAPI.LoginWithCustomID(request, (loginResult) =>
            {

                //新建帳號需修改DisplayName
                if (loginResult.NewlyCreated)
                {
                    PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest { DisplayName = Identifier.Substring(0, 6) }, null, (ue) =>
                    {
                        Debug.LogError("update DisplayName fail.");
                    });
                }

                AuthSuccessCallback(loginResult.PlayFabId);

            }, AuthFailCallback);
        }
    }
}
