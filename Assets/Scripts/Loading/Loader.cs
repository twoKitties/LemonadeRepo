using UnityEngine;

public class Loader : MonoBehaviour
{
    public void LoadScene(string name)
    {
        CustomLevelLoader.LoadLevel(name);
    }
}
