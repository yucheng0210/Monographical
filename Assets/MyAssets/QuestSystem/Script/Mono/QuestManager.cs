using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public QuestList_SO questList;

    public Inventory_SO backpack;

    private QuestUIManager questUIManager;

    public Inventory_SO rewardInventory;

    public Inventory_SO objectiveInventory;

    private void Awake()
    {
        questUIManager = GetComponent<QuestUIManager>();
    }

    public void SetQuestActive(DialogList_SO dialogList, int index)
    {
        questList.QuestList[int.Parse(dialogList.DialogList[index].Order)].Status = 1;
        questUIManager.RefreshItem();
    }

    public bool GetQuestState(Item_SO objectiveItem, Item_SO backItem)
    {
        return backItem.ItemHeld >= objectiveItem.ItemHeld;
    }
}
