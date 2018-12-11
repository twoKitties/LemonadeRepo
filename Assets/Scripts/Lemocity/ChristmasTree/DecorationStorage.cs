using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace LemonadeStore
{
    public class DecorationStorage : MonoBehaviour
    {
        public static List<DecorationStorage> AllDecorationStorages = new List<DecorationStorage>();

        [SerializeField]
        private DecorSet decorationsItems;
        [SerializeField]
        private GameObject[] DecorGameobjects;
        [SerializeField]
        private Button myButton;

        private int currentDecoration;
        private bool[] unlocked = { true, false, false, false, false };

        private void Start()
        {
            AllDecorationStorages.Add(this);

            LoadDecorationInfo();

            SetDecorGameobject(currentDecoration);
        }
        private void LoadDecorationInfo()
        {
            DataStorage.Item infoToLoad = SaveLoadController.Instance.Load(decorationsItems.DecorSetKey);
            if (infoToLoad == null)
                return;
            currentDecoration = infoToLoad.CurrentGradeID;
            unlocked = infoToLoad.GradeStates;
        }
        private void SetDecorGameobject(int itemNumber)
        {
            for (int i = 0; i < DecorGameobjects.Length; i++)
            {
                if (i == itemNumber)
                    DecorGameobjects[i].SetActive(true);
                else
                    DecorGameobjects[i].SetActive(false);
            }

            // Set decoration in little tree
            GetComponent<DecorationHelper>().SetDecoration(currentDecoration);
        }
        public Sprite GetDecorationItemImage(int buttonNumber)
        {
            if (unlocked[buttonNumber + 1])
                return decorationsItems.Icons[buttonNumber];
            else
                return decorationsItems.IconsLocked[buttonNumber];
        }
        public bool GetDecorationItemState(int buttonNumber)
        {
            return unlocked[buttonNumber + 1];
        }
        public void PickDecorationItem(int decorationID)
        {
            currentDecoration = decorationID;
            for (int i = 0; i < DecorGameobjects.Length; i++)
            {
                if (i == decorationID)
                    DecorGameobjects[i].SetActive(true);
                else
                    DecorGameobjects[i].SetActive(false);
            }

            // Set decoration in little tree
            GetComponent<DecorationHelper>().SetDecoration(currentDecoration);
        }
        public void UnlockDecoration(int decorationID)
        {
            if (!unlocked[decorationID])
                unlocked[decorationID] = true;
        }
        public bool HasLockedDecoration()
        {
            for (int i = 0; i < unlocked.Length; i++)
                if (!unlocked[i])
                    return true;
            return false;
        }
        public DecorationItem GetItem()
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

            DecorationItem item = new DecorationItem(decorationsItems.Icons[itemID - 1], itemID);
            return item;
        }
        public DecorationInfo GetDecorationStorageInfo()
        {
            DecorationInfo info = new DecorationInfo(decorationsItems.DecorSetKey, currentDecoration, unlocked);
            return info;
        }
    }
    [System.Serializable]
    public struct DecorationItem
    {
        public Sprite Sprite;
        public int ID;
        public bool IsInitialized;
        public DecorationItem(Sprite Sprite, int ID)
        {
            this.Sprite = Sprite;
            this.ID = ID;
            IsInitialized = true;
        }
        public DecorationItem(bool state = false)
        {
            Sprite = new Sprite();
            ID = -1;
            IsInitialized = state;
        }
    }
    [System.Serializable]
    public struct DecorationInfo
    {
        public string Key;
        public int CurrentDecoration;
        public bool[] UnlockedDecorations;
        public DecorationInfo(string Key, int CurrentDecoration, bool[] UnlockedDecorations)
        {
            this.Key = Key;
            this.CurrentDecoration = CurrentDecoration;
            this.UnlockedDecorations = UnlockedDecorations;
        }
    }
}
