using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LemonadeStore
{
    public class ShopInapp : MonoBehaviour
    {
        public void AddCandies(int amount)
        {
            FindObjectOfType<UpgradeWorker>().Points += amount;
            int points = FindObjectOfType<UpgradeWorker>().Points;
            FindObjectOfType<GradeWindow>().SetPoints(points);
        }
    }
}
