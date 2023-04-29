using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public enum QuestState
    {
        Inactive, // 未接受任务
        Active, // 已接受任务
        Completed, // 任务完成
        Rewarded // 任务已领奖
    }

    public QuestState Status { get; set; }

    public void UpdateQuestState(int stateAmount)
    {
        switch (Status)
        {
            case QuestState.Inactive:
                break;
            case QuestState.Active:
                break;
            case QuestState.Completed:
                break;
            case QuestState.Rewarded:
                break;
        }
    }

    [SerializeField]
    private int id;

    [SerializeField]
    private string theName;

    [SerializeField]
    private string npc;

    [SerializeField]
    private string des;

    [SerializeField]
    private List<ValueTuple<int, int>> rewardList;

    [SerializeField]
    private List<ValueTuple<int, int>> targetList;

    [SerializeField]
    private int parent;
   #region "Read from Quest_SO"
    public int ID
    {
        get { return id; }
        set { id = value; }
    }
    public string TheName
    {
        get { return theName; }
        set { theName = value; }
    }
    public string NPC
    {
        get { return npc; }
        set { npc = value; }
    }
    public string Des
    {
        get { return des; }
        set { des = value; }
    }
    public List<ValueTuple<int, int>> RewardList
    {
        get { return rewardList; }
        set { rewardList = value; }
    }
    public List<ValueTuple<int, int>> TargetList
    {
        get { return targetList; }
        set { targetList = value; }
    }
    public int Parent
    {
        get { return parent; }
        set { parent = value; }
    }
   #endregion
}
