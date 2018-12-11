using UnityEngine;
using UnityEngine.Monetization;
//using UnityEngine.Advertisements;
using System.Collections;

public class AdsController : MonoBehaviour
{
    [SerializeField]
    private GameObject rewardWindow;

    public static int gameStartsCount
    {
        get
        {
            return PlayerPrefsHelper.GetInt(GlobalConst.GAME_STARTS_COUNT_KEY);
        }
        set
        {
            PlayerPrefsHelper.SetInt(GlobalConst.GAME_STARTS_COUNT_KEY, value);
        }
    }
    public static bool isDisabled
    {
        get
        {
            return PlayerPrefsHelper.GetBool(GlobalConst.ADS_DISABLED_KEY);
        }
        set
        {
            PlayerPrefsHelper.SetBool(GlobalConst.ADS_DISABLED_KEY, value);
        }
    }

    private string rewardedPlacementId = "rewardedVideo";
    private string nonrewardedPlacementId = "video";
    private string gameId;
    [SerializeField]
    private bool testMode;

    private void Start()
    {
#if UNITY_ANDROID || UNITY_EDITOR
        gameId = GlobalConst.GAME_ID_GOOGLE_KEY;
#elif UNITY_IOS
        gameId = GlobalConst.GAME_ID_APPLE_KEY;
#endif
        Monetization.Initialize(gameId, testMode);
    }

    public void ShowWindow(GameObject window)
    {
        window.SetActive(true);
    }

#if UNITY_ANDROID || UNITY_IOS
    public void StartAds()
    {
        gameStartsCount++;
        Debug.Log("counter" + gameStartsCount);

        if (gameStartsCount % 2 == 0 && !isDisabled)
        {
            if (!Monetization.isInitialized)
                Monetization.Initialize(gameId, testMode);
            StartCoroutine(LaunchAds());
        }
    }

    private IEnumerator LaunchAds()
    {
        while (!Monetization.IsReady(nonrewardedPlacementId))
            yield return new WaitForSeconds(0.25f);

        ShowAdPlacementContent ads = null;
        ads = Monetization.GetPlacementContent(nonrewardedPlacementId) as ShowAdPlacementContent;

        if (ads != null)
            ads.Show();
    }
    public void DisableAds()
    {
        isDisabled = true;
    }
    public void ShowRewardVideo()
    {
        if (!Monetization.isInitialized)
            Monetization.Initialize(gameId, false);
        // Add request if user wants to watch adds
        StartCoroutine(LaunchRewardAds());
    }
    public void ShowWheelRewardVideo()
    {
        if (!Monetization.isInitialized)
            Monetization.Initialize(gameId, testMode);
        StartCoroutine(LaunchWheelAds());
    }
    private IEnumerator LaunchWheelAds()
    {
        while (!Monetization.IsReady(rewardedPlacementId))
            yield return null;

        ShowAdPlacementContent ads = null;
        ads = Monetization.GetPlacementContent(rewardedPlacementId) as ShowAdPlacementContent;

        if (ads != null)
            ads.Show(AdsRouletteFinished);
    }
    private IEnumerator LaunchRewardAds()
    {
        while (!Monetization.IsReady(rewardedPlacementId))
            yield return null;

        ShowAdPlacementContent ads = null;
        ads = Monetization.GetPlacementContent(rewardedPlacementId) as ShowAdPlacementContent;

        if (ads != null)
            ads.Show(RewardAdsFinished);
    }
    private void AdsRouletteFinished(ShowResult result)
    {
        if(result == ShowResult.Finished)
        {
            if (RouletteInterface.OnAdsFinished != null)
                RouletteInterface.OnAdsFinished();
        }
    }
    private void RewardAdsFinished(ShowResult result)
    {
        if (result == ShowResult.Finished)
        {
            Debug.Log("Reward player");
            rewardWindow.SetActive(true);
        }
    }
#endif
}
