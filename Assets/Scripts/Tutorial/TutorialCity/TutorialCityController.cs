using UnityEngine;

public class TutorialCityController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] stages;
	
    public void StartStage(int stageNumber)
    {
        for(int i = 0; i < stages.Length; i++)
        {
            if (i == stageNumber)
                stages[i].SetActive(true);
            else
                stages[i].SetActive(false);
        }
    }
    public void EndTutorial()
    {
        gameObject.SetActive(false);
    }
    public void Open(GameObject objToOpen)
    {
        objToOpen.SetActive(true);
    }
    public void Close(GameObject objToClose)
    {
        objToClose.SetActive(false);
    }
}
