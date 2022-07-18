using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOnWorld : MonoBehaviour
{
    public Item thisItem;
    public Inventory playerInventory;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AddItem();
            Destroy(gameObject);
        }
    }

    public void AddItem()
    {
        if (!playerInventory.itemList.Contains(thisItem))
        {
            playerInventory.itemList.Add(thisItem);
            thisItem.itemHeld++;
            //InventoryManager.CreateNewItem(thisItem);
        }
        else
            thisItem.itemHeld++;
        InventoryManager.RefreshItem();
    }
}
