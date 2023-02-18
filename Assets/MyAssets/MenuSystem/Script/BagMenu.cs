using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagMenu : UIBase
{
    [SerializeField]
    private BackpackUIManager backpackUIManager;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && OpenBool)
            openMenu.SetActive(false);
    }

    protected override void Open()
    {
        base.Open();
        backpackUIManager.RefreshItem(backpackUIManager.myBag);
        backpackUIManager.UpdateItemInfo("");
    }
}
