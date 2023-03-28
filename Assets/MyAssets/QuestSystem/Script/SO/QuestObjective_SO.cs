using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New QuestObjective", menuName = "Quest/New QuestObjective")]
public class QuestObjective_SO : ScriptableObject
{
    [SerializeField]
    private Item inBackpackItem;

    [SerializeField]
    private int itemHeld;
    public Item InBackpackItem
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
