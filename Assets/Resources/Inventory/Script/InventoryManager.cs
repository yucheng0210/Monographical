using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    static InventoryManager instance;
    public Inventory myBag;
    public GameObject gridManager;
    public Grid gridPrefab;
    public Text itemInfo;

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
    }

    private void OnEnable()
    {
        RefreshItem();
        itemInfo.text = "";
    }

    public static void UpdateItemInfo(string itemDes)
    {
        instance.itemInfo.text = itemDes;
    }

    public static void CreateNewItem(Item item)
    {
        Grid newItem = Instantiate(
            instance.gridPrefab,
            instance.gridManager.transform.position,
            Quaternion.identity
        );
        newItem.gameObject.transform.SetParent(instance.gridManager.transform);
        newItem.gridItem = item;
        newItem.gridImage.sprite = item.itemImage;
        newItem.gridAmount.text = item.itemHeld.ToString();
    }

    public static void RefreshItem()
    {
        for (int i = 0; i < instance.gridManager.transform.childCount; i++)
        {
            if (instance.gridManager.transform.childCount == 0)
                break;
            Destroy(instance.gridManager.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < instance.myBag.itemList.Count; i++)
        {
            if (instance.myBag.itemList[i].itemHeld == 0)
            {
                instance.myBag.itemList.Remove(instance.myBag.itemList[i]);
                RefreshItem();
            }
            else
                CreateNewItem(instance.myBag.itemList[i]);
        }
    }
}
