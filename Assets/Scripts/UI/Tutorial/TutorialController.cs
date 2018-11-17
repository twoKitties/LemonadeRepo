using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] tutorialSteps;

    private void OnEnable()
    {
        tutorialSteps[0].SetActive(true);
    }

    public void GoStep(int stepIndex)
    {
        for (int i = 0; i < tutorialSteps.Length; i++)
        {
            if (i == stepIndex)            
                tutorialSteps[i].SetActive(true);            
            else
                tutorialSteps[i].SetActive(false);
        }
    }
}
