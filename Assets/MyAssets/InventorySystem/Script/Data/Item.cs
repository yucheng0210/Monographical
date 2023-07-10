using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/NewItem")]
public class Item
{
    [SerializeField]
    private int itemIndex;

    [SerializeField]
    private string itemName;

    [SerializeField]
    private Sprite itemImage;

    [SerializeField]
    private int itemHeld;

    [SerializeField]
    [TextArea]
    private string itemInfo;

    [SerializeField]
    private bool equip;

    [SerializeField]
    private int itemCost;

    #region Read from Item_SO
    public int ItemIndex
    {
        get { return itemIndex; }
        set { itemIndex = value; }
    }

    public string ItemName
    {
        get { return itemName; }
        set { itemName = value; }
    }
    public Sprite ItemImage
    {
        get { return itemImage; }
        set { itemImage = value; }
    }
    public int ItemHeld
    {
        get { return itemHeld; }
        set
        {
            itemHeld = value;
            if (itemHeld > 99)
                itemHeld = 99;
        }
    }
    public string ItemInfo
    {
        get { return itemInfo; }
        set { itemInfo = value; }
    }
    public bool ItemEquip
    {
        get { return equip; }
        set { equip = value; }
    }
    public int ItemBuyPrice { get; set; }
    public int ItemSellPrice { get; set; }
    public List<ValueTuple<string, int>> ItemEffectType { get; set; }
    public int ItemCost
    {
        get { return itemCost; }
        set
        {
            itemCost = value;
            if (itemCost < 0)
                itemCost = 0;
        }
    }
    public string ItemTarget { get; set; }
    #endregion
}
