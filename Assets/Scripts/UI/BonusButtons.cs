using UnityEngine;
using UnityEngine.UI;

public class BonusButtons : MonoBehaviour
{
    public static System.Action OnRowBonusActivation;
    public static System.Action OnColumnBonusActivation;

    [SerializeField]
    private GameObject rowBonusButton;
    [SerializeField]
    private Text rowBonusButtonText;
    public GameObject RowBonusButton { get { return rowBonusButton; } }
    [SerializeField]
    private GameObject columnBonusButton;
    [SerializeField]
    private Text columnBonusButtonText;
    public GameObject ColumnBonusButton { get { return columnBonusButton; } }
    [SerializeField]
    private BoardController boardController;

    private void Start()
    {
        OnRowBonusActivation += ShowRowButton;
        OnColumnBonusActivation += ShowColumnButton;
    }
    private void OnDestroy()
    {
        OnRowBonusActivation -= ShowRowButton;
        OnColumnBonusActivation -= ShowColumnButton;
    }
    private void ShowRowButton()
    {
        rowBonusButton.SetActive(true);
        rowBonusButtonText.text = boardController.RowBonusAmount.ToString();
    }
    private void ShowColumnButton()
    {
        columnBonusButton.SetActive(true);
        columnBonusButtonText.text = boardController.ColumnBonusAmount.ToString();
    }
    public void UpdateText()
    {
        rowBonusButtonText.text = boardController.RowBonusAmount.ToString();
        columnBonusButtonText.text = boardController.ColumnBonusAmount.ToString();
    }
    public void CloseButton(Animator anim)
    {
        anim.GetComponent<Animator>().SetTrigger("isTriggered");
    }
}
