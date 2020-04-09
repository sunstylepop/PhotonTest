using Facebook.Unity;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Auth
{
    class FaceBookLogin : IAuth
    {
        public void Auth(string customId, Action<string> AuthSuccessCallback, Action<PlayFabError> AuthFailCallback)
        {
            FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, (result) =>
            {
                if (result.Cancelled)
                {
                    AuthFailCallback(null);
                }
                else if (!string.IsNullOrEmpty(result.Error))
                {
                    AuthFailCallback(null);
                    Debug.LogError("FaceBookLogin fail: " + result.Error);
                }
                else
                {
                    PlayFabClientAPI.LoginWithFacebook(new LoginWithFacebookRequest()
                    {
                        AccessToken = result.AccessToken.TokenString,
                        CreateAccount = true
                    }, (loginResult) =>
                    {
                        if (loginResult.NewlyCreated)
                        {
                            FB.API("/me?fields=id,name,gender,email", HttpMethod.GET, (r) =>
                            {
                                if (r.ResultDictionary.TryGetValue("name", out string name))
                                {
                                    PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest { DisplayName = name }, null, (ue) =>
                                    {
                                        Debug.LogError("update DisplayName fail.");
                                    });
                                }

                            });
                        }

                        AuthSuccessCallback(loginResult.PlayFabId);
                    }, AuthFailCallback);

                }

            });
        }

    }
}
