using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagMenu : Menu
{
    [SerializeField]
    private BackpackUIManager backpackUIManager;

    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!OpenBool)
            {
                Open();
                backpackUIManager.RefreshItem(backpackUIManager.myBag);
                backpackUIManager.UpdateItemInfo("");
            }
            else
                Close();
        }
    }
}
