using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public QuestList_SO questList;

    public Inventory_SO backpack;

    private QusetUIManager qusetUIManager;

    public Inventory_SO rewardInventory;

    public Inventory_SO objectiveInventory;

    private void Awake()
    {
        qusetUIManager = GetComponent<QusetUIManager>();
    }

    public void SetQuestActive(DialogList_SO dialogList, int index)
    {
        //Debug.Log(int.Parse(dialogSystem.DialogList[index].Order));
        questList.QuestList[int.Parse(dialogList.DialogList[index].Order)].Status = 1;
        qusetUIManager.RefreshItem();
    }

    public bool GetQuestState(Item_SO objectiveItem, Item_SO backItem)
    {
        return backItem.ItemHeld >= objectiveItem.ItemHeld;
    }

    public void GetReward()
    {
        foreach (Item_SO i in rewardInventory.ItemList)
            Debug.Log("");
    }
}
