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
    private QuestManager questManager;

    private void Awake()
    {
        questManager = GetComponent<QuestManager>();
        RefreshItem();
        questInfo.text = "";
    }

    public void UpdateQuestDes(string questDes)
    {
        questInfo.text = questDes;
    }

    public void UpdateQuestRewards(string reward)
    {
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

    public void RefreshItem()
    {
        for (int i = 0; i < gridManager.transform.childCount; i++)
            Destroy(gridManager.transform.GetChild(i).gameObject);
        for (int i = 0; i < questManager.QuestList.Count; i++)
        {
            if (questManager.QuestList[i].Status != 1)
            {
                questManager.QuestList.Remove(questManager.QuestList[i]);
                RefreshItem();
            }
            else
                CreateNewItem(questManager.QuestList[i]);
        }
    }
}
