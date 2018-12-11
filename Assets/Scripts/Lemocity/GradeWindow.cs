using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LemonadeStore
{
    public class GradeWindow : MonoBehaviour
    {
        [SerializeField]
        private Text Label;
        [SerializeField]
        private Text Description;
        [SerializeField]
        private Image ChosenGrade;
        [SerializeField]
        private Image[] GradeLevels;
        [SerializeField]
        private Button[] GradeButtons;
        [SerializeField]
        private Button[] testDecorationButtons;
        [SerializeField]
        private Button UpgradeButton;
        [SerializeField]
        private Text Cost;
        [SerializeField]
        private Text AllPoints;

        private bool testButtonsActive;
        public bool TestButtonsActive { get { return testButtonsActive; } }

        public void SetButtonImages(Sprite sprite, int buttonID)
        {
            GradeLevels[buttonID - 1].sprite = sprite;
        }
        public void SetLabel(string label)
        {
            Label.text = label;
        }
        public void SetDescription(string descriptionText)
        {
            Description.text = descriptionText;
        }
        public void SetChosenGrade(Sprite sprite)
        {
            ChosenGrade.sprite = sprite;
        }
        public void SetCost(int cost)
        {
            Cost.text = cost.ToString();
        }
        public void SetPoints(int points)
        {
            AllPoints.text = points.ToString();
        }
        public void SetUpgradeButtonState(bool state, int gradeLevel, int generalGradeLevel)
        {
            if (!state && (gradeLevel == generalGradeLevel))
                UpgradeButton.interactable = true;
            else
                UpgradeButton.interactable = false;
        }
        public void ActivateChooseButtons(bool[] buttonStates)
        {
            for (int i = 0; i < GradeButtons.Length; i++)
            {
                GradeButtons[i].interactable = !buttonStates[i + 1];
            }
        }
        #region TestDecorationButtons
        public void ActivateDecorationButtons(bool activate)
        {
            for (int i = 0; i < testDecorationButtons.Length; i++)
                testDecorationButtons[i].gameObject.SetActive(activate);
            testButtonsActive = activate;
        }
        public void SetDecorationButtonsInteractive(bool[] states)
        {
            for (int i = 0; i < testDecorationButtons.Length; i++)
                testDecorationButtons[i].interactable = states[i + 1];
        }
        // SetDecorationButtonsWithImages(
        public void SetDecorationButtonsWithImages(Sprite[] sprites)
        {
            for (int i = 0; i < testDecorationButtons.Length; i++)
                testDecorationButtons[i].GetComponent<Image>().sprite = sprites[i];
        }
        #endregion
        [SerializeField]
        private Color blinkColor;
        [SerializeField]
        private Color defaultColor;

        public delegate void OpenWindowCallback();
        private IEnumerator Blink(OpenWindowCallback callback)
        {
            float maxTime = 1f;
            float time = Time.deltaTime;

            AllPoints.color = blinkColor;
            Cost.color = blinkColor;

            while (maxTime <= 1)
            {
                yield return new WaitForEndOfFrame();

                time += Time.deltaTime;
                float perc = time / maxTime;

                AllPoints.color = Color.Lerp(blinkColor, defaultColor, perc);
                Cost.color = Color.Lerp(blinkColor, defaultColor, perc);
            }
            callback();
        }
        public void CallBlink(OpenWindowCallback callback)
        {
            StartCoroutine(Blink(callback));
        }
    }
}
