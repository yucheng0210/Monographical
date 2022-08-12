using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestGrid : MonoBehaviour
{
    private Quest_SO gridItem;

    [SerializeField]
    private Text gridName;

    private Button useButton;
    private QusetUIManager qusetUIManager;
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

    private void OnClicked()
    {
        qusetUIManager.UpdateQuestDes(gridItem.Des);
        qusetUIManager.UpdateQuestRewards(gridItem.Rewards);
    }
}
