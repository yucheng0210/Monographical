using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid : MonoBehaviour
{
    [SerializeField]
    private Item_SO gridItem;

    [SerializeField]
    private Image gridImage;

    [SerializeField]
    private Text gridAmount;

    [SerializeField]
    private Button useButton;
    public static int abilityCount;
    public Image GridImage
    {
        get { return gridImage; }
        set { gridImage = value; }
    }
    public Text GridAmount
    {
        get { return gridAmount; }
        set { gridAmount = value; }
    }
    public Item_SO GridItem
    {
        get { return gridItem; }
        set { gridItem = value; }
    }

    public void OnClicked()
    {
        InventoryUIManager.Instance.UpdateItemInfo(gridItem.ItemInfo);
        useButton = GameObject.Find("Use").GetComponent<Button>();
        useButton.onClick.RemoveAllListeners();
        useButton.onClick.AddListener(OnUsed);
    }

    public void OnUsed()
    {
        switch (gridItem.itemAbility)
        {
            case Item_SO.ItemAbility.Tonic:
                abilityCount = 1;
                break;
            case Item_SO.ItemAbility.AttackUp:
                abilityCount = 2;
                break;
        }
        if (abilityCount == gridItem.ItemAbilityNum)
        {
            InventoryUIManager.Instance.UpdateItemInfo("");
            gridItem.ItemHeld--;
            InventoryUIManager.Instance.RefreshItem();
        }
    }
}
