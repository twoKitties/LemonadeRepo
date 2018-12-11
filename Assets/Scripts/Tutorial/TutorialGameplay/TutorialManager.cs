using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    private TutorialStage TutorialWindowStart;
    [SerializeField]
    private TutorialStage TutorialSwipeStage;
    [SerializeField]
    private TutorialStage TutorialBurnStage;
    [SerializeField]
    private TutorialStage TutorialHorizontalStage;
    [SerializeField]
    private TutorialStage TutorialVerticalStage;
    [SerializeField]
    private TutorialStage TutorialSuperStage;
    [SerializeField]
    private TutorialStage TutorialButtonHorizontalStage;
    [SerializeField]
    private TutorialStage TutorialButtonVerticalStage;

    [Header("End Tutorial Window")]
    [SerializeField]
    private GameObject endWindowTutorial;
    private GameController gameController;

    private void OnEnable()
    {
        Initialize();
    }
    public void Initialize()
    {
        TutorialWindowStart.StartStage();
        GameController.IsTutorActive = true;
        gameController = FindObjectOfType<GameController>();
    }
    public void StartSwipeStage()
    {
        TutorialWindowStart.CloseStage();
        TutorialSwipeStage.StartStage();
    }
    public void StartBurnStage()
    {
        TutorialSwipeStage.CloseStage();
        TutorialBurnStage.StartStage();
        FindObjectOfType<BoardController>().ResetBoard(true);
    }
    public void StartBurnHorStage()
    {
        TutorialBurnStage.CloseStage();
        TutorialHorizontalStage.StartStage();
        FindObjectOfType<BoardController>().ResetBoard(true);
    }
    public void StartBurnVerStage()
    {
        TutorialHorizontalStage.CloseStage();
        TutorialVerticalStage.StartStage();
        FindObjectOfType<BoardController>().ResetBoard(true);
    }
    public void StartSuperStage()
    {
        TutorialVerticalStage.CloseStage();
        TutorialSuperStage.StartStage();
        FindObjectOfType<BoardController>().ResetBoard(true);
    }
    public void StartButtonHorStage()
    {
        GameController.IsTutorActive = false;
        TutorialSuperStage.CloseStage();
        TutorialButtonHorizontalStage.StartStage();
        FindObjectOfType<BoardController>().ResetBoard(true);
    }
    public void StartButtonVerStage()
    {
        TutorialButtonHorizontalStage.CloseStage();
        TutorialButtonVerticalStage.StartStage();
        FindObjectOfType<BoardController>().ResetBoard(true);
    }
    public void StartFinalStage()
    {
        endWindowTutorial.SetActive(true);
    }
    public void EndTutorial()
    {
        gameController.EndTutorial();
        FindObjectOfType<LemonadeStore.UpgradeWorker>().Points += 500;
        gameController.OpenLemoncityFromGame();
    }
    public void CloseWindow(GameObject window)
    {
        window.SetActive(false);
    }
    public void OpenWindow(GameObject window)
    {
        window.SetActive(true);
    }
}
