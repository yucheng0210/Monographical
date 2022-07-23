using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid : MonoBehaviour
{
    public Item gridItem;
    public Image gridImage;
    public Text gridAmount;
    public Button useButton;
    public static int abilityCount;

    public void OnClicked()
    {
        InventoryManager.Instance.UpdateItemInfo(gridItem.itemInfo);
        useButton = GameObject.Find("Use").GetComponent<Button>();
        useButton.onClick.RemoveAllListeners();
        useButton.onClick.AddListener(OnUsed);
    }

    public void OnUsed()
    {
        switch (gridItem.ability)
        {
            case Item.itemAbility.Tonic:
                abilityCount = 1;
                break;
            case Item.itemAbility.AttackUp:
                abilityCount = 2;
                break;
        }
        if (abilityCount == gridItem.itemAbilityNum)
        {
            InventoryManager.Instance.UpdateItemInfo("");
            gridItem.itemHeld--;
            InventoryManager.Instance.RefreshItem();
        }
    }
}
