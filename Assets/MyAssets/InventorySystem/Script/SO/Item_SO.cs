using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/NewItem")]
public class Item_SO : ScriptableObject
{
    public enum ItemAbility
    {
        Tonic,
        AttackUp
    }

    [SerializeField]
    private int itemIndex;

    [SerializeField]
    private int itemAbilityNum;

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

    [SerializeField]
    private bool itemInShop;

    [SerializeField]
    private Item_SO itemInOther;

    #region Read from Item_SO
    public int ItemIndex
    {
        get { return itemIndex; }
    }
    public ItemAbility itemAbility;
    public int ItemAbilityNum
    {
        get { return itemAbilityNum; }
    }
    public string ItemName
    {
        get { return itemName; }
    }
    public Sprite ItemImage
    {
        get { return itemImage; }
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
    }
    public bool ItemEquip
    {
        get { return equip; }
        set { equip = value; }
    }
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
    public bool ItemInShop
    {
        get { return itemInShop; }
    }
    public Item_SO ItemInOther
    {
        get { return itemInOther; }
    }
    #endregion
}
