using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : InventoryUIManager
{
    private BackpackManager manager;
    private ShopManager shopManager;
    private bool switchBool;

    [SerializeField]
    private Text buttonText;

    [SerializeField]
    private Text useButtonText;

    public override void GetManager()
    {
        manager = GetComponent<BackpackManager>();
        shopManager = GetComponent<ShopManager>();
    }

    public override void OnUsed(Item_SO item)
    {
        if (!switchBool)
        {
            if (manager.GetMoney() >= item.ItemCost && item.ItemInShop)
            {
                manager.ReduceMoney(item.ItemCost);
                manager.AddItem(item.ItemInOther);
                item.ItemHeld--;
                RefreshItem(MyBag);
            }
        }
        else
        {
            manager.AddMoney(item.ItemCost);
            shopManager.AddItem(item.ItemInOther);
            item.ItemHeld--;
            RefreshItem(Backpack);
        }
    }

    public void SellSwitch()
    {
        if (switchBool)
        {
            RefreshItem(MyBag);
            buttonText.text = "Sell";
            useButtonText.text = "Buy";
            switchBool = false;
        }
        else
        {
            RefreshItem(Backpack);
            buttonText.text = "Buy";
            useButtonText.text = "Sell";
            switchBool = true;
        }
    }
}
