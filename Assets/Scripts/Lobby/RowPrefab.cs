using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RowPrefab : MonoBehaviour
{
    public Text RankTxt;
    public Text NameTxt;
    public Text RateTxt;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(int Rank, string Name, int Rate)
    {
        RankTxt.text = Rank.ToString();
        NameTxt.text = Name;
        RateTxt.text = (Rate/100.0).ToString() + "%";
    }
}
