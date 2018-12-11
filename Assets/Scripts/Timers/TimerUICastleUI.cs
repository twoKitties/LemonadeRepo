using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUICastleUI : TimerUI
{
    [SerializeField]
    private Text[] texts;

    public override void RefreshTime(string time)
    {
        for (int i = 0; i < texts.Length; i++)
            texts[i].text = time;
    }
}
