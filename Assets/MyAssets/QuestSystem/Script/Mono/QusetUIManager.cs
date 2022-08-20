using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QusetUIManager : MonoBehaviour
{
    [SerializeField]
    private Text questInfo;

    [SerializeField]
    private Text questRewards;

    [SerializeField]
    private QuestGrid gridPrefab;

    [SerializeField]
    private GameObject gridManager;

    [SerializeField]
    private GameObject objectiveManager;

    [SerializeField]
    private QuestObjectiveGrid objectivePrefab;
    private QuestManager questManager;

    public QuestList_SO questList;

    private void Awake()
    {
        questManager = GetComponent<QuestManager>();
        Initialize();
    }

    public void Initialize()
    {
        DestroyObjective();
        RefreshItem();
        questInfo.text = "";
        questRewards.text = "";
    }

    public void UpdateQuestText(string questDes, string reward)
    {
        questInfo.text = questDes;
        questRewards.text = reward;
        //questObjective.text = objective;
    }

    private void CreateNewItem(Quest_SO quest)
    {
        QuestGrid newQuest = Instantiate(
            gridPrefab,
            gridManager.transform.position,
            Quaternion.identity
        );
        newQuest.gameObject.transform.SetParent(gridManager.transform, false);
        newQuest.GridItem = quest;
        newQuest.GridName.text = quest.TheName;
    }

    private void CreateNewObjective(Item_SO objectiveItem, int id)
    {
        QuestObjectiveGrid newObjective = Instantiate(
            objectivePrefab,
            objectiveManager.transform.position,
            Quaternion.identity
        );
        newObjective.gameObject.transform.SetParent(objectiveManager.transform, false);
        foreach (Item_SO backItem in questManager.backpack.ItemList)
        {
            if (objectiveItem.ItemInOther == backItem)
            {
                newObjective.ObjectiveText.text =
                    backItem.ItemHeld.ToString() + "/" + objectiveItem.ItemHeld.ToString();
                newObjective.ObjectiveImage.sprite = objectiveItem.ItemImage;
                if (questManager.GetQuestState(objectiveItem, backItem))
                {
                    questList.QuestList[id].Status = 2;
                    backItem.ItemHeld -= objectiveItem.ItemHeld;
                }
            }
        }
    }

    public void RefreshItem()
    {
        for (int i = 0; i < gridManager.transform.childCount; i++)
            Destroy(gridManager.transform.GetChild(i).gameObject);
        for (int i = 0; i < questList.QuestList.Count; i++)
        {
            if (questList.QuestList[i].Status == 1)
                CreateNewItem(questList.QuestList[i]);
        }
    }

    public void RefreshObjective(int id)
    {
        DestroyObjective();
        for (int i = 0; i < questManager.objectiveInventory.ItemList.Count; i++)
        {
            if (questManager.objectiveInventory.ItemList[i].ItemIndex == id)
                CreateNewObjective(questManager.objectiveInventory.ItemList[i], id);
        }
    }

    public void DestroyObjective()
    {
        for (int i = 0; i < objectiveManager.transform.childCount; i++)
            Destroy(objectiveManager.transform.GetChild(i).gameObject);
    }
}
