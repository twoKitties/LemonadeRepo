using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class SpriteSwaper : EditorWindow
{
    private AnimationClip clip;
    private AnimationCurve curve;
    private Texture2D oldTex;
    private Texture2D tex;
    [SerializeField]
    private Sprite[] sprites;

    [MenuItem("Custom/Sprite Swaper")]
    private static void Init()
    {
        // Get existing open window, if none make new one
        GetWindow(typeof(SpriteSwaper));
    }
    private void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Label("Select animation to transpose");
        // Gets animation clip to transpose
        clip = (AnimationClip)EditorGUILayout.ObjectField(clip, typeof(AnimationClip), true);
        GUILayout.Label("Select sprite sheet");
        // Gets new sprite texture
        tex = (Texture2D)EditorGUILayout.ObjectField(tex, typeof(Texture2D), true);

        if(tex != oldTex)
        {
            // Grabs the texture asset and fills the sprite array with the sprites
            // Serializes it and displays it so you can see the sprites in the array
            string spriteSheet = AssetDatabase.GetAssetPath(tex);
            sprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheet).OfType<Sprite>().ToArray();
            oldTex = tex;
        }
        // Displays array in window
        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty spritesProp = so.FindProperty("sprites");
        EditorGUILayout.PropertyField(spritesProp, true);
        // Button that creates new animation
        if (GUILayout.Button("Create new animation"))
            CreateNewAnimation();

        GUILayout.EndVertical();
    }
    private void CreateNewAnimation()
    {
        string[] splitName;
        AnimationClip clonedClip;
        // Check to see if folder you wish to drop them in exists. 
        // REFERENCE YOUR FOLDER STRUCTURE HERE
        // OTHERWISE IT WILL CREATE NEW ONE WITH SPECIFIED NAME
        string path = "Assets/GameResources/Animations/GameTileBursts_new";
        if(!AssetDatabase.IsValidFolder(path + tex.name))
            AssetDatabase.CreateFolder(path, tex.name);

        // Copies original animation clip and assignes it to a retrieves the copied animation clip
        AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(clip), path + tex.name + "/" + tex.name + clip.name + ".anim");
        clonedClip = (AnimationClip)AssetDatabase.LoadAssetAtPath(path + tex.name + "/" + tex.name + clip.name + ".anim", typeof(AnimationClip));

        // CHECKED WITH JUST SPRITE ANIMATIONS!
        // Gets the binding for object curves
        foreach (var binding in AnimationUtility.GetObjectReferenceCurveBindings(clonedClip))
        {
            // Gets the keyframes and assignes them to an array
            ObjectReferenceKeyframe[] keyframes = AnimationUtility.GetObjectReferenceCurve(clonedClip, binding);
            for (int i = 0; i < keyframes.Length; i++)
            {
                // Get the "_#" from the original sprite  ImageA_01 gets 01
                splitName = keyframes[i].value.name.Split('_');
                // Uses LINQ to capture the sprite on from the sprite array we created earlier and sets the keyframe value to the new sprite

            }
        }
    }
}
