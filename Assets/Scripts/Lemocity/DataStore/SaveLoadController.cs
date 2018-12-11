using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataStorage;
using LemonadeStore;
using System.IO;

public class SaveLoadController : MonoBehaviour
{
    public static SaveLoadController Instance;
    [SerializeField]
    private string fileName;
    public string FileName { get { return fileName; } }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(this);
    }
    public void Save()
    {
        Data data = new Data();
        foreach (var item in Storage.AllStorages)
        {
            GradeInfo gradeInfo = item.GetGradeInfo();
            Item i = new Item(gradeInfo.Key, gradeInfo.GradeID, gradeInfo.AreBought);
            data.Items.Add(i);
        }
        // Test part for decoration items storage
        foreach (var item in DecorationStorage.AllDecorationStorages)
        {
            DecorationInfo info = item.GetDecorationStorageInfo();
            Item i = new Item(info.Key, info.CurrentDecoration, info.UnlockedDecorations);
            data.Items.Add(i);
        }
        foreach(var item in HomeDecoration.AllHomeDecorations)
        {
            DecorationInfo info = item.GetHomeDecorationInfo();
            Item i = new Item(info.Key, info.CurrentDecoration, info.UnlockedDecorations);
            data.Items.Add(i);
        }
        // End of test
        string json = JsonUtility.ToJson(data);
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
#elif UNITY_ANDROID
        string path = Path.Combine(Application.persistentDataPath, fileName);
#endif

        // TRY USING Application.persistentDataPath

        if (File.Exists(path))
            File.Delete(path);
        File.WriteAllText(path, json);
        Debug.Log("Saving Done");
    }
    public Item Load(string key)
    {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
#elif UNITY_ANDROID
        string path = Path.Combine(Application.persistentDataPath, fileName);
#endif
        if (File.Exists(path))
        {
            string dataAsJson = File.ReadAllText(path);
            Data data = JsonUtility.FromJson<Data>(dataAsJson);
            foreach (var item in data.Items)
            {
                if (key == item.Key)
                    return item;
            }
            Debug.Log("Data not found");
            return null;
        }
        else
        {
            Debug.Log("File not found");
            return null;
        }
    }
    public void DeleteData(ref string info)
    {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
#elif UNITY_ANDROID
        string path = Path.Combine(Application.persistentDataPath, fileName);
#endif
        // TRY USING Application.persistentDataPath
        if (File.Exists(path))
        {
            File.Delete(path);
            info = "Data deleted";
        }
        else
            info = "Data not found";
    }
}
