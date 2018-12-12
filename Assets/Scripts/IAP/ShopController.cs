using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Purchasing;

public class ShopController : MonoBehaviour
{    
    public ShopItem[] items;

    //[HideInInspector]
    public bool isRandom;

    private void Awake()
    {
        StaticEventManager.OnBackgroundColorChanged += UnlockItem;

        items[0].IsUnlocked = true;
    }

    private void OnDestroy()
    {
        StaticEventManager.OnBackgroundColorChanged -= UnlockItem;
    }

    private void OnEnable()
    {
        ResetActiveItems();
        MoveFrame(PlayerPrefsHelper.GetInt(GlobalConst.CURRENT_COLOR_KEY));
    }

    private IEnumerator WaitTilPurchasingLoaded()
    {
        while (!CodelessIAPStoreListener.initializationComplete)
        {
            yield return null;
        }        
    }

    public void ResetActiveItems()
    {
        for (int i = 0; i < items.Length - 1; i++)
        {
            ShopItem item = items[i];
            item.Panel.SetActive(!item.IsUnlocked);
        }
    }

    public void BuyAllItems()
    {
        for (int i = 0; i < items.Length - 1; i++)
        {
            UnlockItem(i);
        }
    }

    public void ShowPurchaseFailed()
    {
        Debug.Log("Purchase failed");
    }

    public string GetItemCost(int id)
    {
        string s = items[id].Cost.text;
        Debug.Log(s);
        return s;
    }

    private void UnlockItem(int index)
    {
        if (!items[index].IsUnlocked)
        {
            items[index].IsUnlocked = true;
            items[index].Panel.SetActive(false);
        }
    }

    public void PickItem(int selectedItem)
    {
        if(selectedItem != items.Length - 1)
        {
            PlayerPrefsHelper.SetInt(GlobalConst.CURRENT_COLOR_KEY, selectedItem);
            StaticEventManager.CallOnBackgroundColorChanged(selectedItem);
            isRandom = false;
        }
        else
        {
            isRandom = true;
        }

        MoveFrame(selectedItem);
    }

    public int GetFirstLockedIndex()
    {
        int index = 0;
        for (int i = 0; i < items.Length; i++)
        {
            if (!items[i].IsUnlocked)
            {
                index = items[i].Id;
                return index;
            }                
        }
        return 0;
    }

    private void MoveFrame(int itemIndex)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (i == itemIndex)
                items[i].Frame.SetActive(true);
            else
                items[i].Frame.SetActive(false);
        }
    }

    public void SetNewBackground(int index)
    {
        if (!items[index].IsUnlocked && index < items.Length)
        {
            UnlockItem(index);
            StaticEventManager.CallOnBackgroundColorChanged(index);
            PlayerPrefsHelper.SetInt(GlobalConst.CURRENT_COLOR_KEY, index);
            StaticEventManager.CallOnBottleImageRefresh(index + 1);
            SoundPlayer.Play("openbottle", 1f);
        }
    }

    public void GetRandomBackground()
    {
        List<int> availableBackgrounds = new List<int>();

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].IsUnlocked)
                availableBackgrounds.Add(items[i].Id);
        }

        int randomColor = Random.Range(0, availableBackgrounds.Count);

        PlayerPrefsHelper.SetInt(GlobalConst.CURRENT_COLOR_KEY, randomColor);
        StaticEventManager.CallOnBackgroundColorChanged(randomColor);
    }
}

[System.Serializable]
public class ShopItem
{
    public int Id;
    public Sprite Image;
    public Text Cost;
    public GameObject Panel;
    public GameObject Frame;
    public Button PickButton;
    public int PointsToUnlock;
    public string Description;
    public bool IsUnlocked
    {
        get
        {
            return PlayerPrefsHelper.GetBool(SHOP_ITEM_KEY + Id);
        }
        set
        {
            PlayerPrefsHelper.SetBool(SHOP_ITEM_KEY + Id, value);
        }
    }

    public const string SHOP_ITEM_KEY = "isUnlocked";
}
