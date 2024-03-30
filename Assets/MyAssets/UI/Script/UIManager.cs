using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        //EventManager.Instance.AddEventRegister(EventDefinition.eventMainLine, HandleMainLine);
        // GameManager.Instance.AddObservers(this);
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
        for (int i = 0; i < UIDict.Count; i++)
        {
            string key = UIDict.ElementAt(i).Key;
            HideUI(key);
        }
    }
    #region CreateNewItem
    private void CreateNewItem(Item item, BackpackSlot slotPrefab, Transform slotGroupTrans)
    {
        BackpackSlot newItem = Instantiate(slotPrefab, slotGroupTrans.position, Quaternion.identity);
        newItem.transform.SetParent(slotGroupTrans, false);
        newItem.SlotItem = item;
        newItem.SlotImage.sprite = item.ItemImage;
        newItem.SlotCount.text = item.ItemHeld.ToString();
    }
    private void CreateNewItem(Weapon weapon, WeaponSlot slotPrefab, Transform slotGroupTrans)
    {
        WeaponSlot newItem = Instantiate(slotPrefab, slotGroupTrans.position, Quaternion.identity);
        newItem.transform.SetParent(slotGroupTrans, false);
        newItem.SlotWeapon = weapon;
        newItem.SlotImage.sprite = Resources.Load<Sprite>(weapon.WeaponImagePath);
        newItem.SlotCount.text = weapon.WeaponHeld.ToString();
    }
    private void CreateNewItem(Quest quest, QuestSlot slotPrefab, Transform slotGroupTrans)
    {
        QuestSlot newItem = Instantiate(slotPrefab, slotGroupTrans.position, Quaternion.identity);
        newItem.transform.SetParent(slotGroupTrans, false);
        newItem.MyQuest = quest;
        newItem.SlotName.text = quest.TheName;
    }
    #endregion
    #region RefreshItem
    public void RefreshItem(BackpackSlot slotPrefab, Transform slotGroupTrans, Dictionary<int, Item> inventory)
    {
        for (int i = 0; i < slotGroupTrans.childCount; i++)
            Destroy(slotGroupTrans.GetChild(i).gameObject);
        foreach (KeyValuePair<int, Item> i in inventory)
        {
            if (i.Value.ItemHeld == 0)
            {
                inventory.Remove(i.Key);
                RefreshItem(slotPrefab, slotGroupTrans, inventory);
                break;
            }
            else
                CreateNewItem(i.Value, slotPrefab, slotGroupTrans);
        }
    }

    public void RefreshItem(QuestSlot slotPrefab, Transform slotGroupTrans, Dictionary<int, Quest> inventory)
    {
        for (int i = 0; i < slotGroupTrans.childCount; i++)
            Destroy(slotGroupTrans.GetChild(i).gameObject);
        foreach (KeyValuePair<int, Quest> i in inventory)
        {
            if (i.Value.Status == Quest.QuestState.Rewarded)
            {
                QuestManager.Instance.UpdateActiveQuests();
                continue;
            }
            else if (QuestManager.Instance.ActiveQuestList.Contains(i.Value))
                CreateNewItem(i.Value, slotPrefab, slotGroupTrans);
        }
    }
    public void RefreshItem(WeaponSlot slotPrefab, Transform slotGroupTrans, Dictionary<int, Weapon> inventory)
    {
        for (int i = 0; i < slotGroupTrans.childCount; i++)
            Destroy(slotGroupTrans.GetChild(i).gameObject);
        for (int i = 0; i < inventory.Count; i++)
        {
            int id = inventory.ElementAt(i).Key;
            if (inventory[id].WeaponHeld == 0)
            {
                inventory.Remove(id);
                RefreshItem(slotPrefab, slotGroupTrans, inventory);
                break;
            }
            else
                CreateNewItem(inventory[id], slotPrefab, slotGroupTrans);
        }
    }
    #endregion
    public IEnumerator FadeOut(CanvasGroup canvasGroup, float fadeTime)
    {
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime / fadeTime;
            yield return null;
        }
    }

    public IEnumerator FadeIn(CanvasGroup canvasGroup, float fadeTime)
    {
        while (canvasGroup.alpha != 0)
        {
            canvasGroup.alpha -= Time.unscaledDeltaTime / fadeTime;
            yield return null;
        }
    }

    public IEnumerator FadeOutIn(CanvasGroup canvasGroup,
        float firstWaitTime,
        float secondWaitTime,
        bool destroyFadeMenuBool,
        float fadeTime
    )
    {
        yield return new WaitForSecondsRealtime(firstWaitTime);
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime / fadeTime;
            yield return null;
        }
        yield return new WaitForSecondsRealtime(secondWaitTime);
        while (canvasGroup.alpha != 0)
        {
            canvasGroup.alpha -= Time.deltaTime / fadeTime;
            yield return null;
        }
        if (destroyFadeMenuBool)
            Destroy(canvasGroup.gameObject);
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
