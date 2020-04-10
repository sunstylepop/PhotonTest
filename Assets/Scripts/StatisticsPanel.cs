using Assets.Scripts;
using Assets.Scripts.Game.BlackJack.Model;
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

public class StatisticsPanel : MonoBehaviour, IPanel
{
    public GameObject StatisticsContent;
    public GameObject StatisticsPrefab;

    private List<GameObject> StatisticsListEntries = new List<GameObject>();

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
        OnStatisticsButtonClicked();
    }

    public void OnBackClicked()
    {
        GetComponentInParent<MainPanel>().SetActivePanel(SysPanel.SelectionPanel);
    }

    private void OnStatisticsButtonClicked()
    {
        foreach (var g in StatisticsListEntries)
        {
            Destroy(g.gameObject);
        }
        StatisticsListEntries.Clear();

        PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest() { StatisticName = "PlayerHighScore", MaxResultsCount = 10 }, (boardResult) =>
        {
            int i = 0;
            foreach (var b in boardResult.Leaderboard)
            {
                var _name = b.DisplayName.Length <= 6 ? b.DisplayName : b.DisplayName.Substring(0, 6);

                GameObject entry = Instantiate(StatisticsPrefab);
                entry.transform.SetParent(StatisticsContent.transform);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<RowPrefab>().Initialize(++i, _name, b.StatValue);

                if (b.PlayFabId == PhotonNetwork.LocalPlayer.UserId)
                {
                    var img = entry.GetComponent<Image>();
                    img.color = new Color(1, 0.5137255f, 0.5137255f, 0.3921569f);
                }

                StatisticsListEntries.Add(entry);
            }

        }, (error) =>
        {
            ModalHelper.WarningMessage("Get Leaderboard fail");
        });

    }

}
