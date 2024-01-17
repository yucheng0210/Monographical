using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMenu : UIBase
{
    [SerializeField]
    private Transform slotGroupTrans;

    [SerializeField]
    private WeaponSlot slotPrefab;
    protected override void Start()
    {
        base.Start();
        EventManager.Instance.AddEventRegister(EventDefinition.eventOnClickedToWeapon, EventOnClickedToWeapon);
    }
    public override void Show()
    {
        base.Show();
        UpdateItemInfo();
        UIManager.Instance.RefreshItem(slotPrefab, slotGroupTrans, DataManager.Instance.WeaponBag);
    }
    private void UpdateItemInfo()
    {

    }
    private void EventOnClickedToWeapon(params object[] args)
    {
        UpdateItemInfo();
    }
}
