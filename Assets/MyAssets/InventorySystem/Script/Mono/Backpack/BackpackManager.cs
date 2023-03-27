using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackManager : Singleton<BackpackManager>
{
    public static int abilityCount;

    private int moneyCount;
    public Dictionary<int, Item_SO> Backpack { get; set; }

    protected override void Awake()
    {
        base.Awake();
        Backpack = new Dictionary<int, Item_SO>();
    }

    public void AddItem(Item_SO item)
    {
        if (!Backpack.ContainsValue(item))
        {
            Backpack.Add(item.ItemIndex, item);
            item.ItemHeld++;
        }
        else
            item.ItemHeld++;
        EventManager.Instance.DispatchEvent(EventDefinition.eventAddItemToBag);
    }

    public void RemoveItem(Item_SO item)
    {
        if (item.ItemHeld <= 0)
            Backpack.Remove(item.ItemIndex);
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
        moneyCount += count;
        EventManager.Instance.DispatchEvent(EventDefinition.eventReviseMoneyToBag);
    }

    public void ReduceMoney(int count)
    {
        moneyCount -= count;
        EventManager.Instance.DispatchEvent(EventDefinition.eventReviseMoneyToBag);
    }

    public int GetMoney()
    {
        return moneyCount;
    }
}
