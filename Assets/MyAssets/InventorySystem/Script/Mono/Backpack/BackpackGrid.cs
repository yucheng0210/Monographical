using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackpackGrid : Grid
{
    private BackpackUIManager UIManager;

    public override void GetUIManager()
    {
        UIManager = FindObjectOfType<BackpackUIManager>();
    }

    public override void OnUsed(Item_SO item)
    {
        UIManager.OnUsed(item);
    }

    public override void UpdateItemInfo(string itemDes)
    {
        UIManager.UpdateItemInfo(itemDes);
    }
}
