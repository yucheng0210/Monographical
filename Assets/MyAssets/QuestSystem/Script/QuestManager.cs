using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    public List<Quest> ActiveQuestList { get; set; }
    public Dictionary<int, int> QuestCurrentKill { get; set; }

    protected override void Awake()
    {
        base.Awake();
        ActiveQuestList = new List<Quest>();
        QuestCurrentKill = new Dictionary<int, int>();
    }
    public void AddQuestCurrentKill(int id)
    {
        if (QuestCurrentKill.ContainsKey(id))
            QuestCurrentKill[id] += 1;
        else
            QuestCurrentKill.Add(id, 1);
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
            if (DataManager.Instance.GetQuest(ActiveQuestList[i].ID).Status == Quest.QuestState.Completed)
                ActiveQuestList.Remove(ActiveQuestList[i]);
        }
    }

    public void CheckQuestProgress(int questID)
    {
        Quest quest = DataManager.Instance.GetQuest(questID);
        if (!ActiveQuestList.Contains(quest))
            return;
        int targetCount = quest.TargetList.Count;
        int targetEnemyCount = quest.TargetEnemyList.Count;
        quest.UpdateQuestState();
        for (int i = 0; i < quest.TargetList.Count; i++)
        {
            int targetIndex = quest.TargetList[i].Item1;
            int targetHeld = quest.TargetList[i].Item2;
            if (!DataManager.Instance.Backpack.ContainsKey(targetIndex))
                break;
            int itemHeld = DataManager.Instance.GetItem(questID).ItemHeld;
            if (targetHeld <= itemHeld)
                targetCount--;
        }
        for (int i = 0; i < quest.TargetEnemyList.Count; i++)
        {
            int targetID = quest.TargetEnemyList[i].Item1;
            int targetHeld = quest.TargetEnemyList[i].Item2;
            if (!QuestCurrentKill.ContainsKey(targetID))
                break;
            int killCount = QuestCurrentKill[targetID];
            if (targetHeld <= killCount)
                targetEnemyCount--;
        }
        if (targetCount == 0 && targetEnemyCount == 0)
        {
            FinishQuest(questID);
            quest.Status = Quest.QuestState.Completed;
        }
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
        QuestCurrentKill.Clear();
    }

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
