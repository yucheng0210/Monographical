using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataManager : MonoBehaviour
{
    private string itemDataListPath =
        "Assets/MyAssets/InventorySystem/ItemDatas/BackpackData/ITEMDATALIST.csv";

    private void Start()
    {
        LoadData();
    }

    private void LoadData()
    {
        BackpackManager.Instance.Backpack.Clear();
        string[] lineData = File.ReadAllLines(itemDataListPath);
        for (int i = 1; i < lineData.Length - 1; i++)
        {
            string[] row = lineData[i].Split(',');
            Item_SO item = ScriptableObject.CreateInstance<Item_SO>();
            item.ItemName = row[0];
            item.ItemImagePath = row[1];
            item.ItemInfo = row[2];
            item.ItemBuyPrice = int.Parse(row[3]);
            item.ItemSellPrice = int.Parse(row[4]);
            item.ItemType = row[5];
            item.ItemIndex = int.Parse(row[6]);
            BackpackManager.Instance.Backpack.Add(item.ItemIndex, item);
        }
//        EventManager.Instance.DispatchEvent(EventDefinition.eventLoadDataFinish);
    }
}
