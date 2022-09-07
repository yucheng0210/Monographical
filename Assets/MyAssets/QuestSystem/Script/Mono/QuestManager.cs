using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public QuestList_SO questList;

    public Inventory_SO backpack;

    private QuestUIManager questUIManager;

    public List<QuestItemList_SO> questItemList = new List<QuestItemList_SO>();

    private void Awake()
    {
        questUIManager = GetComponent<QuestUIManager>();
    }

    public void SetQuestActive(DialogList_SO dialogList, int index)
    {
        questList.QuestList[int.Parse(dialogList.DialogList[index].Order)].Status = 1;
        questUIManager.RefreshItem();
    }
}
