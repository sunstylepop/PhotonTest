using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackJackTimer : MonoBehaviour
{
    bool counting = false;
    float timerTotal = 0f;
    Text text;

    private void Start()
    {
        text = gameObject.GetComponent<Text>();
    }

    private void Update()
    {
        if (counting)
        {
            timerTotal -= Time.deltaTime;
            text.text = timerTotal.ToString();

            if (timerTotal <= 0)
            {
                counting = false;
                text.gameObject.SetActive(false);
            }
        }
    }

    public void StartCount(int seconds)
    {
        counting = true;
        if (text != null)
            text.gameObject.SetActive(true);
        timerTotal = seconds;
    }
}
