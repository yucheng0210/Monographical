using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackManager : Singleton<BackpackManager>, ISavable
{
    public static int abilityCount;

    private int moneyCount;
    public List<Item> Backpack { get; set; }
    public Dictionary<int, Item> ItemList { get; set; }

    protected override void Awake()
    {
        base.Awake();
        Backpack = new List<Item>();
        ItemList = new Dictionary<int, Item>();
    }

    private void Start()
    {
        ISavable savable = this;
        savable.AddSavableRegister();
    }

    public void AddItem(Item item)
    {
        if (!Backpack.Contains(item))
        {
            Backpack.Add(item);
            item.ItemHeld++;
        }
        else
            item.ItemHeld++;
        EventManager.Instance.DispatchEvent(EventDefinition.eventAddItemToBag);
    }

    public void RemoveItem(Item item)
    {
        if (item.ItemHeld <= 0)
            Backpack.Remove(item);
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
        EventManager.Instance.DispatchEvent(EventDefinition.eventReviseMoneyToBag, moneyCount);
    }

    public void ReduceMoney(int count)
    {
        moneyCount -= count;
        EventManager.Instance.DispatchEvent(EventDefinition.eventReviseMoneyToBag, moneyCount);
    }

    public int GetMoney()
    {
        return moneyCount;
    }

    public void AddSavableRegister()
    {
        SaveLoadManager.Instance.AddRegister(this);
    }

    public GameSaveData GenerateGameData()
    {
        GameSaveData gameSaveData = new GameSaveData();
        gameSaveData.backpack = Backpack;
        gameSaveData.moneyCount = moneyCount;
        return gameSaveData;
    }

    public void RestoreGameData(GameSaveData gameSaveData)
    {
        Debug.Log("restore");
        Backpack = gameSaveData.backpack;
        moneyCount = gameSaveData.moneyCount;
    }
}
