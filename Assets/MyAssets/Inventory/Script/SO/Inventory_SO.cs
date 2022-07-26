using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInventory", menuName = "Inventory/NewInventory")]
public class Inventory_SO : ScriptableObject
{
    [SerializeField]
    private List<Item_SO> itemList = new List<Item_SO>();
    public List<Item_SO> ItemList
    {
        get { return itemList; }
        set { itemList = value; }
    }
}
