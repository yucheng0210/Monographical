using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : Singleton<ShopUIManager>
{
    [SerializeField]
    private GameObject gridManager;

    [SerializeField]
    private BackpackGrid gridPrefab;

    [SerializeField]
    private Text itemInfo;

    [SerializeField]
    private Inventory_SO myBag;

    [SerializeField]
    private Text moneyText;

    private void OnEnable()
    {
        RefreshItem();
        itemInfo.text = "";
    }

    public void UpdateItemInfo(string itemDes)
    {
        Instance.itemInfo.text = itemDes;
    }

    private void CreateNewItem(Item_SO item)
    {
        BackpackGrid newItem = Instantiate(
            Instance.gridPrefab,
            Instance.gridManager.transform.position,
            Quaternion.identity
        );
        newItem.gameObject.transform.SetParent(Instance.gridManager.transform, false);
        newItem.GridItem = item;
        newItem.GridImage.sprite = item.ItemImage;
        newItem.GridAmount.text = item.ItemHeld.ToString();
    }

    public void RefreshItem()
    {
        for (int i = 0; i < Instance.gridManager.transform.childCount; i++)
        {
            if (Instance.gridManager.transform.childCount == 0)
                break;
            Destroy(Instance.gridManager.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < myBag.ItemList.Count; i++)
        {
            if (myBag.ItemList[i].ItemHeld == 0)
            {
                myBag.ItemList.Remove(myBag.ItemList[i]);
                RefreshItem();
            }
            else
                CreateNewItem(myBag.ItemList[i]);
        }
        myBag.MoneyCount = BackpackManager.Instance.GetMoney();
        moneyText.text = myBag.MoneyCount.ToString();
    }

    public void OnUsed(Item_SO item)
    {
        if (myBag.MoneyCount >= item.ItemCost)
            myBag.MoneyCount -= item.ItemCost;
        BackpackManager.Instance.AddItem(item);
    }
}
