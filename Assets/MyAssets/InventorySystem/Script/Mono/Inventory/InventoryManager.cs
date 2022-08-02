using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryManager : Singleton<InventoryManager>
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
        InventoryUIManager.Instance.RefreshItem();
    }

    public void RemoveItem(Item_SO item)
    {
        InventoryUIManager.Instance.UpdateItemInfo("");
        if (item.ItemHeld <= 0)
            myBag.ItemList.Remove(item);
        else
            item.ItemHeld--;
        InventoryUIManager.Instance.RefreshItem();
    }

    public void AddMoney(int moneyCount)
    {
        myBag.MoneyCount += moneyCount;
        InventoryUIManager.Instance.RefreshItem();
    }
}
