using UnityEngine;

public class BackgroundPicker : MonoBehaviour
{
    [SerializeField]
    private GameObject[] panels;
    [SerializeField]
    private ShopController shopController;

    private void OnEnable()
    {
        GetInfoFromShop();
    }

    private void GetInfoFromShop()
    {
        if (shopController != null)
        {
            ShopItem[] items = shopController.items;

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].IsUnlocked)
                    panels[i].SetActive(false);
            }

            Debug.Log("Items unlocked");
        }
        else
            Debug.LogWarning("ShopController not found");
    }

    public void PickItem(int selectedItem)
    {
        shopController.PickItem(selectedItem);
    }
}
