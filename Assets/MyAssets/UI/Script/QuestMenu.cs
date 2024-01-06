using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestMenu : UIBase
{
    [SerializeField]
    private Text questInfo;

    [SerializeField]
    private QuestSlot slotPrefab;

    [SerializeField]
    private Transform slotGroupTrans;

    protected override void Start()
    {
        base.Start();
        EventManager.Instance.AddEventRegister(
            EventDefinition.eventOnClickedToQuest,
            EventOnClicked
        );
    }

    public override void Show()
    {
        base.Show();
        UIManager.Instance.RefreshItem(slotPrefab, slotGroupTrans, DataManager.Instance.QuestList);
        UpdateItemInfo("");
    }

    public void UpdateItemInfo(string questDes)
    {
        questInfo.text = questDes;
    }

    public void EventOnClicked(params object[] args)
    {
        UpdateItemInfo(((Quest)args[0]).Des);
    }
}
