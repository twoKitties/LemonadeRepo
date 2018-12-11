using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LemonadeStore
{
    public class HomeDecoration : MonoBehaviour
    {
        public static List<HomeDecoration> AllHomeDecorations = new List<HomeDecoration>();

        [SerializeField]
        private GameObject[] flags;
        [SerializeField]
        private GameObject[] lightBulbs;
        [SerializeField]
        private GameObject[] lightBulbsBottom;
        [SerializeField]
        private GameObject[] ribbons;
        [SerializeField]
        private Sprite[] decorationIcons;
        [SerializeField]
        private Sprite[] decorationIconsLocked;
        [SerializeField]
        private string[] descriptionEn;
        [SerializeField]
        private string[] descriptionRu;
        private bool[] unlocked = { true, false, false, false, false };
        private List<GameObject[]> decorations = new List<GameObject[]>();
        private int currentDecorationID;
        [SerializeField]
        private string homeDecorationKey;

        private void Awake()
        {
            decorations.Add(flags);
            decorations.Add(lightBulbs);
            decorations.Add(lightBulbsBottom);
            decorations.Add(ribbons);
        }
        private void Start()
        {
            AllHomeDecorations.Add(this);

            LoadDecorationInfo();

            PickDecorationItem(currentDecorationID);
        }
        private void OnDisable()
        {
            AllHomeDecorations.Remove(this);
        }
        private void LoadDecorationInfo()
        {
            DataStorage.Item infoToLoad = SaveLoadController.Instance.Load(homeDecorationKey);
            if (infoToLoad == null)
                return;
            currentDecorationID = infoToLoad.CurrentGradeID;
            unlocked = infoToLoad.GradeStates;
        }
        public Sprite[] GetDecorationItemImage()
        {
            Sprite[] sprites = new Sprite[4];
            for(int i = 0; i < 4; i++)
            {
                if (unlocked[i + 1])
                    sprites[i] = decorationIcons[i];
                else
                    sprites[i] = decorationIconsLocked[i];
            }
            return sprites;
        }        
        public HomeDecorItem GetItem()
        {
            int itemID = 0;
            for (int i = 0; i < decorations.Count; i++)
            {
                if (!unlocked[i])
                {
                    itemID = i;
                    break;
                }
            }
            string description = (PlayerPrefsHelper.GetInt(GlobalConst.CURRENT_LANGUAGE) == 0) ? descriptionRu[itemID] : descriptionEn[itemID];
            HomeDecorItem item = new HomeDecorItem(decorations[itemID - 1][1].GetComponent<Image>().sprite, itemID, description);
            return item;
        }
        public bool[] GetDecorationItemStates()
        {
            return unlocked;
        }
        public void PickDecorationItem(int decorationID)
        {
            currentDecorationID = decorationID;
            if (currentDecorationID == 0)
                return;

            for (int i = 0; i < decorations.Count; i++)
            {
                if (i == decorationID - 1)
                    for (int j = 0; j < decorations[i].Length; j++)
                        decorations[i][j].SetActive(true);
                else
                    for (int j = 0; j < decorations[i].Length; j++)
                        decorations[i][j].SetActive(false);
            }
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
        public DecorationInfo GetHomeDecorationInfo()
        {
            DecorationInfo info = new DecorationInfo(homeDecorationKey, currentDecorationID, unlocked);
            return info;
        }
    }
}
