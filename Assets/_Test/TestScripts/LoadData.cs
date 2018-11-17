using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadData : MonoBehaviour
{
    [SerializeField]
    private PrefabHolder data;

    private void Start()
    {
        Controller.Holders.Add(data);
        Controller.OnLoadCall += GetData;
    }
    private void OnDestroy()
    {
        Controller.OnLoadCall -= GetData;
    }
    private void GetData()
    {
        SaveData dataToLoad = Controller.Instance.LoadAllData(data.Key);
        data.States = dataToLoad.State;
        data.CurrentGrade = dataToLoad.ID;
    }
}
