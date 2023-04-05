using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackManager : Singleton<BackpackManager>
{
    public static int abilityCount;

    public void AddItem(Item item)
    {
        if (!DataManager.Instance.Backpack.Contains(item))
        {
            DataManager.Instance.Backpack.Add(item);
            item.ItemHeld++;
        }
        else
            item.ItemHeld++;
        EventManager.Instance.DispatchEvent(EventDefinition.eventAddItemToBag);
    }

    public void RemoveItem(Item item)
    {
        if (item.ItemHeld <= 0)
            DataManager.Instance.Backpack.Remove(item);
        else
            item.ItemHeld--;
        EventManager.Instance.DispatchEvent(EventDefinition.eventRemoveItemToBag);
    }

    /*public void RemoveAllItem()
    {
        UpdateItemInfo("");
        myBag.ItemList.RemoveAll();
        RefreshItem();
    }*/

    public void AddMoney(int count)
    {
        DataManager.Instance.MoneyCount += count;
        EventManager.Instance.DispatchEvent(
            EventDefinition.eventReviseMoneyToBag,
            DataManager.Instance.MoneyCount
        );
    }

    public void ReduceMoney(int count)
    {
        DataManager.Instance.MoneyCount -= count;
        EventManager.Instance.DispatchEvent(
            EventDefinition.eventReviseMoneyToBag,
            DataManager.Instance.MoneyCount
        );
    }

    public int GetMoney()
    {
        return DataManager.Instance.MoneyCount;
    }
}
