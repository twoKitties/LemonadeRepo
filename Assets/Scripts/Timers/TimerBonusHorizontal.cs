using UnityEngine;
using System;
using LemonadeStore;

public class TimerBonusHorizontal : TimerCore
{
    [SerializeField]
    private Storage storage;
    private BoardController boardController;
    private int bonusesAmount
    {
        get { return PlayerPrefsHelper.GetInt("hBonusesAmount"); }
        set { PlayerPrefsHelper.SetInt("hBonusesAmount", value); }
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

    private void Awake()
    {
        BonusButtons.OnRowBonusActivation += SaveTimeStamp;
    }
    private void OnDestroy()
    {
        BonusButtons.OnRowBonusActivation -= SaveTimeStamp;
    }
    protected override void Start()
    {
        timerUI = GetComponent<TimerUIHorizontal>();
        boardController = FindObjectOfType<BoardController>();
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
        else
            isCyclePassed = true;

        timerUI.RefreshAmount(GetBonusesAmount());
    }
    protected override void Update()
    {
        if (!isCyclePassed && storage.MaxUnlockedGrade > 0)
        {
            if (IsItemReady())
            {
                //isCyclePassed = true;
                cycleCounter--;
                if (bonusesAmount < 3)
                    bonusesAmount++;
                //timerUI.RefreshTime(GetBonusesAmount());
                timerUI.RefreshAmount(GetBonusesAmount());
                return;
            }
            // Set the timer
            ulong delta = (ulong)DateTime.Now.Ticks - savedTime;
            ulong m = delta / TimeSpan.TicksPerMillisecond;
            float secondsLeft = (msToWait - m) / 1000f;
            UpdateTime(secondsLeft);
            timerUI.RefreshAmount(GetBonusesAmount());
        }
    }
    protected override void UpdateTime(float timeLeft)
    {
        base.UpdateTime(timeLeft);
        //string r = "";
        //// Minutes
        //r += ((int)timeLeft / 60).ToString("00") + " ";
        //// Seconds
        //r += (timeLeft % 60).ToString("00");
        //timerUI.RefreshUI(r);
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
    private string GetBonusesAmount()
    {
        return bonusesAmount.ToString();
    }
    public void ClaimBonus()
    {
        boardController.RowBonusAmount += bonusesAmount;
        bonusesAmount = 0;
        cycleCounter = 0;
        timerUI.RefreshAmount(GetBonusesAmount());
    }
}
