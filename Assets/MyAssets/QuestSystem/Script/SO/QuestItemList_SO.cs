using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New QuestItem", menuName = "Quest/New QuestItemList")]
public class QuestItemList_SO : ScriptableObject
{
    [SerializeField]
    private List<QuestObjective_SO> objectiveItemList = new List<QuestObjective_SO>();

    [SerializeField]
    private List<QuestReward_SO> rewardItemList = new List<QuestReward_SO>();

    [SerializeField]
    private int rewardMoney;

    [SerializeField]
    private int objectiveMoney;
    public List<QuestObjective_SO> ObjectiveItemList
    {
        get { return objectiveItemList; }
        set { objectiveItemList = value; }
    }
    public List<QuestReward_SO> RewardItemList
    {
        get { return rewardItemList; }
        set { rewardItemList = value; }
    }
    public int RewardMoney
    {
        get { return rewardMoney; }
        set { rewardMoney = value; }
    }
    public int ObjectiveMoney
    {
        get { return objectiveMoney; }
        set { objectiveMoney = value; }
    }
}
