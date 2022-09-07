using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New QuestReward", menuName = "Quest/New QuestReward")]
public class QuestReward_SO : ScriptableObject
{
    [SerializeField]
    private Item_SO inBackpackItem;

    [SerializeField]
    private int itemHeld;
    public Item_SO InBackpackItem
    {
        get { return inBackpackItem; }
        set { inBackpackItem = value; }
    }
    public int ItemHeld
    {
        get { return itemHeld; }
        set { itemHeld = value; }
    }
}
