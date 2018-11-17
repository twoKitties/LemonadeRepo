using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;

public class AdsController : MonoBehaviour
{
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
    private bool isDisabled
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



#if UNITY_ANDROID || UNITY_IOS
    public void StartAds()
    {
        gameStartsCount++;
        Debug.Log("counter" + gameStartsCount);
        if (gameStartsCount % 2 == 0 && !isDisabled)
        {
            Advertisement.Initialize(GlobalConst.GAME_ID_GOOGLE_KEY, true);
            StartCoroutine(LaunchAds());
        }
    }

    private IEnumerator LaunchAds()
    {
        while (!Advertisement.IsReady())
            yield return null;
        Advertisement.Show();
    }

    public void DisableAds()
    {
        isDisabled = true;
    }
#endif
}
