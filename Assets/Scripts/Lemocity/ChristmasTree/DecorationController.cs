using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LemonadeStore
{
    public class DecorationController : MonoBehaviour
    {
        [SerializeField]
        private DecorationWindow decorationWindow;
        private DecorationStorage currentStorage;

        public void OpenDecorationWindow(DecorationStorage chosenStorage)
        {
            // Set images of buttons in decoration window and their state
            for (int i = 0; i < 4; i++)
            {
                decorationWindow.LoadButtonImage(i, chosenStorage.GetDecorationItemImage(i));
                decorationWindow.LoadButtonState(i, chosenStorage.GetDecorationItemState(i));
            }

            // Open decoration window
            decorationWindow.gameObject.SetActive(true);
            // Save ref to current storage
            if (currentStorage == null)
                currentStorage = chosenStorage;
        }
        public void CloseDecorationWindow()
        {
            if (currentStorage != null)
                currentStorage = null;
            decorationWindow.gameObject.SetActive(false);
        }
        public void ChooseActiveDecoration(int buttonNumber)
        {
            currentStorage.PickDecorationItem(buttonNumber);

            SaveLoadController.Instance.Save();
        }
    }
}
