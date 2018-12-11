using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataCleaner : MonoBehaviour
{
    [SerializeField]
    private Text text;
    private string showInfo;

    public void CleanAllData()
    {
        PlayerPrefsHelper.DeleteAll();
        SaveLoadController.Instance.DeleteData(ref showInfo);
        ShowInfo();
    }
    private void ShowInfo()
    {
        text.text = showInfo;
        StartCoroutine(TextDecay());
    }
    private IEnumerator TextDecay()
    {
        yield return new WaitForSeconds(1f);
        text.text = "";
    }
}
