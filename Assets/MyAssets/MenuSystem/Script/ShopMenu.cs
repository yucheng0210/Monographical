using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopMenu : Menu
{
    [SerializeField]
    private DialogSystem shopDialog;

    [SerializeField]
    private ShopUIManager shopUIManager;

    protected override void Update()
    {
        base.Update();
        if (shopDialog.OpenMenu && !OpenBool)
        {
            Open();
            shopDialog.OpenMenu = false;
            shopUIManager.RefreshItem(shopUIManager.myBag);
            shopUIManager.UpdateItemInfo("");
        }
    }
}
