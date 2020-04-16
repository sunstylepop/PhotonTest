using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Lobby
{
    class SystemManage
    {
        public static TypedLobby SqlLobby = new TypedLobby("myLobby", LobbyType.SqlLobby);

        public static Dictionary<string, CatalogItem> AllStoreItem { get; set; }
        public static Dictionary<string, string> AllTitleData { get; set; }
        public static List<BlackJackRoom> BlackJackRooms { get; set; }

        public static void GetAllStroeItem()
        {
            PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest() { CatalogVersion = "main" }, (r) =>
            {
                AllStoreItem = r.Catalog.ToDictionary(k => k.ItemId, v => v);
            }, (e) =>
            {
                ModalHelper.WarningMessage("Get CatalogItems fail. " + e.ErrorMessage);
            });
        }

        public static void GetAllTitleData()
        {
            PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), (r) =>
            {
                AllTitleData = r.Data;

                //讀取21點所有房間
                if (AllTitleData.TryGetValue("BlackJackRooms", out string blackJackRoomJson))
                    BlackJackRooms = LitJson.JsonMapper.ToObject<List<BlackJackRoom>>(blackJackRoomJson);
            }, (e) =>
            {
                ModalHelper.WarningMessage("Get AllTitleData fail. " + e.ErrorMessage);
            });
        }

    }

}
