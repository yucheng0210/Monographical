using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopGrid : Grid
{
    private ShopUIManager UIManager;

    public override void GetUIManager()
    {
        UIManager = FindObjectOfType<ShopUIManager>();
    }

    public override void OnUsed(Item item)
    {
        UIManager.OnUsed(item);
    }

    public override void UpdateItemInfo(string itemDes)
    {
        UIManager.UpdateItemInfo(itemDes);
    }
}
