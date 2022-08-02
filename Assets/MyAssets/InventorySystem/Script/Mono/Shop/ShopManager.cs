using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
    [SerializeField]
    private Inventory_SO myBag;

    public void AddItem(Item_SO item)
    {
        if (!myBag.ItemList.Contains(item))
        {
            myBag.ItemList.Add(item);
            item.ItemHeld++;
            //InventoryManager.CreateNewItem(thisItem);
        }
        else
            item.ItemHeld++;
        ShopUIManager.Instance.RefreshItem();
    }

    public void RemoveItem(Item_SO item)
    {
        ShopUIManager.Instance.UpdateItemInfo("");
        if (item.ItemHeld <= 0)
            myBag.ItemList.Remove(item);
        else
            item.ItemHeld--;
        ShopUIManager.Instance.RefreshItem();
    }

    public void AddMoney(int moneyCount)
    {
        myBag.MoneyCount += moneyCount;
        ShopUIManager.Instance.RefreshItem();
    }
}
