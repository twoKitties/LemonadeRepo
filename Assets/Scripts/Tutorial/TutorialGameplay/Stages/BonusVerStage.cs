using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusVerStage : TutorialStage
{
    [SerializeField]
    private GameObject[] stageWindowUI;

    private void Start()
    {
        SetStageUI(true);
    }
    public void Accept()
    {
        SetStageUI(false);
        TutorialEventList.OnGameTilesReachedBottom += ShowRestartWindow;
        TutorialEventList.OnGameTilesBurned += EndStage;
        FindObjectOfType<BoardController>().SpawnForVertical();
    }
    public override void EndStage()
    {
        base.EndStage();
        TutorialEventList.OnGameTilesReachedBottom -= ShowRestartWindow;
        TutorialEventList.OnGameTilesBurned -= EndStage;
    }
    public override void RestartStage()
    {
        base.RestartStage();
        SetStageUI(true);
    }
    private void SetStageUI(bool state)
    {
        foreach (var item in stageWindowUI)
        {
            item.SetActive(state);
        }
    }
}
