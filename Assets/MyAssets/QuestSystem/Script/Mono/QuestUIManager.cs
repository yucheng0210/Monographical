using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUIManager : MonoBehaviour
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
    private BackpackManager backpackManager;

    private void Awake()
    {
        questManager = GetComponent<QuestManager>();
        backpackManager = GetComponent<BackpackManager>();
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

    private void CreateNewObjective(Item_SO objectiveItem, Item_SO rewardItem, int id)
    {
        Debug.Log(rewardItem.ItemInOther != null);
        QuestObjectiveGrid newObjective = Instantiate(
            objectivePrefab,
            objectiveManager.transform.position,
            Quaternion.identity
        );
        newObjective.gameObject.transform.SetParent(objectiveManager.transform, false);
        for (int i = 0; i < questManager.backpack.ItemList.Count; i++)
        {
            newObjective.ObjectiveImage.sprite = objectiveItem.ItemImage;
            if (objectiveItem.ItemInOther == questManager.backpack.ItemList[i])
            {
                newObjective.ObjectiveText.text =
                    questManager.backpack.ItemList[i].ItemHeld.ToString()
                    + "/"
                    + objectiveItem.ItemHeld.ToString();
                if (questManager.GetQuestState(objectiveItem, questManager.backpack.ItemList[i]))
                {
                    questManager.questList.QuestList[id].Status = 2;
                    questManager.backpack.ItemList[i].ItemHeld -= objectiveItem.ItemHeld;
                    backpackManager.AddMoney(rewardItem.ItemCost);
                    if (rewardItem.ItemInOther != null)
                        backpackManager.AddItem(rewardItem.ItemInOther);
                }
            }
            else
                newObjective.ObjectiveText.text = "0" + "/" + objectiveItem.ItemHeld.ToString();
        }
    }
    public void RefreshItem()
    {
        DestroyObjective();
        for (int i = 0; i < gridManager.transform.childCount; i++)
            Destroy(gridManager.transform.GetChild(i).gameObject);
        for (int i = 0; i < questManager.questList.QuestList.Count; i++)
        {
            if (questManager.questList.QuestList[i].Status == 1)
                CreateNewItem(questManager.questList.QuestList[i]);
        }
    }

    public void RefreshObjective(int id)
    {
        DestroyObjective();
        for (int i = 0; i < questManager.objectiveInventory.ItemList.Count; i++)
        {
            for (int j = 0; j < questManager.rewardInventory.ItemList.Count; j++)
            {
                if (
                    questManager.objectiveInventory.ItemList[i].ItemIndex == id
                    && questManager.rewardInventory.ItemList[j].ItemIndex == id
                )
                    CreateNewObjective(
                        questManager.objectiveInventory.ItemList[i],
                        questManager.rewardInventory.ItemList[j],
                        id
                    );
            }
        }
    }

    public void DestroyObjective()
    {
        for (int i = 0; i < objectiveManager.transform.childCount; i++)
            Destroy(objectiveManager.transform.GetChild(i).gameObject);
    }
}
