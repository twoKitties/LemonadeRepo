using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using LemonadeStore;

public class RouletteInterface : MonoBehaviour
{
    [SerializeField]
    private RewardWindow rewardWindow;
    [SerializeField]
    private RewardList listOfRewards;
    #region Execute Spinning
    [SerializeField]
    private Button spinWithBetButton;
    [SerializeField]
    private Text costText;
    [SerializeField]
    private Text spinButtonText;
    [SerializeField]
    private Button spinWithAdsButton;
    [SerializeField]
    private int cost;
    #endregion
    [SerializeField]
    private GameObject wheel;
    [SerializeField]
    private AnimationCurve[] animationCurves;

    private bool isSpinning;
    private float anglePerItem;
    private int randomTime;
    private int itemNumber;
    private int[] prize = { 0, 1, 2, 3, 4, 5, 6};

    private LemonadeStore.UpgradeWorker upgradeWorker;
    public static System.Action OnAdsFinished;

    private void Start()
    {
        upgradeWorker = FindObjectOfType<LemonadeStore.UpgradeWorker>();
        OnAdsFinished += DoRotation;
    }
    private void OnDestroy()
    {
        OnAdsFinished -= DoRotation;
    }
    private int GetRandomItem()
    {
        float r = Random.value;
        return -1;
    }
    public void Rotate()
    {
        if (upgradeWorker == null)
        {
            Debug.Log("upgradeWorker is null, can't execute sequence");
            return;
        }

        int result = upgradeWorker.Points - cost;
        Debug.Log("Points " + upgradeWorker.Points);
        if (result < 0)
        {
            var defColor = Color.white;
            var blinkColor = Color.red;
            var blinker = new Blinker(defColor, blinkColor, costText);
            StartCoroutine(blinker.Blink());
            var blinker2 = new Blinker(defColor, blinkColor, spinButtonText);
            StartCoroutine(blinker2.Blink());
        }
        else
        {
            upgradeWorker.Points -= cost;
            if (PointsCount.OnPointsChange != null)
                PointsCount.OnPointsChange();
            DoRotation();
        }
    }
    private void DoRotation()
    {
        isSpinning = false;
        anglePerItem = 360 / prize.Length;

        randomTime = Random.Range(4, 8);

        itemNumber = GetItemNumber();
        float maxAngle = 360 * randomTime + (itemNumber * anglePerItem);

        StartCoroutine(SpinTheWheel(randomTime, maxAngle));
    }
    private int GetItemNumber()
    {
        int result = 0;
        float r = Random.value;
        if (r >= 0.2f && r < 0.9f)
        {
            result = Random.Range(0, 2) == 0 ? 2 : 4;
        }
        else if (r >= 0.9f && r < 0.95f)
            result = 5;
        else if (r >= 0.95f && r < 0.99f)
            result = 6;
        else if(r > 0.99f)
        {
            int subrandom = Random.Range(0, 3);
            if (subrandom == 0)
                result = 0;
            else if (subrandom == 1)
                result = 1;
            else if (subrandom == 2)
                result = 3;
        }
        return result;
    }
    private IEnumerator SpinTheWheel(float time, float maxAngle)
    {
        isSpinning = true;
        spinWithBetButton.interactable = false;
        spinWithAdsButton.interactable = false;

        float timer = 0f;
        float startAngle = wheel.transform.eulerAngles.z;
        maxAngle = maxAngle - startAngle;

        int animationCurveNumber = Random.Range(0, animationCurves.Length);
        Debug.Log("Animation Curve # " + animationCurveNumber);

        while(timer < time)
        {
            float angle = maxAngle * animationCurves[animationCurveNumber].Evaluate(timer / time);
            Debug.Log("Angle " + angle);
            wheel.transform.eulerAngles = new Vector3(0, 0, angle + startAngle);
            timer += Time.deltaTime;
            //yield return null;
            yield return 0;
        }
        wheel.transform.eulerAngles = new Vector3(0, 0, maxAngle + startAngle);
        isSpinning = false;

        spinWithBetButton.interactable = true;
        spinWithAdsButton.interactable = true;
        Debug.Log("Prize: " + prize[itemNumber]);

        yield return new WaitForSeconds(0.2f);

        if(itemNumber == 2 || itemNumber == 4 || itemNumber == 5 || itemNumber == 6 || itemNumber == 1 || itemNumber == 0 || itemNumber == 3)
        {
            int result = 0;
            switch (itemNumber)
            {
                case 0:
                    ShowDecorationRewardItem();
                    break;
                case 2:
                    result = 0;
                    ShowRewardWindow(result);
                    break;
                case 3:
                    // Rewrite
                    ShowHomeDecorRewardItem();
                    break;
                case 4:
                    result = 0;
                    ShowRewardWindow(result);
                    break;
                case 5:
                    result = 1;
                    ShowRewardWindow(result);
                    break;
                case 6:
                    result = 2;
                    ShowRewardWindow(result);
                    break;
                case 1:
                    result = 3;
                    ShowRewardWindow(result);
                    break;
            }
            
        }
    }
    private void ShowDecorationRewardItem()
    {
        DecorationItem decorationItem = new DecorationItem();
        var shuffledList = DecorationStorage.AllDecorationStorages.OrderBy(x => Random.value).ToList();
        foreach (var item in shuffledList)
        {
            if (item.HasLockedDecoration())
            {
                decorationItem = item.GetItem();
                decorationItem.IsInitialized = true;
                item.UnlockDecoration(decorationItem.ID);
                string description = (PlayerPrefsHelper.GetInt(GlobalConst.CURRENT_LANGUAGE) == 0) ? "Украшение!" : "Decoration!";
                rewardWindow.SetRewardItemData(decorationItem.Sprite, description, 0);
                break;
            }
        }
        if (!decorationItem.IsInitialized)
        {
            ShowRewardWindow(0);
            return;
        }
        SaveLoadController.Instance.Save();
        rewardWindow.gameObject.SetActive(true);
    }

    // REWRITE ALL THIS STUFF, DECORATIONS FOR FUCKING HOMES SHOULD BE GAMBLED INSTEAD OF UPGRADES
    private void ShowHomeDecorRewardItem()
    {
        HomeDecorItem homeDecorItem = new HomeDecorItem(-1);
        var shuffledList = HomeDecoration.AllHomeDecorations.OrderBy(x => Random.value).ToList();
        foreach (var item in shuffledList)
        {
            homeDecorItem = item.GetItem();
            homeDecorItem.IsInitizlized = true;
            item.UnlockDecoration(homeDecorItem.ID);
            rewardWindow.SetRewardItemData(homeDecorItem.Sprite, homeDecorItem.Description, 0);
            break;
        }
        if (!homeDecorItem.IsInitizlized)
        {
            ShowRewardWindow(0);
            return;
        }
        SaveLoadController.Instance.Save();
        rewardWindow.gameObject.SetActive(true);
    }
    private void ShowRewardWindow(int prizeID)
    {
        rewardWindow.gameObject.SetActive(true);
        var reward = listOfRewards.candyRewards[prizeID];
        string description = (PlayerPrefsHelper.GetInt(GlobalConst.CURRENT_LANGUAGE) == 0) ? reward.DescriptionRu : reward.DescriptionEn;
        rewardWindow.SetRewardItemData(reward.Sprite, description, reward.Amount);
    }
}
