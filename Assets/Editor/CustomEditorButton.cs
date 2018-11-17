using UnityEngine;
using UnityEditor;

public class CustomEditorButton : MonoBehaviour {

    [MenuItem("Custom/ClearPrefs", false)]
    static void ClearPrefs()
    {
        PlayerPrefsHelper.DeleteAll();
        Debug.Log("Prefs deleted");
    }
    [MenuItem("Custom/ResetLemoncity", false)]
    static void ResetLemoncity()
    {
    }
}
