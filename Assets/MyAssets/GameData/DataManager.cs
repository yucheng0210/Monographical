using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class DataManager : Singleton<DataManager>, ISavable
{
    private readonly string itemDataListPath = Application.streamingAssetsPath + "/ITEMDATALIST.csv";
    private readonly string effectDataListPath = Application.streamingAssetsPath + "/EFFECTDATALIST.csv";
    private readonly string questDataListPath = Application.streamingAssetsPath + "/QUESTLIST.csv";
    private readonly string dialogDataListPath = Application.streamingAssetsPath + "/DialogData";
    private readonly string characterDataListPath = Application.streamingAssetsPath + "/CHARACTERLIST.csv";
    private readonly string weaponDataListPath = Application.streamingAssetsPath + "/WEAPONLIST.csv";
    public Dictionary<int, Item> Backpack { get; set; }
    public Dictionary<int, Item> ShopBag { get; set; }
    public Dictionary<int, Item> ItemList { get; set; }
    public Dictionary<string, Effect> EffectList { get; set; }
    //public Dictionary<string, CharacterState> CharacterList { get; set; }
    public Dictionary<int, Quest> QuestList { get; set; }
    public Dictionary<int, Item> ShortcutBar { get; set; }
    public Dictionary<string, List<Dialog>> DialogList { get; set; }
    public Dictionary<int, Character> CharacterList { get; set; }
    public Dictionary<int, Weapon> WeaponList { get; set; }
    public Dictionary<int, Weapon> WeaponBag { get; set; }
    public int MoneyCount { get; set; }

    protected override void Awake()
    {
        base.Awake();
        Backpack = new Dictionary<int, Item>();
        ShopBag = new Dictionary<int, Item>();
        ItemList = new Dictionary<int, Item>();
        EffectList = new Dictionary<string, Effect>();
        //CharacterList = new Dictionary<string, CharacterState>();
        ShortcutBar = new Dictionary<int, Item>();
        QuestList = new Dictionary<int, Quest>();
        DialogList = new Dictionary<string, List<Dialog>>();
        CharacterList = new Dictionary<int, Character>();
        WeaponList = new Dictionary<int, Weapon>();
        WeaponBag = new Dictionary<int, Weapon>();
        LoadData();
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        ISavable savable = this;
        savable.AddSavableRegister();
        BackpackManager.Instance.AddMoney(1000);
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
            item.ItemEffectType = new List<(string, int)>();
            if (row[5] != "")
            {
                string[] effects = row[5].Split(';');
                for (int j = 0; j < effects.Length; j++)
                {
                    string[] effect = effects[j].Split('=');
                    string id;
                    int count;
                    id = effect[0];
                    if (int.TryParse(effect[1], out count))
                        item.ItemEffectType.Add(new ValueTuple<string, int>(id, count));
                }
            }
            item.ItemIndex = int.Parse(row[6]);
            item.ItemTarget = row[7];
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
            item.ItemEffectType = new List<(string, int)>();
            if (row[5] != "")
            {
                string[] effects = row[5].Split(';');
                for (int j = 0; j < effects.Length; j++)
                {
                    string[] effect = effects[j].Split('=');
                    string id;
                    int count;
                    id = effect[0];
                    if (int.TryParse(effect[1], out count))
                        item.ItemEffectType.Add(new ValueTuple<string, int>(id, count));
                }
            }
            item.ItemIndex = int.Parse(row[6]);
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
        #region 任務列表
        lineData = File.ReadAllLines(questDataListPath);
        for (int i = 1; i < lineData.Length; i++)
        {
            string[] row = lineData[i].Split(',');
            Quest quest = new Quest();
            quest.ID = int.Parse(row[0]);
            quest.TheName = row[1];
            quest.NPC = row[2];
            quest.Des = row[3];
            quest.RewardList = new List<(int, int)>();
            quest.TargetList = new List<(int, int)>();
            quest.TargetEnemyList = new List<(int, int)>();
            string[] rewards = row[4].Split(';');
            for (int j = 0; j < rewards.Length; j++)
            {
                string[] reward = rewards[j].Split('=');
                int id,
                    count;
                if (int.TryParse(reward[0], out id) && int.TryParse(reward[1], out count))
                    quest.RewardList.Add(new ValueTuple<int, int>(id, count));
            }
            string[] targets = row[5].Split(';');
            for (int j = 0; j < targets.Length; j++)
            {
                string[] target = targets[j].Split('=');
                int id,
                    count;
                if (int.TryParse(target[0], out id) && int.TryParse(target[1], out count))
                    quest.TargetList.Add(new ValueTuple<int, int>(id, count));
            }
            string[] targetEnemies = row[6].Split(';');
            for (int j = 0; j < targetEnemies.Length; j++)
            {
                string[] targetEnemy = targetEnemies[j].Split('=');
                int id,
                    count;
                if (int.TryParse(targetEnemy[0], out id) && int.TryParse(targetEnemy[1], out count))
                    quest.TargetEnemyList.Add(new ValueTuple<int, int>(id, count));
            }
            quest.Parent = int.Parse(row[7]);
            quest.Status = Quest.QuestState.Inactive;
            quest.IsNewQuest = true;
            QuestList.Add(quest.ID, quest);
        }
        #endregion
        #region 對話列表
        foreach (string file in Directory.GetFiles(dialogDataListPath, "*.csv"))
        {
            lineData = File.ReadAllLines(file);
            List<Dialog> dialogs = new List<Dialog>();
            for (int i = 1; i < lineData.Length; i++)
            {
                string[] row = lineData[i].Split(',');
                Dialog dialog = new Dialog();
                dialog.Branch = row[0];
                dialog.Type = row[1];
                dialog.TheName = row[2];
                dialog.Order = row[3];
                dialog.Content = row[4];
                dialogs.Add(dialog);
            }
            string fileName = Path.GetFileNameWithoutExtension(file);
            DialogList.Add(fileName, dialogs);
        }
        #endregion
        #region 角色列表
        lineData = File.ReadAllLines(characterDataListPath);
        for (int i = 1; i < lineData.Length; i++)
        {
            string[] row = lineData[i].Split(',');
            Character character = new Character
            {
                CharacterID = int.Parse(row[0]),
                CharacterName = row[1],
                MaxHealth = int.Parse(row[2]),
                BaseDefence = int.Parse(row[3]),
                MaxPoise = int.Parse(row[4]),
                MinAttack = int.Parse(row[5]),
                MaxAttack = int.Parse(row[6]),
                CriticalMultiplier = float.Parse(row[7]),
                CriticalChance = float.Parse(row[8]),
                PoiseAttack = int.Parse(row[9])
            };
            CharacterList.Add(character.CharacterID, character);
        }
        #endregion
        #region 武器列表
        lineData = File.ReadAllLines(weaponDataListPath);
        for (int i = 1; i < lineData.Length; i++)
        {
            string[] row = lineData[i].Split(',');
            Weapon weapon = new Weapon
            {
                WeaponType = row[0],
                WeaponID = int.Parse(row[1]),
                WeaponRare = row[2],
                WeaponName = row[3],
                WeaponImagePath = row[4],
                WeaponAttribute = row[5],
                WeaponAttack = int.Parse(row[6]),
                WeaponEffect = row[7],
                WeaponCriticalChance = float.Parse(row[8]),
                WeaponCriticalMultiplier = float.Parse(row[9]),
                WeaponDescription = row[10],
                WeaponHitStopTime = float.Parse(row[11]),
                WeaponHeld = 0
            };
            WeaponList.Add(weapon.WeaponID, weapon);
        }
        #endregion
    }

    public Quest GetQuest(int questID)
    {
        if (QuestList.ContainsKey(questID))
            return QuestList[questID];
        else
            return null;
    }

    public Item GetItem(int itemID)
    {
        if (Backpack.ContainsKey(itemID))
            return Backpack[itemID];
        else
            return null;
    }

    /*public void AddCharacterRegister(CharacterState character)
    {
        CharacterList.Add(character.CharacterName, character);
    }*/

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
