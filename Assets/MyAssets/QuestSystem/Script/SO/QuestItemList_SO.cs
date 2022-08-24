using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New QuestItem", menuName = "Quest/New QuestItemList")]
public class QuestItemList_SO : ScriptableObject
{
    [SerializeField]
    private List<Item_SO> objectiveItemList = new List<Item_SO>();

    [SerializeField]
    private List<Item_SO> rewardItemList = new List<Item_SO>();
    public List<Item_SO> ObjectiveItemList
    {
        get { return objectiveItemList; }
        set { objectiveItemList = value; }
    }
    public List<Item_SO> RewardItemList
    {
        get { return rewardItemList; }
        set { rewardItemList = value; }
    }
}
