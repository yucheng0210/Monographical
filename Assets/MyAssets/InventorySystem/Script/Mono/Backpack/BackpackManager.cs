using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackManager : InventoryManager
{
    public static int abilityCount;

    private BackpackUIManager UIManager;
    private QuestUIManager qusetUIManager;

    public override void GetUIManager()
    {
        UIManager = GetComponent<BackpackUIManager>();
    }

    public override void RefreshItem()
    {
        UIManager.RefreshItem(MyBag);
    }

    public override void UpdateItemInfo(string itemDes)
    {
        UIManager.UpdateItemInfo(itemDes);
    }
}
