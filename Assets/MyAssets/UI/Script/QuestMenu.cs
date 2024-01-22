using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class QuestMenu : UIBase
{
    [SerializeField]
    private Text questName;
    [SerializeField]
    private Text questNPC;
    [SerializeField]
    private Text questTarget;
    [SerializeField]
    private Text questInfo;
    [SerializeField]
    private Transform questRewardTrans;
    [SerializeField]
    private GameObject questRewardPrefab;
    [SerializeField]
    private QuestSlot slotPrefab;

    [SerializeField]
    private Transform slotGroupTrans;
    [SerializeField]
    private Button receiveRewardButton;

    protected override void Start()
    {
        base.Start();
        EventManager.Instance.AddEventRegister(EventDefinition.eventOnClickedToQuest, EventOnClicked);
    }

    public override void Show()
    {
        base.Show();
        UIManager.Instance.RefreshItem(slotPrefab, slotGroupTrans, DataManager.Instance.QuestList);
        ClearAllItemInfo();
    }

    public void ClearAllItemInfo()
    {
        questName.text = "";
        questNPC.text = "";
        questTarget.text = "";
        questInfo.text = "";
        for (int i = 0; i < questRewardTrans.childCount; i++)
        {
            Destroy(questRewardTrans.GetChild(i).gameObject);
        }
    }

    public void EventOnClicked(params object[] args)
    {
        ClearAllItemInfo();
        Quest quest = (Quest)args[0];
        string questTargetContent = "";
        QuestManager.Instance.CheckQuestProgress(quest.ID);
        receiveRewardButton.onClick.RemoveAllListeners();
        if (quest.Status == Quest.QuestState.Completed)
        {
            receiveRewardButton.onClick.AddListener(() => QuestManager.Instance.GetRewards(quest.ID));
            receiveRewardButton.onClick.AddListener(() => receiveRewardButton.onClick.RemoveAllListeners());
            quest.Status = Quest.QuestState.Rewarded;
        }
        for (int i = 0; i < quest.TargetList.Count; i++)
        {
            Item item = DataManager.Instance.ItemList[quest.TargetList[i].Item1];
            string backpackHeld = DataManager.Instance.GetItem(quest.TargetList[i].Item1).ItemHeld.ToString() != null
            ? DataManager.Instance.GetItem(quest.TargetList[i].Item1).ItemHeld.ToString() : "0";
            questTargetContent += item.ItemName + "：" + backpackHeld + "/" + quest.TargetList[i].Item2 + "\r\n";
        }
        for (int i = 0; i < quest.TargetEnemyList.Count; i++)
        {
            Character enemy = DataManager.Instance.CharacterList[quest.TargetEnemyList[i].Item1];
            string currentKill = QuestManager.Instance.QuestCurrentKill.ContainsKey(enemy.CharacterID)
            ? QuestManager.Instance.QuestCurrentKill[enemy.CharacterID].ToString() : "0";
            questTargetContent += enemy.CharacterName + "：" + currentKill + "/" + quest.TargetEnemyList[i].Item2 + "\r\n";
        }
        questName.text = quest.TheName;
        questNPC.text = quest.NPC;
        questTarget.text = questTargetContent;
        questInfo.text = quest.Des;
        for (int i = 0; i < quest.RewardList.Count; i++)
        {
            GameObject reward = Instantiate(questRewardPrefab, questRewardTrans);
            reward.GetComponent<Image>().sprite = DataManager.Instance.ItemList[quest.RewardList[i].Item1].ItemImage;
        }
    }
}
