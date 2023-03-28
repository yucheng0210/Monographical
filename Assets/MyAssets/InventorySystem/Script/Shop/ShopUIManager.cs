using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : InventoryUIManager
{
    private BackpackManager backpackManager;
    private ShopManager shopManager;
    private bool switchBool;

    [SerializeField]
    private Text buttonText;

    [SerializeField]
    private Text useButtonText;

    public override void GetManager()
    {
        backpackManager = GetComponent<BackpackManager>();
        shopManager = GetComponent<ShopManager>();
    }

    public override void OnUsed(Item item)
    {
        if (!switchBool)
        {
            if (backpackManager.GetMoney() >= item.ItemCost && item.ItemInShop)
            {
                backpackManager.ReduceMoney(item.ItemCost);
                backpackManager.AddItem(item.ItemInOther);
                item.ItemHeld--;
                RefreshItem(myBag);
            }
        }
        else
        {
            backpackManager.AddMoney(item.ItemCost);
            shopManager.AddItem(item.ItemInOther);
            item.ItemHeld--;
            RefreshItem(backpack);
        }
    }

    public void SellSwitch()
    {
        if (switchBool)
        {
            RefreshItem(myBag);
            buttonText.text = "Sell";
            useButtonText.text = "Buy";
            switchBool = false;
        }
        else
        {
            RefreshItem(backpack);
            buttonText.text = "Buy";
            useButtonText.text = "Sell";
            switchBool = true;
        }
    }
}
