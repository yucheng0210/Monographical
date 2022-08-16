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

    [SerializeField]
    private QuestList_SO questList;

    private void Awake()
    {
        questManager = GetComponent<QuestManager>();
        RefreshItem();
        questInfo.text = "";
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

    private void CreateNewObjective(Item_SO item)
    {
        QuestObjectiveGrid newObjective = Instantiate(
            objectivePrefab,
            objectiveManager.transform.position,
            Quaternion.identity
        );
        newObjective.gameObject.transform.SetParent(objectiveManager.transform, false);
        foreach (Item_SO i in questManager.backpack.ItemList)
        {
            if (i.ItemInOther == item)
            {
                newObjective.ObjectiveText.text =
                    i.ItemHeld.ToString() + "/" + item.ItemHeld.ToString();
                newObjective.ObjectiveImage.sprite = item.ItemImage;
            }
        }
    }

    public void RefreshItem()
    {
        for (int i = 0; i < gridManager.transform.childCount; i++)
            Destroy(gridManager.transform.GetChild(i).gameObject);
        for (int i = 0; i < questList.QuestList.Count; i++)
        {
            if (questList.QuestList[i].Status != 1)
            {
                questList.QuestList.Remove(questList.QuestList[i]);
                RefreshItem();
            }
            else
                CreateNewItem(questList.QuestList[i]);
        }
    }

    public void RefreshObjective(int id)
    {
        for (int i = 0; i < objectiveManager.transform.childCount; i++)
            Destroy(objectiveManager.transform.GetChild(i).gameObject);
        for (int i = 0; i < questManager.objectiveInventory.ItemList.Count; i++)
        {
            if (questManager.objectiveInventory.ItemList[i].ItemHeld == 0)
            {
                questManager.objectiveInventory.ItemList.Remove(
                    questManager.objectiveInventory.ItemList[i]
                );
                RefreshObjective(id);
            }
            else if (questManager.objectiveInventory.ItemList[i].ItemIndex == id)
                CreateNewObjective(questManager.objectiveInventory.ItemList[i]);
        }
    }
}
