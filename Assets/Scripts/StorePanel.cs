using Assets.Scripts;
using Assets.Scripts.Lobby;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StorePanel : MonoBehaviour, IPanel
{
    public Text storeName;
    public Text storeDescription;
    public GameObject StorePrefab;
    public GameObject PackItemContent;
    public GameObject ItemContent;


    private List<GameObject> PackItemEntries = new List<GameObject>();
    private List<GameObject> ItemEntries = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init()
    {
        ShowStoreItem();
    }

    private void ShowStoreItem()
    {
        //清除舊的商店資料
        foreach (var g in ItemEntries)
        {
            Destroy(g.gameObject);
        }
        ItemEntries.Clear();

        foreach (var g in PackItemEntries)
        {
            Destroy(g.gameObject);
        }
        PackItemEntries.Clear();

        //重新抓取商店資料
        PlayFabClientAPI.GetStoreItems(new GetStoreItemsRequest() { CatalogVersion = "main", StoreId = "FruitStore" }, (r) =>
        {
            storeName.text = r.MarketingData.DisplayName;
            storeDescription.text = r.MarketingData.Description;

            foreach (var s in r.Store)
            {

                if(SystemManage.AllStoreItem.TryGetValue(s.ItemId, out CatalogItem _item))
                {
                    GameObject entry = Instantiate(StorePrefab);
                    if(_item.Bundle != null || _item.Container != null)
                    {
                        entry.transform.SetParent(PackItemContent.transform);
                        PackItemEntries.Add(entry);
                    }
                    else
                    {
                        entry.transform.SetParent(ItemContent.transform);
                        ItemEntries.Add(entry);
                    }
                    entry.transform.localScale = Vector3.one;
                    var _prise = s.VirtualCurrencyPrices.TryGetValue("PI", out var val) ? (int)val : 0;

                    entry.GetComponent<StoreItemPrefab>().Initialize(_item.DisplayName, _prise, _item.Description, s.ItemId);
                }

            }
        }, (e) =>
        {
            ModalHelper.WarningMessage("GetStoreItems faile. " + e.ErrorMessage);
        });

    }

    public void OnBackClicked()
    {
        GetComponentInParent<MainPanel>().SetActivePanel(SysPanel.SelectionPanel);
    }

}
