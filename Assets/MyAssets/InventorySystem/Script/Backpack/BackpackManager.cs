using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackManager : Singleton<BackpackManager>
{
    private IEffectFactory effectFactory;

    public void RegisterFactory(IEffectFactory effectFactory)
    {
        this.effectFactory = effectFactory;
    }

    public void UseItem(int itemIndex)
    {
        ReduceItem(itemIndex, DataManager.Instance.Backpack);
        IEffect effect = effectFactory.CreateEffect(
            DataManager.Instance.ItemList[itemIndex].ItemEffectType
        );
        effect.ApplyEffect(
            DataManager.Instance.EffectList[
                DataManager.Instance.ItemList[itemIndex].ItemEffectName
            ].EffectTarget,
            DataManager.Instance.EffectList[
                DataManager.Instance.ItemList[itemIndex].ItemEffectName
            ].EffectValue
        );
    }

    public Item GetItem(int itemID)
    {
        if (DataManager.Instance.QuestList.ContainsKey(itemID))
            return DataManager.Instance.Backpack[itemID];
        else
        {
            Debug.LogWarning("找不到物品 " + itemID);
            return null;
        }
    }

    public void AddItem(int itemIndex, Dictionary<int, Item> inventory)
    {
        if (!inventory.ContainsKey(itemIndex))
        {
            inventory.Add(itemIndex, DataManager.Instance.ItemList[itemIndex]);
            inventory[itemIndex].ItemHeld++;
        }
        else
            inventory[itemIndex].ItemHeld++;
        EventManager.Instance.DispatchEvent(EventDefinition.eventAddItemToBag);
    }

    public void ReduceItem(int itemIndex, Dictionary<int, Item> inventory)
    {
        inventory[itemIndex].ItemHeld--;
        if (inventory[itemIndex].ItemHeld <= 0)
            inventory.Remove(itemIndex);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRemoveItemToBag);
    }

    public void SetShortcutBar(Item item)
    {
        if (!DataManager.Instance.ShortcutBar.ContainsKey(item.ItemIndex))
            DataManager.Instance.ShortcutBar.Add(item.ItemIndex, item);
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
