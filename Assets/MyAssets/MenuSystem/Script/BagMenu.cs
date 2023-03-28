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
        BackpackManager.Instance.AddMoney(0);
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

    private void CreateNewItem(Item item)
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
        for (int i = 0; i < BackpackManager.Instance.Backpack.Count; i++)
        {
            if (BackpackManager.Instance.Backpack[i].ItemHeld == 0)
            {
                BackpackManager.Instance.Backpack.Remove(BackpackManager.Instance.Backpack[i]);
                RefreshItem();
                break;
            }
            else
                CreateNewItem(BackpackManager.Instance.Backpack[i]);
        }
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
        moneyText.text = args[0].ToString();
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
