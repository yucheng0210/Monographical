using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    public QuestList_SO questList;

    public Inventory_SO backpack;

    private QuestUIManager questUIManager;

    public List<QuestItemList_SO> questItemList = new List<QuestItemList_SO>();

    public List<int> ActiveQuestList { get; set; }

    protected override void Awake()
    {
        base.Awake();
        questUIManager = GetComponent<QuestUIManager>();
    }

    public void ActivateQuest(int questID)
    {
        if (!DataManager.Instance.QuestList.ContainsKey(questID))
            return;
        Quest quest = GetQuest(questID);
        if (quest.Status == Quest.QuestState.Inactive)
        {
            quest.Status = Quest.QuestState.Active;
            if (!ActiveQuestList.Contains(questID))
                ActiveQuestList.Add(questID);
        }
    }

    public void UpdateActiveQuests()
    {
        for (int i = 0; i < ActiveQuestList.Count; i++)
        {
            if (GetQuest(ActiveQuestList[i]).Status == Quest.QuestState.Completed)
                ActiveQuestList.Remove(i);
        }
    }

    public void CheckQuestProgress(int questID)
    {
        Quest quest = GetQuest(questID);
        for (int i = 0; i < quest.TargetList.Count; i++)
        {
            if (quest.TargetList[i].Item1 == BackpackManager.Instance.GetItem(questID).ItemIndex)
                quest.TargetList.Remove(quest.TargetList[i]);
        }
        if (quest.TargetList.Count == 0)
            quest.Status = Quest.QuestState.Completed;
    }

    public void SetQuestActive(DialogList_SO dialogList, int index)
    {
        DataManager.Instance.QuestList[int.Parse(dialogList.DialogList[index].Order)].Status = Quest
            .QuestState
            .Active;
        questUIManager.RefreshItem();
    }

    public Quest GetQuest(int questID)
    {
        if (DataManager.Instance.QuestList.ContainsKey(questID))
            return DataManager.Instance.QuestList[questID];
        else
        {
            Debug.LogWarning("找不到任務 " + questID);
            return null;
        }
    }
}
