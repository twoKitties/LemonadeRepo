using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public bool[] State;
    public int ID;
    public string Key;

    public SaveData(bool[] state, int id, string key)
    {
        State = state;
        ID = id;
        Key = key;
    }
}
[System.Serializable]
public class SaveItems
{
    public List<SaveData> Items = new List<SaveData>();
}
