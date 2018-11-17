using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    [SerializeField]
    private Text loadingText;
    [SerializeField]
    private Slider loadingSlider;
    [SerializeField]
    private Image fillImage;

    private float loadTime;
    private float currentLoadTime;

    [SerializeField]
    private GameObject background;
    [SerializeField]
    private GameObject canvas;

    [SerializeField]
    private GameObject logo;
    private SpriteRenderer logoRenderer;


    private int launchCount
    {
        get
        {
            return PlayerPrefsHelper.GetInt(GlobalConst.LAUNCHES_COUNT_KEY);
        }
        set
        {
            PlayerPrefsHelper.SetInt(GlobalConst.LAUNCHES_COUNT_KEY, value);
        }
    }
    private int languageIndex
    {
        get
        {
            return PlayerPrefsHelper.GetInt(GlobalConst.CURRENT_LANGUAGE);
        }
        set
        {
            PlayerPrefsHelper.SetInt(GlobalConst.CURRENT_LANGUAGE, value);
        }
    }

    private void Start()
    {
        launchCount++;
        StartCoroutine(FakeLoad());
    }    

    private IEnumerator InitLogo()
    {
        background.SetActive(false);
        canvas.SetActive(false);
        logo.SetActive(true);
        logoRenderer = logo.GetComponent<SpriteRenderer>();
        Color alpha = logoRenderer.color;

        float duration = 3;
        float currentTime = 0;
        float percent = 0;
        float time = Time.deltaTime;

        do
        {
            yield return new WaitForEndOfFrame();
            currentTime += time;
            percent = currentTime / duration;
            float a = percent;
            alpha.a = a;
            logoRenderer.color = alpha;
        }
        while (percent <= 1);
        
        yield return new WaitForSeconds(0.5f);

        do
        {
            yield return new WaitForEndOfFrame();
            currentTime -= time;
            percent = currentTime / duration;
            float a = percent;
            alpha.a = a;
            logoRenderer.color = alpha;
        }
        while (percent >= 0);

        background.SetActive(true);
        canvas.SetActive(true);
        logo.SetActive(false);

        StartCoroutine(FakeLoad());
    }

    private IEnumerator FakeLoad()
    {
        float time = Time.deltaTime;
        loadTime = Random.Range(1.5f, 3);
        float percent = 0;

        do
        {
            yield return new WaitForEndOfFrame();
            currentLoadTime += time;
            percent = currentLoadTime / loadTime;
            loadingSlider.value = percent;
        }
        while (percent <= 1);

        CustomLevelLoader.LoadLevel("Game");
    }
}
