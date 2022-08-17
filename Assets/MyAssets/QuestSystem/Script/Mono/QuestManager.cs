using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField]
    private QuestList_SO questList;

    public Inventory_SO backpack;

    private QusetUIManager qusetUIManager;

    public Inventory_SO rewardInventory;

    public Inventory_SO objectiveInventory;

    private void Awake()
    {
        qusetUIManager = GetComponent<QusetUIManager>();
    }

    public void QuestActive(DialogSystem dialogSystem, int index)
    {
        Debug.Log(int.Parse(dialogSystem.DialogList[index].Order));
        questList.QuestList[int.Parse(dialogSystem.DialogList[index].Order)].Status = 1;
        qusetUIManager.RefreshItem();
    }

    public void GetReward()
    {
        foreach (Item_SO i in rewardInventory.ItemList)
            Debug.Log("");
    }
}
