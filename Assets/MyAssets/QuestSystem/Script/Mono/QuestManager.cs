using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    public QuestList_SO questList;

    public Inventory_SO backpack;

    public List<QuestItemList_SO> questItemList = new List<QuestItemList_SO>();

    public List<Quest> ActiveQuestList { get; set; }

    protected override void Awake()
    {
        base.Awake();
        ActiveQuestList = new List<Quest>();
    }

    public void ActivateQuest(int questID)
    {
        if (!DataManager.Instance.QuestList.ContainsKey(questID))
            return;
        Quest quest = DataManager.Instance.GetQuest(questID);
        if (quest.Status == Quest.QuestState.Inactive)
        {
            quest.Status = Quest.QuestState.Active;
            if (!ActiveQuestList.Contains(quest))
                ActiveQuestList.Add(quest);
        }
    }

    public void UpdateActiveQuests()
    {
        for (int i = 0; i < ActiveQuestList.Count; i++)
        {
            if (
                DataManager.Instance.GetQuest(ActiveQuestList[i].ID).Status
                == Quest.QuestState.Completed
            )
                ActiveQuestList.Remove(ActiveQuestList[i]);
        }
    }

    public void CheckQuestProgress(int questID)
    {
        Quest quest = DataManager.Instance.GetQuest(questID);
        int targetCount = quest.TargetList.Count;
        for (int i = 0; i < quest.TargetList.Count; i++)
        {
            int targetIndex = quest.TargetList[i].Item1;
            int targetHeld = quest.TargetList[i].Item2;
            if (DataManager.Instance.GetItem(questID) == null)
                break;
            if (
                targetIndex == DataManager.Instance.GetItem(questID).ItemIndex
                && targetHeld <= DataManager.Instance.GetItem(questID).ItemHeld
            )
                targetCount--;
        }
        if (targetCount == 0)
            quest.Status = Quest.QuestState.Completed;
    }

    public void FinishQuest(int questID)
    {
        Quest quest = DataManager.Instance.GetQuest(questID);
        for (int i = 0; i < quest.TargetList.Count; i++)
        {
            int targetIndex = quest.TargetList[i].Item1;
            int targetHeld = quest.TargetList[i].Item2;
            for (int j = targetHeld; j > 0; j--)
            {
                BackpackManager.Instance.ReduceItem(targetIndex, DataManager.Instance.Backpack);
            }
        }
    }

    /*public void SetQuestActive(DialogList_SO dialogList, int index)
    {
        DataManager.Instance.QuestList[int.Parse(dialogList.DialogList[index].Order)].Status = Quest
            .QuestState
            .Active;
        questUIManager.RefreshItem();
    }*/

    public void GetRewards(int questID)
    {
        Quest quest = DataManager.Instance.GetQuest(questID);
        for (int i = 0; i < quest.RewardList.Count; i++)
        {
            int rewardIndex = quest.RewardList[i].Item1;
            int rewardHeld = quest.RewardList[i].Item2;
            for (int j = rewardHeld; j > 0; j--)
            {
                BackpackManager.Instance.AddItem(rewardIndex, DataManager.Instance.Backpack);
            }
        }
    }
}
