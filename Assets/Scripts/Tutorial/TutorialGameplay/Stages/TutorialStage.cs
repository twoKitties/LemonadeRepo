using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStage : MonoBehaviour
{
    [SerializeField]
    private GameObject successUI;
    [SerializeField]
    private GameObject restartWindow;

    public virtual void StartStage()
    {
        gameObject.SetActive(true);
    }
    public virtual void CloseStage()
    {
        gameObject.SetActive(false);
    }
    public virtual void EndStage()
    {
        successUI.SetActive(true);
    }
    public virtual void ShowRestartWindow()
    {
        restartWindow.SetActive(true);
    }
    public virtual void RestartStage()
    {
        FindObjectOfType<BoardController>().ResetBoard(true);
    }
}
