using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopMenu : Menu
{
    [SerializeField]
    private DialogSystem shopDialog;

    [SerializeField]
    private ShopUIManager shopUIManager;

    private void Update()
    {
        if (shopDialog.OpenMenu && !OpenBool)
        {
            Open();
            shopDialog.OpenMenu = false;
            shopUIManager.RefreshItem(shopUIManager.MyBag);
            shopUIManager.UpdateItemInfo("");
        }
    }
}
