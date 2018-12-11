using UnityEngine;
using System;

public class TimerCore : MonoBehaviour
{
    [SerializeField]
    protected float msToWait = 5000f;
    [SerializeField]
    protected int cycleCounter;
    [SerializeField]
    protected string dateKey = "DateKey";
    protected ulong savedTime;
    protected bool isCyclePassed;

    protected TimerUI timerUI;

    protected virtual void Start()
    {
        timerUI = FindObjectOfType<TimerUI>();

        if (PlayerPrefsHelper.HasKey(dateKey))
            savedTime = ulong.Parse(PlayerPrefsHelper.GetString(dateKey));

        if (!IsItemReady() && cycleCounter < 3)
            isCyclePassed = false;
        else
            isCyclePassed = true;
    }
    public virtual void SaveTimeStamp()
    {
        savedTime = (ulong)DateTime.Now.Ticks;
        PlayerPrefsHelper.SetString(dateKey, savedTime.ToString());
        isCyclePassed = false;
    }
    protected virtual void Update()
    {
        if (!isCyclePassed)
        {
            if (IsItemReady())
            {
                //isCyclePassed = true;
                timerUI.RefreshTime("");
                return;
            }
            // Set the timer
            ulong delta = (ulong)DateTime.Now.Ticks - savedTime;
            ulong m = delta / TimeSpan.TicksPerMillisecond;
            float secondsLeft = (msToWait - m) / 1000f;

            UpdateTime(secondsLeft);
        }
    }
    protected virtual void UpdateTime(float timeLeft)
    {
        string r = "";
        // Hours
        r += ((int)timeLeft / 3600).ToString() + " ";
        timeLeft -= ((int)timeLeft/ 3600) * 3600;
        // Minutes
        r += ((int)timeLeft / 60).ToString("00") + " ";
        // Seconds
        r += (timeLeft % 60).ToString("00");
        timerUI.RefreshTime(r);
    }
    protected virtual bool IsItemReady()
    {
        ulong delta = (ulong)DateTime.Now.Ticks - savedTime;
        ulong m = delta / TimeSpan.TicksPerMillisecond;
        float secondsLeft = (msToWait - m) / 1000f;

        Debug.Log(secondsLeft);
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
}
