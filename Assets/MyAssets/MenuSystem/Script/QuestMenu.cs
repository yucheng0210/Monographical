using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestMenu : UIBase
{
    [SerializeField]
    private QuestUIManager questUIManager;

    [SerializeField]
    private Text questInfo;

    [SerializeField]
    private Text questRewards;

    [SerializeField]
    private QuestGrid slotPrefab;

    [SerializeField]
    private Transform slotGroupTrans;

    [SerializeField]
    private Transform targetGroupTrans;

    [SerializeField]
    private QuestObjectiveGrid targetSlotPrefab;

    public void QuestInitialize()
    {
        questUIManager.Initialize();
    }
}
