using UnityEngine;
using UnityEngine.SceneManagement;

public static class CustomLevelLoader {

    /// <summary>
    /// Loads leves through Loading scene
    /// </summary>
    /// <param name="name">name of leve to load</param>
    public static void LoadLevelWithLoading(string name)
    {
        SceneManager.LoadScene("Loading");
        if (SceneManager.GetActiveScene().isLoaded)
        {
            SceneManager.LoadScene(name);
        }
        else
        {
            Debug.Log("Can't load the scene");
        }
    }

    public static void LoadLevel(string name)
    {
        SceneManager.LoadScene(name);
    }
}
