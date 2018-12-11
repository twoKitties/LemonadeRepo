using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class BannerController : MonoBehaviour
{
    private string placementId = "banner";
    [SerializeField]
    private bool testMode;

    private void Start()
    {
#if UNITY_ANDROID || UNITY_EDITOR
        string gameID = GlobalConst.GAME_ID_GOOGLE_KEY;
#elif UNITY_IOS
    string gameID = GlobalConst.GAME_ID_APPLE_KEY;
#endif
        Advertisement.Initialize(gameID, testMode);
    }
    public void StartBanner()
    {
        if (AdsController.gameStartsCount % 2 == 1 && !AdsController.isDisabled)
            StartCoroutine(LaunchBanner());
    }
    private IEnumerator LaunchBanner()
    {
        while (!Advertisement.IsReady())
            yield return new WaitForSeconds(0.5f);

        Advertisement.Banner.Show(placementId);
    }
}
