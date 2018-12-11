using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefsHelper {

	public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }
    public static int GetInt(string key)
    {
        return PlayerPrefs.GetInt(key);
    }
    public static void SetBool(string key, bool value)
    {
        SetInt(key, value ? 1 : 0);
    }
    public static bool GetBool(string key)
    {
        return (GetInt(key) == 1 ? true : false);
    }
    public static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }
    public static float GetFloat(string key, float defaultValue = 0)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }
    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
    }
    public static string GetString(string key)
    {
        return PlayerPrefs.GetString(key);
    }
    public static bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }
    public static void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.Save();
    }
    public static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
