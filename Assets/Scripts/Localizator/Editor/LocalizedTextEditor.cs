using UnityEngine;
using UnityEditor;
using System.IO;

public class LocalizedTextEditor : EditorWindow
{
    public LocalizationData localizationData;

    private Vector2 scrollPosition;

    [MenuItem("Window/Localized Text Editor")]
    private static void Init()
    {
        GetWindow(typeof(LocalizedTextEditor)).Show();
    }

    private void OnGUI()
    {
        if(localizationData != null)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty serializedProperty = serializedObject.FindProperty("localizationData");
            EditorGUILayout.PropertyField(serializedProperty, true);
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.BeginVertical();

            if (GUILayout.Button("Save Data"))
                SaveGameData();
        }

        if (GUILayout.Button("Load Data"))
            LoadGameData();

        if (GUILayout.Button("Create New Data"))
            CreateNewData();

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }

    private void LoadGameData()
    {
        string filePath = EditorUtility.OpenFilePanel("Select localization data file", Application.streamingAssetsPath, "json");

        if (!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);

            localizationData = JsonUtility.FromJson<LocalizationData>(dataAsJson);
        }
    }

    private void SaveGameData()
    {
        string filePath = EditorUtility.SaveFilePanel("Save localization data file", Application.streamingAssetsPath, "", "json");

        if (!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = JsonUtility.ToJson(localizationData);
            File.WriteAllText(filePath, dataAsJson);
        }
    }

    private void CreateNewData()
    {
        localizationData = new LocalizationData();
    }
}
