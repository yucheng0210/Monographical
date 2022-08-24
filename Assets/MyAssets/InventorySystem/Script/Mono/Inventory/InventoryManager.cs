using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryManager : MonoBehaviour
{
    public Inventory_SO myBag;

    public void Awake()
    {
        GetUIManager();
    }

    public abstract void GetUIManager();

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
        RefreshItem();
    }

    public void RemoveItem(Item_SO item)
    {
        UpdateItemInfo("");
        if (item.ItemHeld <= 0)
            myBag.ItemList.Remove(item);
        else
            item.ItemHeld--;
        RefreshItem();
    }

    /*public void RemoveAllItem()
    {
        UpdateItemInfo("");
        myBag.ItemList.RemoveAll();
        RefreshItem();
    }*/

    public void AddMoney(int moneyCount)
    {
        myBag.MoneyCount += moneyCount;
        RefreshItem();
    }

    public void ReduceMoney(int moneyCount)
    {
        myBag.MoneyCount -= moneyCount;
        RefreshItem();
    }

    public int GetMoney()
    {
        return myBag.MoneyCount;
    }

    public abstract void UpdateItemInfo(string itemDes);
    public abstract void RefreshItem();
}
