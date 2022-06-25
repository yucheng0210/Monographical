using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/NewItem")]
public class Item : ScriptableObject
{
    public enum itemAbility
    {
        Tonic,
        AttackUp
    }

    public itemAbility ability;
    public int itemAbilityNum;
    public string itemName;
    public Sprite itemImage;
    public int itemHeld;

    [TextArea]
    public string itemInfo;
    public bool equip;
}
