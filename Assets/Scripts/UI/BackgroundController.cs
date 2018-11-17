using UnityEngine;

public enum BGColors
{
    Black,
    Orange,
    LightBlue,
    Green,
    Purple
}
public class BackgroundController : MonoBehaviour {

    [SerializeField]
    private GameObject[] backgrounds;
    [SerializeField]
    private GameObject menuBackground;

    private void Awake()
    {
        StaticEventManager.OnBackgroundColorChanged += SetBackground;
        StaticEventManager.OnMenuEnter += EnableMenuBackground;
        StaticEventManager.OnMenuExit += DisableMenuBackground;
    }

    private void OnDestroy()
    {
        StaticEventManager.OnBackgroundColorChanged -= SetBackground;
        StaticEventManager.OnMenuEnter -= EnableMenuBackground;
        StaticEventManager.OnMenuExit -= DisableMenuBackground;
    }

    private void SetBackground(int index)
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (i == index)
                backgrounds[i].SetActive(true);
            else
                backgrounds[i].SetActive(false);
        }
        Debug.Log("color is set " + index);
    }

    private void EnableMenuBackground()
    {
        menuBackground.SetActive(true);
    }

    private void DisableMenuBackground()
    {
        menuBackground.SetActive(false);
    }
}
