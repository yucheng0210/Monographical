using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class InventoryUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject gridManager;

    [SerializeField]
    private Grid gridPrefab;

    [SerializeField]
    private Text itemInfo;

    [SerializeField]
    private Inventory_SO myBag;

    [SerializeField]
    private Inventory_SO backpack;

    [SerializeField]
    private Text moneyText;

    public void Awake()
    {
        GetManager();
        RefreshItem();
        itemInfo.text = "";
    }

    public abstract void GetManager();

    public void UpdateItemInfo(string itemDes)
    {
        itemInfo.text = itemDes;
    }

    private void CreateNewItem(Item_SO item)
    {
        Grid newItem = Instantiate(gridPrefab, gridManager.transform.position, Quaternion.identity);
        newItem.gameObject.transform.SetParent(gridManager.transform, false);
        newItem.GridItem = item;
        newItem.GridImage.sprite = item.ItemImage;
        newItem.GridAmount.text = item.ItemHeld.ToString();
    }

    public void RefreshItem()
    {
        for (int i = 0; i < gridManager.transform.childCount; i++)
        {
            if (gridManager.transform.childCount == 0)
                break;
            Destroy(gridManager.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < myBag.ItemList.Count; i++)
        {
            if (myBag.ItemList[i].ItemHeld == 0)
            {
                myBag.ItemList.Remove(myBag.ItemList[i]);
                RefreshItem();
            }
            else
                CreateNewItem(myBag.ItemList[i]);
        }
        moneyText.text = backpack.MoneyCount.ToString();
    }

    public abstract void OnUsed(Item_SO item);
}
