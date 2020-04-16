using Assets.Scripts;
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

public class StoreItemPrefab : MonoBehaviour
{
    public Text NameTxt;
    public Text PriseTxt;
    public Text DescriptionTxt;

    private int _prise;
    private string _itemId;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(string name, int prise, string description, string ItemId)
    {
        _prise = prise;
        _itemId = ItemId;

        NameTxt.text = name;
        PriseTxt.text = "$" + prise.ToString();
        DescriptionTxt.text = description;
    }


    public void OnRowClick()
    {
        ModalHelper.ConfirmMessage($"確定要以 {_prise}元 購買 [{NameTxt.text}] 嗎?", BuySomething);

    }

    private void BuySomething()
    {
        PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest() { CatalogVersion = "main", StoreId = "FruitStore", VirtualCurrency = "PI", ItemId = _itemId, Price = _prise }, (r) =>
        {
            var mainItem = r.Items[0].DisplayName;  //第一個一定是主項目

            //若有多項產品會放在主項目後面
            string subItem = "";
            if (r.Items.Count > 1)
            {
                var subItemAry = r.Items.Where((v, i) => i > 0).Select(x => x.DisplayName);
                subItem = "，獲得下列物品:" + string.Join("、", subItemAry);
            }

            ModalHelper.WarningMessage("購買成功 [" + r.Items[0].DisplayName + "] 成功" + subItem);

        }, (e) =>
        {
            ModalHelper.WarningMessage("PurchaseItem fail. " + e.ErrorMessage);
        });
    }

}
