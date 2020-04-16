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

public class BagPanel : MonoBehaviour, IPanel
{
    public GameObject ItemPrefab;
    public GameObject ItemContent;
    public Text BagInfo;


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
        ShowBagItem();
    }

    private void ShowBagItem()
    {
        //清除舊資料
        foreach (var g in ItemEntries)
        {
            Destroy(g.gameObject);
        }
        ItemEntries.Clear();

        //重新抓取商店資料
        PlayerManage.UpdateInventory(()=> {
            foreach (var s in PlayerManage.Inventory)
            {

                GameObject entry = Instantiate(ItemPrefab);
                entry.transform.SetParent(ItemContent.transform);
                ItemEntries.Add(entry);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<ItemInBagPrefab>().Initialize(s, ShowBagItem);
            }

            BagInfo.text = $"餘額: {PlayerManage.Wallet}\n";
            BagInfo.text += $"物品總數量: {PlayerManage.Inventory.Count}";
        });

    }

    public void OnBackClicked()
    {
        GetComponentInParent<MainPanel>().SetActivePanel(SysPanel.SelectionPanel);
    }

}
