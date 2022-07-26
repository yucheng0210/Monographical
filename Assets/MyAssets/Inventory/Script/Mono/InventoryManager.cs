using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField]
    private Item_SO item_SO;

    [SerializeField]
    private Inventory_SO myBag;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AddItem();
            Destroy(gameObject);
        }
    }

    private void AddItem()
    {
        if (!myBag.ItemList.Contains(item_SO))
        {
            myBag.ItemList.Add(item_SO);
            item_SO.ItemHeld++;
            //InventoryManager.CreateNewItem(thisItem);
        }
        else
            item_SO.ItemHeld++;
        InventoryUIManager.Instance.RefreshItem();
    }
}
