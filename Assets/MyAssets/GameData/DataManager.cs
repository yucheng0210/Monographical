using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataManager : Singleton<DataManager>, ISavable
{
    private string itemDataListPath =
        "Assets/MyAssets/InventorySystem/ItemDatas/BackpackData/ITEMDATALIST.csv";
    private string effectDataListPath =
        "Assets/MyAssets/InventorySystem/ItemDatas/BackpackData/EFFECTDATALIST.csv";
    public Dictionary<int, Item> Backpack { get; set; }
    public Dictionary<int, Item> ShopBag { get; set; }
    public Dictionary<int, Item> ItemList { get; set; }
    public Dictionary<string, Effect> EffectList { get; set; }
    public Dictionary<string, CharacterState> CharacterList { get; set; }
    public Dictionary<int, Item> ShortcutBar { get; set; }
    public int MoneyCount { get; set; }

    protected override void Awake()
    {
        base.Awake();
        Backpack = new Dictionary<int, Item>();
        ShopBag = new Dictionary<int, Item>();
        ItemList = new Dictionary<int, Item>();
        EffectList = new Dictionary<string, Effect>();
        CharacterList = new Dictionary<string, CharacterState>();
        ShortcutBar = new Dictionary<int, Item>();
        MoneyCount = 1000;
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
        EffectList.Clear();
        #region 物品列表
        string[] lineData = File.ReadAllLines(itemDataListPath);
        for (int i = 1; i < lineData.Length; i++)
        {
            string[] row = lineData[i].Split(',');
            Item item = new Item();
            item.ItemName = row[0];
            item.ItemImage = Resources.Load<Sprite>(row[1]);
            item.ItemInfo = row[2];
            item.ItemBuyPrice = int.Parse(row[3]);
            item.ItemSellPrice = int.Parse(row[4]);
            item.ItemEffectType = row[5];
            item.ItemIndex = int.Parse(row[6]);
            item.ItemEffectName = row[7];
            item.ItemHeld = 0;
            ItemList.Add(item.ItemIndex, item);
        }
        #endregion
        #region 商品背包
        lineData = File.ReadAllLines(itemDataListPath);
        for (int i = 1; i < lineData.Length; i++)
        {
            string[] row = lineData[i].Split(',');
            Item item = new Item();
            item.ItemName = row[0];
            item.ItemImage = Resources.Load<Sprite>(row[1]);
            item.ItemInfo = row[2];
            item.ItemBuyPrice = int.Parse(row[3]);
            item.ItemSellPrice = int.Parse(row[4]);
            item.ItemEffectType = row[5];
            item.ItemIndex = int.Parse(row[6]);
            item.ItemEffectName = row[7];
            item.ItemHeld = 5;
            ShopBag.Add(item.ItemIndex, item);
        }
        #endregion
        #region 物品效果
        lineData = File.ReadAllLines(effectDataListPath);
        for (int i = 1; i < lineData.Length; i++)
        {
            string[] row = lineData[i].Split(',');
            Effect effect = new Effect();
            effect.EffectName = row[0];
            effect.EffectValue = int.Parse(row[1]);
            effect.EffectTarget = row[2];
            EffectList.Add(effect.EffectName, effect);
        }
        #endregion

        //EventManager.Instance.DispatchEvent(EventDefinition.eventLoadDataFinish);
    }

    public void AddCharacterRegister(CharacterState character)
    {
        CharacterList.Add(character.CharacterName, character);
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
        Backpack = gameSaveData.backpack;
        MoneyCount = gameSaveData.moneyCount;
    }
}
