using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInventory", menuName = "Inventory/NewInventory")]
public class Inventory_SO : ScriptableObject
{
    [SerializeField]
    private List<Item_SO> itemList = new List<Item_SO>();

    [SerializeField]
    private int moneyCount;
    public List<Item_SO> ItemList
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
