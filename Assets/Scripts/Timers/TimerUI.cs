using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    [SerializeField]
    private Text[] timeTexts;
    
    public virtual void RefreshTime(string time)
    {
        foreach (var item in timeTexts)
        {
            item.text = time;
        }
    }
    public virtual void RefreshAmount(string amount)
    {

    }
}
