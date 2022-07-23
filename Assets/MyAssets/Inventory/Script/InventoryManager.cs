using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager :Singleton<InventoryManager>
{
    public Inventory myBag;
    public GameObject gridManager;
    public Grid gridPrefab;
    public Text itemInfo;
    private void OnEnable()
    {
        RefreshItem();
        itemInfo.text = "";
    }

    public void UpdateItemInfo(string itemDes)
    {
        Instance.itemInfo.text = itemDes;
    }

    public void CreateNewItem(Item item)
    {
        Grid newItem = Instantiate(
            Instance.gridPrefab,
            Instance.gridManager.transform.position,
            Quaternion.identity
        );
        newItem.gameObject.transform.SetParent(Instance.gridManager.transform);
        newItem.gridItem = item;
        newItem.gridImage.sprite = item.itemImage;
        newItem.gridAmount.text = item.itemHeld.ToString();
    }

    public void RefreshItem()
    {
        for (int i = 0; i < Instance.gridManager.transform.childCount; i++)
        {
            if (Instance.gridManager.transform.childCount == 0)
                break;
            Destroy(Instance.gridManager.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < Instance.myBag.itemList.Count; i++)
        {
            if (Instance.myBag.itemList[i].itemHeld == 0)
            {
                Instance.myBag.itemList.Remove(Instance.myBag.itemList[i]);
                RefreshItem();
            }
            else
                CreateNewItem(Instance.myBag.itemList[i]);
        }
    }
}
