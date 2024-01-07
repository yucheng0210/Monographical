using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShortcutBarMenu : UIBase
{
    [SerializeField]
    private Image itemImage;

    [SerializeField]
    private Text itemCountText;

    [SerializeField]
    private BackpackSlot slotPrefab;

    [SerializeField]
    private Transform slotGroupTrans;
    private int currentIndex;
    private Button shortcutBarButton;

    protected override void Start()
    {
        base.Start();
        currentIndex = 0;
        EventManager.Instance.AddEventRegister(EventDefinition.eventOnClickedToBag, EventOnClicked);
        EventManager.Instance.AddEventRegister(
            EventDefinition.eventRemoveItemToBag,
            EventRemoveItem
        );
        shortcutBarButton = openMenu.GetComponentInChildren<Button>();
    }

    protected override void Update()
    {
        base.Update();
        if (DataManager.Instance.ShortcutBar.Count == 0)
            return;
        SwitchItem();
        UseItem();
    }

    private void SwitchItem()
    {
        bool isSwitch = false;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            isSwitch = true;
            currentIndex--;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            isSwitch = true;
            currentIndex++;
        }
        CheckIndex();
        if (isSwitch)
        {
            isSwitch = false;
            UpdateItemInfo();
        }
    }

    private void CheckIndex()
    {
        if (currentIndex > DataManager.Instance.ShortcutBar.Count - 1)
            currentIndex = 0;
        if (currentIndex < 0)
            currentIndex = DataManager.Instance.ShortcutBar.Count - 1;
    }

    private void UpdateItemInfo()
    {
        if (DataManager.Instance.ShortcutBar.Count == 0)
        {
            itemImage.sprite = null;
            itemCountText.text = "";
            return;
        }
        itemImage.sprite = DataManager.Instance.ShortcutBar.ElementAt(currentIndex).Value.ItemImage;
        itemCountText.text = DataManager.Instance.ShortcutBar
            .ElementAt(currentIndex)
            .Value.ItemHeld.ToString();
    }

    private void UseItem()
    {
        if (Input.GetKeyDown(KeyCode.R))
            BackpackManager.Instance.UseItem(
                DataManager.Instance.ShortcutBar.ElementAt(currentIndex).Value.ItemIndex
            );
    }

    public void EventOnClicked(params object[] args)
    {
        shortcutBarButton.onClick.RemoveAllListeners();
        shortcutBarButton.onClick.AddListener(() =>
        {
            BackpackManager.Instance.SetShortcutBar((Item)args[0]);
            UpdateItemInfo();
            UIManager.Instance.RefreshItem(
                slotPrefab,
                slotGroupTrans,
                DataManager.Instance.ShortcutBar
            );
        });
    }

    public void EventRemoveItem(params object[] args)
    {
        UIManager.Instance.RefreshItem(
            slotPrefab,
            slotGroupTrans,
            DataManager.Instance.ShortcutBar
        );
        CheckIndex();
        UpdateItemInfo();
    }
}
