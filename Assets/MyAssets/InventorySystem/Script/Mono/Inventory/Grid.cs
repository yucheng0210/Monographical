using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Grid : MonoBehaviour
{
    private Item_SO gridItem;

    [SerializeField]
    private Image gridImage;

    [SerializeField]
    private Text gridAmount;

    private Button useButton;
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
    public abstract void GetUIManager();

    public void Awake()
    {
        GetUIManager();
    }

    public void OnClicked()
    {
        UpdateItemInfo(gridItem.ItemInfo);
        useButton = GameObject.FindGameObjectWithTag("Use").GetComponent<Button>();
        useButton.onClick.RemoveAllListeners();
        useButton.onClick.AddListener(
            () =>
            {
                OnUsed(gridItem);
            }
        );
    }

    public abstract void OnUsed(Item_SO item);
    public abstract void UpdateItemInfo(string itemDes);
}
