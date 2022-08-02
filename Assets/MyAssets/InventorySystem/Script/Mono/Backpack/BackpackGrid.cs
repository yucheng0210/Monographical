using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackpackGrid : MonoBehaviour
{
    [SerializeField]
    private Item_SO gridItem;

    [SerializeField]
    private Image gridImage;

    [SerializeField]
    private Text gridAmount;

    [SerializeField]
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

    public void OnClicked()
    {
        BackpackUIManager.Instance.UpdateItemInfo(gridItem.ItemInfo);
        useButton = GameObject.Find("Use").GetComponent<Button>();
        useButton.onClick.RemoveAllListeners();
        useButton.onClick.AddListener(
            () =>
            {
                BackpackUIManager.Instance.OnUsed(gridItem);
            }
        );
    }
}
