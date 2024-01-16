using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackpackSlot : MonoBehaviour
{

    [SerializeField]
    private Image slotImage;

    [SerializeField]
    private Text slotCount;

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
    public Item SlotItem { get; set; }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClicked);
    }

    public void OnClicked()
    {
        EventManager.Instance.DispatchEvent(EventDefinition.eventOnClickedToBag, SlotItem);
    }
}
