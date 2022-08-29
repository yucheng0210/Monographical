using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New QuestReward", menuName = "Quest/New QuestReward")]
public class QuestReward_SO : ScriptableObject
{
    [SerializeField]
    private Item_SO rewardItem;

    [SerializeField]
    private int itemHeld;
    public Item_SO RewardItem
    {
        get { return rewardItem; }
        set { rewardItem = value; }
    }
    public int ItemHeld
    {
        get { return itemHeld; }
        set { itemHeld = value; }
    }
}
