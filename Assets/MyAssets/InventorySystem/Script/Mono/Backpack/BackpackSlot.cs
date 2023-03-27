using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackpackSlot : MonoBehaviour
{
    private Item_SO slotItem;

    [SerializeField]
    private Image slotImage;

    [SerializeField]
    private Text slotCount;

    private Button useButton;
    public Image SlotImage
    {
        get { return slotImage; }
        set { slotImage = value; }
    }
    public Text SlotCount
    {
        get { return slotCount; }
        set { slotCount = value; }
    }
    public Item_SO SlotItem
    {
        get { return slotItem; }
        set { slotItem = value; }
    }

    public void OnClicked()
    {
        EventManager.Instance.DispatchEvent(EventDefinition.eventOnClickedToBag, slotItem.ItemInfo);
    }

    public void OnUsed(Item_SO item)
    {
        //UIManager.OnUsed(item);
    }
}
