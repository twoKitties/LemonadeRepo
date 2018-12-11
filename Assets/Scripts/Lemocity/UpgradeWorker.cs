using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;

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
        private GradeWindow gradeWindowExtended;
        [SerializeField]
        private ShopInapp shop;
        // Here lies current loaded scriptableObject
        private Upgrade currentGrade;
        private Storage currentStorage;

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
        public int GeneralGradeLevel { get { return generalGradeLevel; } }
        private int currentlyChosenGrade;
        public Button PressedButton;
        [SerializeField]
        private GameObject congrazWindow;
        [Header("Tutorial")]
        [SerializeField]
        private TutorialCityController tutorialCityController;
        private bool tutorialPassed
        {
            get { return PlayerPrefsHelper.GetBool("tutorialState"); }
            set { PlayerPrefsHelper.SetBool("tutorialState", value); }
        }
        #region Test Decoration
        private HomeDecoration homeDecoration;
        #endregion
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

            // Starts tutorial
            if (!tutorialPassed)
            {
                tutorialCityController.gameObject.SetActive(true);
                tutorialPassed = true;
            }
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
            foreach (var item in Storage.AllStorages)
            {
                i += item.MaxUnlockedGrade;
            }
            float result = i / Storage.AllStorages.Count;
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
            // Here if player doesn't have enough points show window opens
            if(total < 0)
            {
                //GradeWindow.OpenWindowCallback callback = OpenShopWindow;
                //if (gradeWindow.enabled)
                //    gradeWindow.CallBlink(callback);
                //else if (gradeWindowExtended.enabled)
                //    gradeWindowExtended.CallBlink(callback);
                OpenShopWindow();
                return;
            }

            Points -= storageInfo.Cost[currentlyChosenGrade];
            currentStorage.UpgradeItem(currentlyChosenGrade);
            storageInfo = currentStorage.GetGradeInfo();
            if (gradeWindow.gameObject.activeSelf)
            {
                gradeWindow.SetPoints(Points);
                gradeWindow.ActivateChooseButtons(storageInfo.AreBought);
                gradeWindow.SetUpgradeButtonState(storageInfo.AreBought[currentlyChosenGrade], currentlyChosenGrade, generalGradeLevel);
            }
            else if (gradeWindowExtended.gameObject.activeSelf)
            {
                gradeWindowExtended.SetPoints(Points);
                gradeWindowExtended.ActivateChooseButtons(storageInfo.AreBought);
                gradeWindowExtended.SetUpgradeButtonState(storageInfo.AreBought[currentlyChosenGrade], currentlyChosenGrade, generalGradeLevel);
            }

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

            // If open grade is 0 then all info in grade array loads with next index (i.e. 1)
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

            #region TestRegion
            //homeDecoration = objToGrade.GetComponent<HomeDecoration>();
            //if (homeDecoration != null)
            //{
            //    gradeWindow.ActivateDecorationButtons(true);
            //    var states = homeDecoration.GetDecorationItemStates();
            //    gradeWindow.SetDecorationButtonsInteractive(states);
            //    var sprites = homeDecoration.GetDecorationItemImage();
            //    gradeWindow.SetDecorationButtonsWithImages(sprites);
            //}
            #endregion
        }
        public void OpenGradeWindowExtended(GameObject objToGrade)
        {
            if (objToGrade == null)
                return;

            currentStorage = objToGrade.transform.GetComponent<Storage>();
            var storageInfo = currentStorage.GetGradeInfo();
            gradeWindowExtended.SetLabel(storageInfo.Label);

            // If open grade is 0 then all info in grade array loads with next index (i.e. 1)
            int temp = (storageInfo.AreBought[storageInfo.GradeID]) ? temp = storageInfo.GradeID + 1 : temp = storageInfo.GradeID;
            temp = (temp > 4) ? 4 : temp;

            gradeWindowExtended.SetDescription(storageInfo.Description[temp]);
            gradeWindowExtended.SetChosenGrade(storageInfo.GradeImages[temp]);
            gradeWindowExtended.SetCost(storageInfo.Cost[temp]);
            for (int i = 1; i < 5; i++)
                gradeWindowExtended.SetButtonImages(storageInfo.GradeImages[i], i);
            gradeWindowExtended.SetPoints(Points);

            currentlyChosenGrade = temp;
            gradeWindowExtended.ActivateChooseButtons(storageInfo.AreBought);
            gradeWindowExtended.SetUpgradeButtonState(storageInfo.AreBought[currentlyChosenGrade], currentlyChosenGrade, generalGradeLevel);
            // Here Save ref to active button
            //PressedButton = gradeWindow.GetActiveButton(currentGrade);

            SetButtonActive(false);
            gradeWindowExtended.gameObject.SetActive(true);

            homeDecoration = objToGrade.GetComponent<HomeDecoration>();
            gradeWindowExtended.ActivateDecorationButtons(true);
            var states = homeDecoration.GetDecorationItemStates();
            gradeWindowExtended.SetDecorationButtonsInteractive(states);
            var sprites = homeDecoration.GetDecorationItemImage();
            gradeWindowExtended.SetDecorationButtonsWithImages(sprites);
        }
        #region TestChooseDecoration
        public void ChooseDecoration(int decorationID)
        {
            homeDecoration.PickDecorationItem(decorationID);
        }
        #endregion
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
        public void ChooseGradeInExtended(int itemId)
        {
            var storageInfo = currentStorage.GetGradeInfo();
            gradeWindowExtended.SetDescription(storageInfo.Description[itemId]);
            gradeWindowExtended.SetChosenGrade(storageInfo.GradeImages[itemId]);
            gradeWindowExtended.SetCost(storageInfo.Cost[itemId]);
            gradeWindowExtended.SetUpgradeButtonState(storageInfo.AreBought[itemId], itemId, generalGradeLevel);
            currentlyChosenGrade = itemId;
        }
        public void CloseWindow(GameObject window)
        {
            if (currentGrade != null)
                currentGrade = null;
            if (PressedButton != null)
                PressedButton = null;
            #region TestDecorationButtons
            if (gradeWindow.TestButtonsActive)
                gradeWindow.ActivateDecorationButtons(false);
            #endregion
            SetButtonActive(true);
            window.SetActive(false);
        }

        [SerializeField]
        private GameObject closeWindowButton;

        private void SetButtonActive(bool state)
        {
            closeWindowButton.SetActive(state);
        }
    }
}
