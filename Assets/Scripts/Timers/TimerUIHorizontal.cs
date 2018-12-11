using UnityEngine;
using UnityEngine.UI;
public class TimerUIHorizontal : TimerUI
{
    [SerializeField]
    private Text[] amountTexts;
    private BoardController boardController;
    private void Start()
    {
        boardController = FindObjectOfType<BoardController>();
    }
    public override void RefreshAmount(string amount)
    {
        foreach (var item in amountTexts)
        {
            item.text = amount;
        }
    }
}
