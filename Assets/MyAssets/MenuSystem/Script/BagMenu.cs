using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagMenu : UIBase
{
    [SerializeField]
    private Transform slotGroupTrans;

    [SerializeField]
    private BackpackSlot slotPrefab;

    [SerializeField]
    private Text moneyText;

    [SerializeField]
    private Text itemInfo;

    [SerializeField]
    private Button useButton;
    private Dictionary<int, Item_SO> myBag;

    protected override void Start()
    {
        base.Start();
        EventManager.Instance.AddEventRegister(EventDefinition.eventAddItemToBag, EventAddItem);
        EventManager.Instance.AddEventRegister(
            EventDefinition.eventRemoveItemToBag,
            EventRemoveItem
        );
        EventManager.Instance.AddEventRegister(
            EventDefinition.eventReviseMoneyToBag,
            EventReviseMoney
        );
        EventManager.Instance.AddEventRegister(EventDefinition.eventOnClickedToBag, EventOnClicked);
        myBag = BackpackManager.Instance.Backpack;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && OpenBool)
            openMenu.SetActive(false);
    }

    protected override void Open()
    {
        base.Open();
        RefreshItem();
        UpdateItemInfo("");
    }

    public void UpdateItemInfo(string itemDes)
    {
        itemInfo.text = itemDes;
    }

    private void CreateNewItem(Item_SO item)
    {
        BackpackSlot newItem = Instantiate(
            slotPrefab,
            slotGroupTrans.position,
            Quaternion.identity
        );
        newItem.gameObject.transform.SetParent(slotGroupTrans, false);
        newItem.SlotItem = item;
        newItem.SlotImage.sprite = item.ItemImage;
        newItem.SlotCount.text = item.ItemHeld.ToString();
    }

    public void RefreshItem()
    {
        for (int i = 0; i < slotGroupTrans.childCount; i++)
            Destroy(slotGroupTrans.GetChild(i).gameObject);
        for (int i = 0; i < myBag.Count; i++)
        {
            if (myBag[i].ItemHeld == 0)
            {
                myBag.Remove(myBag[i].ItemIndex);
                RefreshItem();
            }
            else
                CreateNewItem(myBag[i]);
        }
        moneyText.text = myBag.ToString();
    }

    public void EventAddItem(params object[] args)
    {
        RefreshItem();
    }

    public void EventRemoveItem(params object[] args)
    {
        UpdateItemInfo("");
        RefreshItem();
    }

    public void EventReviseMoney(params object[] args)
    {
        RefreshItem();
    }

    public void EventOnClicked(params object[] args)
    {
        UpdateItemInfo(args[0].ToString());
        useButton.onClick.RemoveAllListeners();
        useButton.onClick.AddListener(() =>
        {
            EventManager.Instance.DispatchEvent(EventDefinition.eventOnUsedToBag);
        });
    }
}
