using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    [SerializeField]
    private Inventory_SO myBag;
    public static int abilityCount;

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
}
