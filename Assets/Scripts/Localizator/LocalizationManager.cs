using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    private Dictionary<string, string> localizedText;
    private bool isReady = false;
    private string missingTextString = "localized text not found";

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if( Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        int launches = PlayerPrefsHelper.GetInt(GlobalConst.LAUNCHES_COUNT_KEY);
        if (launches < 1)
        {
            if (Application.systemLanguage == SystemLanguage.English)
                LoadLocalizedText("localizedText_en.json");
            else if (Application.systemLanguage == SystemLanguage.Russian)
                LoadLocalizedText("localizedText_ru.json");
        }
        else if(launches >= 1)
        {
            int languageIndex = PlayerPrefsHelper.GetInt(GlobalConst.CURRENT_LANGUAGE);
            if (languageIndex == 0)
                LoadLocalizedText("localizedText_ru.json");
            else if (languageIndex == 1)
                LoadLocalizedText("localizedText_en.json");
        }

        Debug.Log("launches " + launches);
    }

    private void LoadLocalizedText(string fileName)
    {
        localizedText = new Dictionary<string, string>();

        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            for (int i = 0; i < loadedData.items.Length; i++)
            {
                localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
            }
            Debug.Log("Data loaded, dictionary contains: " + localizedText.Count + " entries");
        }
        else
        {
            Debug.LogError("Cannot find file!");
        }

#elif UNITY_ANDROID
        string text = "";
        if (filePath.Contains("://"))
        {
            WWW www = new WWW("jar:file://" + Application.dataPath + "!/assets/" + fileName);
            while (!www.isDone) { }
            text = www.text;
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(text);

            for (int i = 0; i < loadedData.items.Length; i++)
            {
                localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
            }
        }
#endif
        isReady = true;
    }

    public bool GetIsReady()
    {
        return isReady;
    }

    /// <summary>
    /// Select language
    /// </summary>
    /// <param name="languageIndex">0 for russian, 1 for english</param>
    public void ChooseLanguage(int languageIndex)
    {
        isReady = false;
        switch (languageIndex)
        {
            case 0:
                LoadLocalizedText("localizedText_ru.json");
                break;
            case 1:
                LoadLocalizedText("localizedText_en.json");
                break;
        }
        StartCoroutine(UILocalizationRefresher.Instance.WaitTilIsReady());
    }

    public string GetLocalizedValue(string key)
    {
        string result = missingTextString;
        if (localizedText.ContainsKey(key))
        {
            result = localizedText[key];
        }

        return result;
    }
}
