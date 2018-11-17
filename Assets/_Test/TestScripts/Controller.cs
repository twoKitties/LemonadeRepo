using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Controller : MonoBehaviour
{
    public static List<PrefabHolder> Holders = new List<PrefabHolder>();
    public static Controller Instance;
    public static System.Action OnLoadCall;

    //public PrefabHolder holder;

    private string fileName = "save.json";

    private void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(this);
    }


    //private void SaveInfo(bool[] state, int id, string key)
    //{
    //    SaveData data = new SaveData(state, id, key);
    //    string json = JsonUtility.ToJson(data);

    //    string path = Path.Combine(Application.streamingAssetsPath, fileName);

    //    if (File.Exists(path))
    //        File.Delete(path);

    //    File.WriteAllText(path, json);

    //    Debug.Log("Saving done");
    //}
    public void SaveAllData()
    {
        SaveItems save = new SaveItems();

        foreach (var item in Holders)
        {
            SaveData data = new SaveData(item.States, item.CurrentGrade, item.Key);
            save.Items.Add(data);
        }
        string json = JsonUtility.ToJson(save);
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        if (File.Exists(path))
            File.Delete(path);
        File.WriteAllText(path, json);
        Debug.Log("saving done");
    }
    public SaveData LoadAllData(string key)
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        if (File.Exists(path))
        {
            string dataAsJson = File.ReadAllText(path);
            SaveItems save = JsonUtility.FromJson<SaveItems>(dataAsJson);
            foreach (var item in save.Items)
            {
                if(key == item.Key)
                {
                    return item;
                }
            }
            Debug.Log("Data not found");
            return null;
        }
        else
        {
            Debug.Log("File doesn't exist");
            return null;
        }
    }
    public void CallLoadData()
    {
        if (OnLoadCall != null)
            OnLoadCall();
    }
    //private void LoadInfo()
    //{
    //    string path = Path.Combine(Application.streamingAssetsPath, fileName);
    //    if (File.Exists(path))
    //    {
    //        string dataAsJson = File.ReadAllText(path);
    //        SaveData data = JsonUtility.FromJson<SaveData>(dataAsJson);
    //        holder.CurrentGrade = data.ID;
    //        holder.States = data.State;
            
    //        Debug.Log("Data loaded");
    //    }
    //    else
    //    {
    //        Debug.LogError("Cannot find file!");
    //    }
    //}
}
