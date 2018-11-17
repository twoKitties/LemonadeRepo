using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Purchasing;

public class ItemsChecker : MonoBehaviour{

    public ShopController shopController;
    public AdsController adsController;

    private void Awake()
    {
        bool isBought = PlayerPrefsHelper.GetBool(GlobalConst.ITEM_PURCHASED_KEY);
        if(!isBought)
        {
            StartCoroutine(StoreInit());
        }
    }

    private IEnumerator StoreInit()
    {
        while (!CodelessIAPStoreListener.initializationComplete)
            yield return null;
#if UNITY_ANDROID || UNITY_IOS
        Product product = CodelessIAPStoreListener.Instance.GetProduct(GlobalConst.ITEM_ID_KEY);
        if(product != null && product.hasReceipt)
        {
            shopController.BuyAllItems();

            adsController.DisableAds();
            }
#endif

    }

    public void SaveData()
    {
        PlayerPrefsHelper.SetBool(GlobalConst.ITEM_PURCHASED_KEY, true);
    }
}
