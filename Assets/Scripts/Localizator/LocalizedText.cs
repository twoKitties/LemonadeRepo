using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour {

    public string key;

    private Text text;

    private void Awake()
    {
        UILocalizationRefresher.OnLanguageChange += RefreshText;
    }

    private void Start()
    {
        text = GetComponent<Text>();
        text.text = LocalizationManager.Instance.GetLocalizedValue(key);
    }

    private void OnDestroy()
    {
        UILocalizationRefresher.OnLanguageChange -= RefreshText;
    }

    private void RefreshText()
    {
        if (LocalizationManager.Instance != null)
            text.text = LocalizationManager.Instance.GetLocalizedValue(key);
        else
            Debug.Log("not found: " + GetInstanceID() + " " + " object name " + gameObject.name);
    }
}
