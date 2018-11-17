using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageSwitch : MonoBehaviour {

    public int languageIndex
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
    [SerializeField]
    private GameObject[] objects;

    private void Start()
    {
        //if (Application.systemLanguage == SystemLanguage.Russian)
        //    languageIndex = 0;
        //else if (Application.systemLanguage == SystemLanguage.English)
        //    languageIndex = 1;

        for (int i = 0; i < objects.Length; i++)
        {
            if (i == languageIndex)
                objects[i].SetActive(true);
            else
                objects[i].SetActive(false);
        }
    }

    public void SwitchManually()
    {
        languageIndex++;

        if (languageIndex >= objects.Length)
            languageIndex = 0;

        SetLanguage();
    }

    private void SetLanguage()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            if (i == languageIndex)
                objects[i].SetActive(true);
            else
                objects[i].SetActive(false);
        }

        LocalizationManager.Instance.ChooseLanguage(languageIndex);
    }
}
