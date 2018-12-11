using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeStage : TutorialStage
{
    [SerializeField]
    private GameObject[] UI;
    private bool stageDone;
    private void Start()
    {
        ShowTutorialWindow();
    }
    private void ShowTutorialWindow()
    {
        foreach (var item in UI)
        {
            item.SetActive(true);
        }
    }
    public void Accept()
    {
        foreach (var item in UI)
        {
            item.SetActive(false);
        }
        TutorialEventList.OnGameTileSwaped += EndStage;
        TutorialEventList.OnGameTilesReachedBottom += ShowRestartWindow;
        FindObjectOfType<BoardController>().SpawnForSwipeStage();
    }
    public override void EndStage()
    {
        base.EndStage();
        TutorialEventList.OnGameTilesReachedBottom -= ShowRestartWindow;
        TutorialEventList.OnGameTileSwaped -= EndStage;
    }
    public override void RestartStage()
    {
        base.RestartStage();
        ShowTutorialWindow();
    }
    public override void ShowRestartWindow()
    {
        TutorialEventList.OnGameTilesReachedBottom -= ShowRestartWindow;
        TutorialEventList.OnGameTileSwaped -= EndStage;
        base.ShowRestartWindow();
    }
}
