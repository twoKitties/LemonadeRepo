using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DataStorage;
using System.IO;

namespace LemonadeStore
{
    public class Storage : MonoBehaviour
    {
        public static List<Storage> AllStorages = new List<Storage>();
        [SerializeField]
        private Upgrade grade;
        // Links to gameobjects with images to show on scene
        [SerializeField]
        private GameObject[] gradesUI;
        [SerializeField]
        private Button myButton;

        private string openLabel;
        private string openDescription;
        private string openCost;

        private Image[] chooseGradeButtons;
        private bool[] unlocked = { true, false, false, false, false };
        public bool[] Unlocked { get { return unlocked; } }
        [SerializeField]
        private int maxUnlockedGrade;
        public int MaxUnlockedGrade { get { return maxUnlockedGrade; } }
        private int currentGrade;

        private void Start()
        {
            AllStorages.Add(this);

            LoadGradeInfo();

            SetGrade(maxUnlockedGrade);
        }
        private void OnDisable()
        {
            AllStorages.Remove(this);
        }
        private void LoadGradeInfo()
        {
            //#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS
            //            string path = Path.Combine(Application.streamingAssetsPath, SaveLoadController.Instance.FileName);
            //#elif UNITY_ANDROID
            //            string path = Path.Combine(Application.persistentDataPath, SaveLoadController.Instance.FileName);
            //#endif
            //            if (File.Exists(path))

            Item infoToLoad = SaveLoadController.Instance.Load(grade.GradeKey);
            if (infoToLoad == null)
                return;
            maxUnlockedGrade = infoToLoad.CurrentGradeID;
            unlocked = infoToLoad.GradeStates;
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
            GradeInfo info = new GradeInfo(maxUnlockedGrade, openLabel, grade.GradeKey, grade.DescriptionKeyRu, grade.GradeCost, grade.GradeImages[maxUnlockedGrade], grade.GradeImages, unlocked);
            return info;
        }
        public void UpgradeItem(int gradeID)
        {
            maxUnlockedGrade = gradeID;
            unlocked[maxUnlockedGrade] = true;
            SetGrade(maxUnlockedGrade);
        }
        public bool HasLockedGrade()
        {
            for (int i = 0; i < unlocked.Length; i++)
                if (!unlocked[i])
                    return true;
            return false;
        }
        public void UnlockGrade(int gradeID)
        {
            if(!unlocked[gradeID])
                unlocked[gradeID] = true;
        }
        public HomeDecorItem GetItem()
        {
            int itemID = 0;
            for (int i = 0; i < unlocked.Length; i++)
            {
                if (!unlocked[i])
                {
                    itemID = i;
                    break;
                }
            }
            string description = (PlayerPrefsHelper.GetInt(GlobalConst.CURRENT_LANGUAGE) == 0) ? grade.DescriptionKeyRu[itemID] : grade.DescriptionKeyEng[itemID];
            HomeDecorItem item = new HomeDecorItem(grade.GradeImages[itemID], itemID, description);
            return item;
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
    public struct HomeDecorItem
    {
        public Sprite Sprite;
        public int ID;
        public string Description;
        public bool IsInitizlized;
        public HomeDecorItem(Sprite Sprite, int ID, string Description)
        {
            this.Sprite = Sprite;
            this.ID = ID;
            this.Description = Description;
            IsInitizlized = true;
        }
        public HomeDecorItem(int i)
        {
            this.Sprite = new Sprite();
            this.ID = i;
            this.Description = "";
            IsInitizlized = false;
        }
    }
}
