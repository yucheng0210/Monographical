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

    #region Read from Item_SO
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
    #endregion
}
