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
    private Text itemNameText;

    [SerializeField]
    private Text itemInfoText;

    [SerializeField]
    private Button useButton;

    [SerializeField]
    private Transform pickUpGroupTrans;
    [SerializeField]
    private PickUpClue pickUpCluePrefab;

    protected override void Start()
    {
        base.Start();
        EventManager.Instance.AddEventRegister(EventDefinition.eventAddItemToBag, EventAddItem);
        EventManager.Instance.AddEventRegister(EventDefinition.eventRemoveItemToBag, EventRemoveItem);
        EventManager.Instance.AddEventRegister(EventDefinition.eventReviseMoneyToBag, EventReviseMoney);
        EventManager.Instance.AddEventRegister(EventDefinition.eventOnClickedToBag, EventOnClicked);
    }

    public override void Show()
    {
        base.Show();
        UIManager.Instance.RefreshItem(slotPrefab, slotGroupTrans, DataManager.Instance.Backpack);
        UpdateItemInfo("", "");
    }

    public void UpdateItemInfo(string itemDes, string itemName)
    {
        itemInfoText.text = itemDes;
        itemNameText.text = itemName;
    }

    public void EventAddItem(params object[] args)
    {
        UIManager.Instance.RefreshItem(slotPrefab, slotGroupTrans, DataManager.Instance.Backpack);
        PickUpClue pickUp = Instantiate(pickUpCluePrefab, pickUpGroupTrans);
        pickUp.ItemImage.sprite = DataManager.Instance.ItemList[(int)args[0]].ItemImage;
        pickUp.ItemNameText.text = DataManager.Instance.ItemList[(int)args[0]].ItemName;
        pickUp.ItemCountText.text = "X" + DataManager.Instance.ItemList[(int)args[0]].ItemHeld.ToString();
        StartCoroutine(UIManager.Instance.FadeOutIn(pickUp.GetComponent<CanvasGroup>(), 0, 2, false, 0.5f));
    }

    public void EventRemoveItem(params object[] args)
    {
        UpdateItemInfo("", "");
        UIManager.Instance.RefreshItem(slotPrefab, slotGroupTrans, DataManager.Instance.Backpack);
    }

    public void EventReviseMoney(params object[] args)
    {
        moneyText.text = args[0].ToString();
    }

    public void EventOnClicked(params object[] args)
    {
        UpdateItemInfo(((Item)args[0]).ItemInfo.ToString(), ((Item)args[0]).ItemName.ToString());
        useButton.onClick.RemoveAllListeners();
        useButton.onClick.AddListener(() =>
        {
            BackpackManager.Instance.UseItem(((Item)args[0]).ItemIndex);
        });
    }
}
