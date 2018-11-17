using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DataStorage;
using System.IO;

namespace LemonadeStore
{
    public class Storage : MonoBehaviour
    {
        public static List<Storage> GradeItems = new List<Storage>();
        [SerializeField]
        private Upgrade grade;
        [SerializeField]
        private GameObject[] gradesUI;
        [SerializeField]
        private Button myButton;

        private string openLabel;
        private string openDescription;
        private string openCost;

        private Image[] chooseGradeButtons;
        private bool[] areBought = { true, false, false, false, false };
        [SerializeField]
        private int currentGradeID;
        [HideInInspector]
        public int CurrentGradeID { get { return currentGradeID; } }

        private void OnEnable()
        {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS
            string path = Path.Combine(Application.streamingAssetsPath, SaveLoadController.Instance.FileName);
#elif UNITY_ANDROID
            string path = Path.Combine(Application.persistentDataPath, SaveLoadController.Instance.FileName);
#endif
            if (File.Exists(path))
            {
                Item item = SaveLoadController.Instance.Load(grade.GradeKey);
                currentGradeID = item.CurrentGradeID;
                areBought = item.GradeStates;
            }

            if (grade != null)
                SetGrade(currentGradeID);
            GradeItems.Add(this);
        }
        private void OnDisable()
        {
            GradeItems.Remove(this);
        }
        private void SetGrade(int gradeID)
        {
            // Set labes, desc and cost
            openLabel = grade.LabelKeyRu;
            openDescription = grade.DescriptionKeyRu[gradeID];
            openCost = grade.GradeCost[gradeID].ToString();
            // Set active object
            for (int i = 0; i < gradesUI.Length; i++)
            {
                if (i == gradeID)
                    gradesUI[i].SetActive(true);
                else
                    gradesUI[i].SetActive(false);
            }
            // Send target graphic to button
            myButton.targetGraphic = gradesUI[gradeID].GetComponent<Image>();
        }
        public GradeInfo GetGradeInfo()
        {
            GradeInfo info = new GradeInfo(currentGradeID, openLabel, grade.GradeKey, grade.DescriptionKeyRu, grade.GradeCost, grade.GradeImages[currentGradeID], grade.GradeImages, areBought);
            return info;
        }
        public void UpgradeItem(int gradeID)
        {
            currentGradeID = gradeID;
            areBought[currentGradeID] = true;
            SetGrade(currentGradeID);
        }
    }
    public struct GradeInfo
    {
        public int GradeID;
        public string Label;
        public string Key;
        public string[] Description;
        public int[] Cost;
        public Sprite GradeCurrentImage;
        public Sprite[] GradeImages;
        public bool[] AreBought;

        public GradeInfo(int GradeID, string Label, string Key, string[] Description, int[] Cost, Sprite GradeCurrent, Sprite[] GradeImages, bool[] AreBought)
        {
            this.GradeID = GradeID;
            this.Label = Label;
            this.Key = Key;
            this.Description = Description;
            this.Cost = Cost;
            this.GradeCurrentImage = GradeCurrent;
            this.GradeImages = GradeImages;
            this.AreBought = AreBought;
        }
    }
}
