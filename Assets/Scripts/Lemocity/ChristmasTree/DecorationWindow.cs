using UnityEngine;
using UnityEngine.UI;

namespace LemonadeStore
{
    public class DecorationWindow : MonoBehaviour
    {
        [SerializeField]
        private Image[] buttonImages;

        public void LoadButtonImage(int buttonNumber, Sprite sprite)
        {
            buttonImages[buttonNumber].sprite = sprite;
        }
        public void LoadButtonState(int buttonNumber, bool state)
        {
            buttonImages[buttonNumber].gameObject.GetComponent<Button>().interactable = state;
        }
    }
}
