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

    private void Start()
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
        foreach (var item in Storage.GradeItems)
        {
            GradeInfo gradeInfo = item.GetGradeInfo();
            Item i = new Item(gradeInfo.Key, gradeInfo.GradeID, gradeInfo.AreBought);
            data.Items.Add(i);
        }
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
}
