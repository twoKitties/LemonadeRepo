using UnityEngine;
using UnityEngine.UI;
using System;
using LemonadeStore;

public class PointsCount : MonoBehaviour
{
    public static Action OnPointsChange;

    [SerializeField]
    private Text points;
    private UpgradeWorker pointsContainer;

    private void OnEnable()
    {
        OnPointsChange += Refresh;
        if (pointsContainer == null)
            pointsContainer = FindObjectOfType<UpgradeWorker>();
        points.text = pointsContainer.Points.ToString();
    }
    private void OnDisable()
    {
        OnPointsChange -= Refresh;
    }
    private void Refresh()
    {
        points.text = pointsContainer.Points.ToString();
    }
}
