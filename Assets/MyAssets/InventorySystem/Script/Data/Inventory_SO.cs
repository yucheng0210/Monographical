using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInventory", menuName = "Inventory/NewInventory")]
public class Inventory_SO : ScriptableObject
{
    [SerializeField]
    private List<Item> itemList = new List<Item>();

    [SerializeField]
    private int moneyCount;
    public List<Item> ItemList
    {
        get { return itemList; }
        set { itemList = value; }
    }
    public int MoneyCount
    {
        get { return moneyCount; }
        set
        {
            moneyCount = value;
            if (moneyCount < 0)
                moneyCount = 0;
        }
    }
}
