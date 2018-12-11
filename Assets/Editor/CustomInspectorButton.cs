using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BonusButtons))]
public class CustomInspectorButton : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();


    }
}
