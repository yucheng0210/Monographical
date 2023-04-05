using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataManager : Singleton<DataManager>, ISavable
{
    private string itemDataListPath =
        "Assets/MyAssets/InventorySystem/ItemDatas/BackpackData/ITEMDATALIST.csv";
    public List<Item> Backpack { get; set; }
    public Dictionary<int, Item> ItemList { get; set; }
    public int MoneyCount { get; set; }

    protected override void Awake()
    {
        base.Awake();
        Backpack = new List<Item>();
        ItemList = new Dictionary<int, Item>();
        MoneyCount = 0;
        LoadData();
    }

    private void Start()
    {
        ISavable savable = this;
        savable.AddSavableRegister();
    }

    private void LoadData()
    {
        Backpack.Clear();
        ItemList.Clear();
        string[] lineData = File.ReadAllLines(itemDataListPath);
        for (int i = 1; i < lineData.Length; i++)
        {
            string[] row = lineData[i].Split(',');
            Item item = new Item();
            item.ItemName = row[0];
            item.ItemImagePath = row[1];
            item.ItemInfo = row[2];
            item.ItemBuyPrice = int.Parse(row[3]);
            item.ItemSellPrice = int.Parse(row[4]);
            item.ItemType = row[5];
            item.ItemIndex = int.Parse(row[6]);
            item.ItemHeld = 0;
            ItemList.Add(item.ItemIndex, item);
            Backpack.Add(item);
        }
        //EventManager.Instance.DispatchEvent(EventDefinition.eventLoadDataFinish);
    }

    public void AddSavableRegister()
    {
        SaveLoadManager.Instance.AddRegister(this);
    }

    public GameSaveData GenerateGameData()
    {
        GameSaveData gameSaveData = new GameSaveData();
        gameSaveData.backpack = Backpack;
        gameSaveData.moneyCount = MoneyCount;
        return gameSaveData;
    }

    public void RestoreGameData(GameSaveData gameSaveData)
    {
        Debug.Log("restore");
        Backpack = gameSaveData.backpack;
        MoneyCount = gameSaveData.moneyCount;
    }
}
