using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGrid : Grid
{
    private QuestUIManager questUIManager;

    public override void GetUIManager()
    {
        questUIManager = GetComponent<QuestUIManager>();
    }

    public override void OnUsed(Item_SO item)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateItemInfo(string itemDes)
    {
        questUIManager.UpdateItemInfo(itemDes);
    }
}
