using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LemonadeStore
{
    [CreateAssetMenu(fileName = "Grade", menuName = "Inventory/Updrages")]
    public class Upgrade : ScriptableObject
    {
        public string GradeKey;
        public string LabelKeyEng;
        public string LabelKeyRu;
        public string[] DescriptionKeyEng;
        public string[] DescriptionKeyRu;
        public Sprite[] GradeImages;
        public int CurrentGradeID;
        public int[] GradeCost = { 0, 10, 20, 30, 40 };
    }
}
