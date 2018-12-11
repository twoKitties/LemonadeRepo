using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpRating : MonoBehaviour
{
    [SerializeField]
    private GameObject ratingWindow;
    [SerializeField]
    private Button laterButton;
    [SerializeField]
    private Image[] rateButtons;
    private WaitForSeconds wait = new WaitForSeconds(0.5f);

    private bool isRated
    {
        get { return PlayerPrefsHelper.GetBool(GlobalConst.APP_RATED_KEY); }
        set { PlayerPrefsHelper.SetBool(GlobalConst.APP_RATED_KEY, value); }
    }
    private void Start()
    {
        if (!isRated && (PlayerPrefsHelper.GetInt(GlobalConst.LAUNCHES_COUNT_KEY) % 3 == 0) && HasConnection() && AdsController.gameStartsCount > 1)
            OpenRatingWindow();
    }
    private void OpenRatingWindow()
    {
        ratingWindow.SetActive(true);
        for (int i = 0; i < rateButtons.Length; i++)
            rateButtons[i].color = Color.black;
    }
    public void RateNow()
    {
        string url = "";
#if UNITY_EDITOR || UNITY_STANDALONE
        url = GlobalConst.ANDROID_RATING_URL_KEY;
#elif UNITY_ANDROID
        url = GlobalConst.ANDROID_RATING_URL_KEY;
#elif UNITY_IOS
#endif
        isRated = true;
        Application.OpenURL(url);
        StartCoroutine(CloseWindow());
    }
    public void RateLater()
    {
        ratingWindow.SetActive(false);
    }
    public void RateNever()
    {
        isRated = true;
        StartCoroutine(CloseWindow());
    }
    public void GlowButton(int buttonNumber)
    {
        for (int i = 0; i < rateButtons.Length; i++)
        {
            if (i <= buttonNumber)
                rateButtons[i].color = Color.white;
        }
    }
    private IEnumerator CloseWindow()
    {
        laterButton.interactable = false;
        yield return wait;
        ratingWindow.SetActive(false);
    }
    private bool HasConnection()
    {
        if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ||
           Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            return true;
        else
            return false;
    }
}
