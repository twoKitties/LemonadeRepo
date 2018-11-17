using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace LemonadeStore
{
    public class UpgradeWorker : MonoBehaviour
    {
        // Refs to canvas and grade window
        [SerializeField]
        private GameObject lemonCanvas;
        [SerializeField]
        private GradeWindow gradeWindow;
        [SerializeField]
        private ShopInapp shop;
        // Here lies current loaded scriptableObject
        private Upgrade currentGrade;
        private Storage currentStorage;

        [SerializeField]
        private GameObject[] grades;
        private readonly string gradesKey = "gradeLevel";
        private int gradesAmount
        {
            get
            {
                return PlayerPrefsHelper.GetInt(gradesKey);
            }
            set
            {
                PlayerPrefsHelper.SetInt(gradesKey, value);
            }
        }
        private readonly string pointsKey = "pointsKey";
        public int Points
        {
            get
            {
                return PlayerPrefsHelper.GetInt(pointsKey);
            }
            set
            {
                PlayerPrefsHelper.SetInt(pointsKey, value);
            }
        }

        private readonly string currentGradeLevelKey = "gradeLevelKey";
        private int generalGradeLevel
        {
            get { return PlayerPrefsHelper.GetInt(currentGradeLevelKey); }
            set { PlayerPrefsHelper.SetInt(currentGradeLevelKey, value); }
        }

        private int currentlyChosenGrade;
        public Button PressedButton;
        [SerializeField]
        private GameObject congrazWindow;

        private void Start()
        {
            if (gradeWindow != null)
                gradeWindow.gameObject.SetActive(false);
            else
                Debug.Log("gradeWindow is missing");
        }

        public void OpenLemoncityWindow()
        {
            lemonCanvas.SetActive(true);
            GetData();
        }
        public void CloseLemoncityWindow()
        {
            lemonCanvas.SetActive(false);
        }
        public void OpenShopWindow()
        {
            shop.gameObject.SetActive(true);
        }
        public void CloseShopWindow(GameObject window)
        {
            window.SetActive(false);
        }
        public void OpenCongrazWindow()
        {
            congrazWindow.SetActive(true);
        }
        private void GetData()
        {
            float i = 0;
            foreach (var item in Storage.GradeItems)
            {
                i += item.CurrentGradeID;
            }
            float result = i / Storage.GradeItems.Count;
            Debug.Log("value " + result);
            // Test lines for window opening
            int temp = generalGradeLevel;           

            switch ((int)result)
            {
                case 0:
                    generalGradeLevel = 1;
                    break;
                case 1:
                    generalGradeLevel = 2;
                    break;
                case 2:
                    generalGradeLevel = 3;
                    break;
                case 3:
                    generalGradeLevel = 4;
                    break;
            }
            Debug.Log("generalGradeLevel " + generalGradeLevel);

            if (generalGradeLevel > 1 && temp != generalGradeLevel)
                OpenCongrazWindow();
        }

        public void Upgrade(/*GameObject objToGrade*/)
        {
            var storageInfo = currentStorage.GetGradeInfo();
            int total = Points - storageInfo.Cost[currentlyChosenGrade];

            Points -= storageInfo.Cost[currentlyChosenGrade];
            gradeWindow.SetPoints(Points);

            currentStorage.UpgradeItem(currentlyChosenGrade);
            storageInfo = currentStorage.GetGradeInfo();
            gradeWindow.ActivateChooseButtons(storageInfo.AreBought);
            gradeWindow.SetUpgradeButtonState(storageInfo.AreBought[currentlyChosenGrade], currentlyChosenGrade, generalGradeLevel);

            GetData();
            SaveLoadController.Instance.Save();
        }
        public void OpenGradeWindow(GameObject objToGrade)
        {
            if (objToGrade == null)
                return;

            currentStorage = objToGrade.transform.GetComponent<Storage>();
            var storageInfo = currentStorage.GetGradeInfo();
            gradeWindow.SetLabel(storageInfo.Label);

            int temp = (storageInfo.AreBought[storageInfo.GradeID]) ? temp = storageInfo.GradeID + 1 : temp = storageInfo.GradeID;
            temp = (temp > 4) ? 4 : temp;

            gradeWindow.SetDescription(storageInfo.Description[temp]);
            gradeWindow.SetChosenGrade(storageInfo.GradeImages[temp]);
            gradeWindow.SetCost(storageInfo.Cost[temp]);
            for (int i = 1; i < 5; i++)
                gradeWindow.SetButtonImages(storageInfo.GradeImages[i], i);
            gradeWindow.SetPoints(Points);

            currentlyChosenGrade = temp;
            gradeWindow.ActivateChooseButtons(storageInfo.AreBought);
            gradeWindow.SetUpgradeButtonState(storageInfo.AreBought[currentlyChosenGrade], currentlyChosenGrade, generalGradeLevel);
            // Here Save ref to active button
            //PressedButton = gradeWindow.GetActiveButton(currentGrade);

            SetButtonActive(false);
            gradeWindow.gameObject.SetActive(true);            
        }
        public void ChooseGrade(int itemID)
        {
            var storageInfo = currentStorage.GetGradeInfo();
            gradeWindow.SetDescription(storageInfo.Description[itemID]);
            gradeWindow.SetChosenGrade(storageInfo.GradeImages[itemID]);
            gradeWindow.SetCost(storageInfo.Cost[itemID]);
            gradeWindow.SetUpgradeButtonState(storageInfo.AreBought[itemID], itemID, generalGradeLevel);
            currentlyChosenGrade = itemID;

            PressedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        }
        public void CloseWindow(GameObject window)
        {
            window.SetActive(false);
            if (currentGrade != null)
                currentGrade = null;
            if (PressedButton != null)
                PressedButton = null;

            SetButtonActive(true);
        }

        [SerializeField]
        private GameObject closeWindowButton;

        private void SetButtonActive(bool state)
        {
            closeWindowButton.SetActive(state);
        }
    }
}
