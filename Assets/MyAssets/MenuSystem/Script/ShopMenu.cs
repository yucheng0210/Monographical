using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopMenu : Menu
{
    [SerializeField]
    private DialogSystem shopDialog;

    private void Update()
    {
        if (shopDialog.OpenShop && !OpenBool)
        {
            Open();
            shopDialog.OpenShop = false;
        }
    }
}
