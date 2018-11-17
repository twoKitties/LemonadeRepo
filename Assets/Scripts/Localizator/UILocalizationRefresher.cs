using UnityEngine;
using System.Collections;

public class UILocalizationRefresher : MonoBehaviour {

    public static UILocalizationRefresher Instance;
    public static event System.Action OnLanguageChange;

    private void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != null)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator WaitTilIsReady()
    {
        while (!LocalizationManager.Instance.GetIsReady())
        {
            yield return null;
        }

        if (OnLanguageChange != null)
            OnLanguageChange();
    }
}
