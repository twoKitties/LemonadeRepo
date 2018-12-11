using UnityEngine;

/// <summary>
/// Prints a debug message when this component is enabled or disabled. Useful when you
/// need to find out what activates/disactivates the Gameobject this component is attached to.
/// </summary>
public class GameObjectActivityMonitor : MonoBehaviour
{
    private void OnEnable()
    {
        Debug.LogFormat("[OnEnable] name: {0}", name);
    }
    private void OnDisable()
    {
        Debug.LogFormat("[OnDisable] name: {0}", name);
    }
}
