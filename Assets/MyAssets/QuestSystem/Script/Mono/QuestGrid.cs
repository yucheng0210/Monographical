using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestGrid : MonoBehaviour
{
    private Quest_SO gridItem;
    private Text gridName;
    private Button useButton;
    private QuestUIManager qusetUIManager;
    public Text GridName
    {
        get { return gridName; }
        set { gridName = value; }
    }
    public Quest_SO GridItem
    {
        get { return gridItem; }
        set { gridItem = value; }
    }

    private void Awake()
    {
        qusetUIManager = FindObjectOfType<QuestUIManager>();
        gridName = GetComponentInChildren<Text>();
    }

    public void OnClicked()
    {
        qusetUIManager.UpdateQuestText(gridItem.Des, gridItem.Rewards);
        qusetUIManager.RefreshObjective(gridItem.ID);
    }
}
