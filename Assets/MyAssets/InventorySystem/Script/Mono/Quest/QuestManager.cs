using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : InventoryManager
{
    private QuestUIManager questUIManager;

    public override void GetUIManager()
    {
        questUIManager = GetComponent<QuestUIManager>();
    }

    public override void RefreshItem()
    {
        questUIManager.RefreshItem(MyBag);
    }

    public override void UpdateItemInfo(string itemDes)
    {
        questUIManager.UpdateItemInfo(itemDes);
    }
}
