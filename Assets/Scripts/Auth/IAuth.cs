using PlayFab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Auth
{
    public interface IAuth
    {
        void Auth(string customId, Action<string> AuthSuccessCallback, Action<PlayFabError> AuthFailCallback);
    }
}
