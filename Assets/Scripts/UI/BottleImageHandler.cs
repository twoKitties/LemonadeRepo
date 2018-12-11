using UnityEngine;
using UnityEngine.UI;

public class BottleImageHandler : MonoBehaviour {

    [SerializeField]
    private BottleImage[] bottleImages;
    [SerializeField]
    private GameObject bottleButton;
    private Image imageComponent;

    //private string[] points = { "0", "50", "100", "150", "200", ""};
    private string[] points = { "0", "100", "150", "200", "210", "220", "230", "250", "260", "270", "" };
    private Text textComponentInChild;

    private void Awake()
    {
        StaticEventManager.OnBottleImageRefresh += ChangeBottleImage;
        StaticEventManager.OnBottleImageRefresh += ChangePointsText;
    }

    private void OnDestroy()
    {
        StaticEventManager.OnBottleImageRefresh -= ChangeBottleImage;
        StaticEventManager.OnBottleImageRefresh -= ChangePointsText;
    }

    private void OnEnable()
    {
        imageComponent = bottleButton.GetComponent<Image>();
        textComponentInChild = GetComponentInChildren<Text>();
    }

    private void ChangeBottleImage(int index)
    {
        if (index < bottleImages.Length)
            imageComponent.sprite = bottleImages[index].sprite;
        Debug.Log("Imagebottle index " + index);
    }

    private void ChangePointsText(int index)
    {
        if (index < points.Length)
            textComponentInChild.text = points[index];
        Debug.Log("text index " + index);
    }

}
[System.Serializable]
public class BottleImage
{
    public Sprite sprite;

    public BottleImage(Sprite sprite)
    {
        this.sprite = sprite;
    }
}