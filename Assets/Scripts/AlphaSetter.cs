using UnityEngine;
using UnityEngine.UI;

public class AlphaSetter : MonoBehaviour
{
    [SerializeField]
    private float alphaValue;

    private void Start()
    {
        this.GetComponent<Image>().alphaHitTestMinimumThreshold = alphaValue;
    }
}
