using Assets.Scripts;
using Assets.Scripts.Lobby;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

public class ItemInBagPrefab : MonoBehaviour
{
    public Text NameTxt;
    public Text DescriptionTxt;

    private ItemInstance _instance;
    private CatalogItem _catalogItem;
    private Action _reSearchBag;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(ItemInstance instance, Action ReSearchBag)
    {
        _instance = instance;
        _reSearchBag = ReSearchBag;
        SystemManage.AllStoreItem.TryGetValue(instance.ItemId, out _catalogItem);

        string consumableMsg = "";
        if (instance.RemainingUses.HasValue)
        {
            consumableMsg = "  X " + instance.RemainingUses;
        }
        if (instance.Expiration.HasValue)
        {
            consumableMsg += $" ({instance.Expiration.Value.ToString("MM/dd hh:mm")})";
        }
        NameTxt.text = instance.DisplayName + consumableMsg;

        if(_catalogItem != null)
        {
            DescriptionTxt.text = _catalogItem.Description;
        }
    }


    public void OnRowClick()
    {
        ModalHelper.ConfirmMessage($"確定使用 [{_instance.DisplayName}] 嗎?", UseSomething);

    }

    private void UseSomething()
    {
        if (_catalogItem == null) return;

        Action<PlayFabError> errAct = (e) => { ModalHelper.WarningMessage("UseSomething fail. " + e.ErrorMessage); };

        if (_catalogItem.Container != null)
        {
            PlayFabClientAPI.UnlockContainerInstance(new UnlockContainerInstanceRequest() { CatalogVersion = "main", ContainerItemInstanceId = _instance.ItemInstanceId }, (r) =>
            {
                var grantedItem = r.GrantedItems.Select(x=>x.DisplayName);
                string getedItemMsg = "，獲得:" + string.Join("、", grantedItem);

                ModalHelper.WarningMessage("成功使用 [" + _instance.DisplayName + "]" + getedItemMsg);
                _reSearchBag?.Invoke();
            }, errAct);
        }
        else
        {
            PlayFabClientAPI.ConsumeItem(new ConsumeItemRequest() { ItemInstanceId = _instance.ItemInstanceId, ConsumeCount = 1 }, (r) =>
            {
                ModalHelper.WarningMessage("成功使用 [" + _instance.DisplayName + "]");
                _reSearchBag?.Invoke();
            }, errAct);
        }
    
    }

}
