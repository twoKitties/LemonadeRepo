using UnityEngine;
using UnityEngine.UI;

public class TimerUIMultiplier : TimerUI
{
    [SerializeField]
    private Text[] texts;

    public override void RefreshTime(string time)
    {
        if (texts.Length > 0)
            for (int i = 0; i < texts.Length; i++)
                texts[i].text = time;
    }
}
