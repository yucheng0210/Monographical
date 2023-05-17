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

    private void CreateNewItem(Quest quest, QuestSlot slotPrefab, Transform slotGroupTrans)
    {
        QuestSlot newItem = Instantiate(slotPrefab, slotGroupTrans.position, Quaternion.identity);
        newItem.gameObject.transform.SetParent(slotGroupTrans, false);
        newItem.MyQuest = quest;
        newItem.SlotName.text = quest.TheName;
        newItem.NPCName.text = quest.NPC;
    }

    public void RefreshItem(
        BackpackSlot slotPrefab,
        Transform slotGroupTrans,
        Dictionary<int, Item> inventory
    )
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

    public void RefreshItem(
        QuestSlot slotPrefab,
        Transform slotGroupTrans,
        Dictionary<int, Quest> inventory
    )
    {
        for (int i = 0; i < slotGroupTrans.childCount; i++)
            Destroy(slotGroupTrans.GetChild(i).gameObject);
        foreach (KeyValuePair<int, Quest> i in inventory)
        {
            if (i.Value.Status == Quest.QuestState.Rewarded)
            {
                inventory.Remove(i.Key);
                RefreshItem(slotPrefab, slotGroupTrans, inventory);
                break;
            }
            else if (i.Value.Status == Quest.QuestState.Active)
                CreateNewItem(i.Value, slotPrefab, slotGroupTrans);
        }
    }

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
        Destroy(gameObject);
    }

    public IEnumerator FadeOutIn(
        CanvasGroup canvasGroup,
        float firstWaitTime,
        float secondWaitTime,
        bool nextMainLineBool,
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
        if (nextMainLineBool)
            EventManager.Instance.DispatchEvent(EventDefinition.eventNextMainLine, this);
        Destroy(gameObject);
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
