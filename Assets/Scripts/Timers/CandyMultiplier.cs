using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class CandyMultiplier : TimerCore
{
    public static bool IsActive;
    [SerializeField]
    private Button[] buttons;
    protected override void Start()
    {
        timerUI = FindObjectOfType<TimerUIMultiplier>();

        if (PlayerPrefsHelper.HasKey(dateKey))
            savedTime = ulong.Parse(PlayerPrefsHelper.GetString(dateKey));

        if (!IsItemReady())
            ActivateButton(false);
        else
            ActivateButton(true);

    }
    protected override void Update()
    {
        if (!isCyclePassed)
        {
            if (IsItemReady())
            {
                ActivateButton(true);
                UILocalizationRefresher.Instance.CallOnLanguageChange();

                return;
            }
            // Set the timer
            ulong delta = (ulong)DateTime.Now.Ticks - savedTime;
            ulong m = delta / TimeSpan.TicksPerMillisecond;
            float secondsLeft = (msToWait - m) / 1000f;

            UpdateTime(secondsLeft);
        }

    }
    public override void SaveTimeStamp()
    {
        base.SaveTimeStamp();
        ActivateButton(false);
    }
    private void ActivateButton(bool state)
    {
        for (int i = 0; i < buttons.Length; i++)
            buttons[i].interactable = state;

        IsActive = !state;
    }
    protected override bool IsItemReady()
    {
        ulong delta = (ulong)DateTime.Now.Ticks - savedTime;
        ulong m = delta / TimeSpan.TicksPerMillisecond;
        float secondsLeft = (msToWait - m) / 1000f;

        //Debug.Log(secondsLeft);
        // Here you can send the 
        if (secondsLeft < 0)
            return true;

        return false;
    }
}
