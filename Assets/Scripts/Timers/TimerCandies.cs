using UnityEngine;
using LemonadeStore;
using System;

public class TimerCandies : TimerCore
{
    private Storage storage;
    private int candiesAmount
    {
        get { return PlayerPrefsHelper.GetInt("CandiesAmount"); }
        set { PlayerPrefsHelper.SetInt("CandiesAmount", value); }
    }
    [Header("Time setters")]
    [SerializeField]
    private float msToWaitLevel1;
    [SerializeField]
    private float msToWaitLevel2;
    [SerializeField]
    private float msToWaitLevel3;
    [SerializeField]
    private float msToWaitLevel4;

    protected override void Start()
    {
        timerUI = FindObjectOfType<TimerUICastleUI>();
        storage = GetComponent<Storage>();
        if (storage.MaxUnlockedGrade == 0)
            return;
        else if (storage.MaxUnlockedGrade == 1)
            msToWait = msToWaitLevel1;
        else if (storage.MaxUnlockedGrade == 2)
            msToWait = msToWaitLevel2;
        else if (storage.MaxUnlockedGrade == 3)
            msToWait = msToWaitLevel3;
        else if (storage.MaxUnlockedGrade == 4)
            msToWait = msToWaitLevel4;


        if (PlayerPrefsHelper.HasKey(dateKey))
            savedTime = ulong.Parse(PlayerPrefsHelper.GetString(dateKey));

        if (!IsItemReady())
            isCyclePassed = false;

        candiesAmount += Mathf.Abs(cycleCounter);
        if (candiesAmount > 1500)
            candiesAmount = 1500;
        timerUI.RefreshTime(GetCandiesAmount());
    }
    protected override void Update()
    {
        if (!isCyclePassed && storage.MaxUnlockedGrade > 0)
        {
            if (IsItemReady())
            {
                //isCyclePassed = true;
                cycleCounter--;
                if (candiesAmount < 1500)
                    candiesAmount++;
                timerUI.RefreshTime(GetCandiesAmount());
                return;
            }
            // Set the timer
            ulong delta = (ulong)DateTime.Now.Ticks - savedTime;
            ulong m = delta / TimeSpan.TicksPerMillisecond;
            float secondsLeft = (msToWait - m) / 1000f;
        }
    }
    protected override bool IsItemReady()
    {
        ulong delta = (ulong)DateTime.Now.Ticks - savedTime;
        ulong m = delta / TimeSpan.TicksPerMillisecond;
        float secondsLeft = (msToWait - m) / 1000f;

        //Debug.Log(secondsLeft);
        // Here you can send the 
        if (secondsLeft < 0)
        {
            PlayerPrefsHelper.DeleteKey(dateKey);
            cycleCounter += (int)secondsLeft % (int)msToWait;
            SaveTimeStamp();
            return true;
        }
        return false;
    }
    private string GetCandiesAmount()
    {
        return candiesAmount.ToString();
    }
    public void ClaimCandies()
    {
        FindObjectOfType<UpgradeWorker>().Points += candiesAmount;
        candiesAmount = 0;
        cycleCounter = 0;
        timerUI.RefreshTime(GetCandiesAmount());
    }
}
