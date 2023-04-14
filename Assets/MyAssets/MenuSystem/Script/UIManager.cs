using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>, IObserver
{
    /*[SerializeField]
    private SceneFader deadImage;

    [SerializeField]
    private SceneFader wifeDeathImage;*/
    public Dictionary<string, UIBase> UIDict { get; set; }
    public bool MenuIsOpen { get; set; }

    protected override void Awake()
    {
        base.Awake();
        UIDict = new Dictionary<string, UIBase>();
        MenuIsOpen = false;
    }

    private void Start()
    {
        //EventManager.Instance.AddEventRegister(EventDefinition.eventMainLine, HandleMainLine);
        GameManager.Instance.AddObservers(this);
    }

    private void OnDisable()
    {
        GameManager.Instance.RemoveObservers(this);
    }

    public void ShowUI(string uiName)
    {
        UIDict[uiName].Show();
    }

    public void HideUI(string uiName)
    {
        UIDict[uiName].Hide();
    }

    public void HideAllUI()
    {
        foreach (var i in UIDict)
        {
            HideUI(i.Key);
        }
    }

    private void CreateNewItem(Item item, BackpackSlot slotPrefab, Transform slotGroupTrans)
    {
        BackpackSlot newItem = Instantiate(
            slotPrefab,
            slotGroupTrans.position,
            Quaternion.identity
        );
        newItem.gameObject.transform.SetParent(slotGroupTrans, false);
        newItem.SlotItem = item;
        newItem.SlotImage.sprite = item.ItemImage;
        newItem.SlotCount.text = item.ItemHeld.ToString();
    }

    public void RefreshItem(BackpackSlot slotPrefab, Transform slotGroupTrans, List<Item> inventory)
    {
        for (int i = 0; i < slotGroupTrans.childCount; i++)
            Destroy(slotGroupTrans.GetChild(i).gameObject);
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].ItemHeld == 0)
            {
                inventory.Remove(inventory[i]);
                RefreshItem(slotPrefab, slotGroupTrans, inventory);
                break;
            }
            else
                CreateNewItem(inventory[i], slotPrefab, slotGroupTrans);
        }
    }

    public void EndNotify()
    {
        //  deadImage.StartCoroutine(deadImage.FadeOutIn(2, 3, false));
    }

    public void SceneLoadingNotify(bool loadingBool)
    {
        //throw new System.NotImplementedException();
    }

    private void HandleMainLine(params object[] args)
    {
        /*if ((int)args[0] == 3)
            StartCoroutine(wifeDeathImage.FadeOutIn(0, 5, true));*/
    }
}
