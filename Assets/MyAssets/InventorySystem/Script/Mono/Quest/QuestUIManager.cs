using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestUIManager : InventoryUIManager
{
    private QuestManager questManager;
    public override void GetManager()
    {
       questManager=GetComponent<QuestManager>();
    }

    public override void OnUsed(Item_SO item)
    {
        throw new System.NotImplementedException();
    }
}
