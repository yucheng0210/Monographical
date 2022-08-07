using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : InventoryManager
{
    private ShopUIManager UIManager;

    public override void GetUIManager()
    {
        UIManager = GetComponent<ShopUIManager>();
    }

    public override void RefreshItem()
    {
        UIManager.RefreshItem();
    }

    public override void UpdateItemInfo(string itemDes)
    {
        UIManager.UpdateItemInfo(itemDes);
    }
}
