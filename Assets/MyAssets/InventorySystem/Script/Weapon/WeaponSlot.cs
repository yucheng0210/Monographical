using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WeaponSlot : MonoBehaviour
{
    private Weapon slotWeapon;

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
    public Weapon SlotWeapon
    {
        get { return slotWeapon; }
        set { slotWeapon = value; }
    }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClicked);
    }

    public void OnClicked()
    {
        EventManager.Instance.DispatchEvent(EventDefinition.eventOnClickedToWeapon, slotWeapon);
    }
}
