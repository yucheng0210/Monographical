using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    [SerializeField]
    private Text textLabel;

    /*[SerializeField]
    private Image faceImage;*/

    [SerializeField]
    private Text characterName;

    [SerializeField]
    private Sprite playerFace,
        npcFace;

    [SerializeField]
    private TextAsset textFile;
    private int index;

    [SerializeField]
    private float maxTextWaitTime;

    [SerializeField]
    private float currentTextWaitTime;
    private bool textFinished;
    private bool openMenu;
    public static bool isTalking;

    [SerializeField]
    private GameObject choiceButton;

    [SerializeField]
    private GameObject choiceManager;

    [SerializeField]
    private DialogList_SO dialogList;
    private Dialog dialog;

    [SerializeField]
    private string currentBranchID = "DEFAULT";
    private bool continueBool;
    private bool inSelection;

    [SerializeField]
    private QuestManager questManager;

    [SerializeField]
    private BackpackManager backpackManager;

    [SerializeField]
    private bool isQuestDialog = false;

    [SerializeField]
    private int questID;
    public bool OpenMenu
    {
        get { return openMenu; }
        set { openMenu = value; }
    }
    public bool BlockContinue { get; set; }

    private void OnEnable()
    {
        textFinished = false;
        textLabel.text = "";
        currentTextWaitTime = maxTextWaitTime;
        continueBool = true;
        index = 0;
        isTalking = true;
        textFinished = true;
        SetCharacterInfo();
        Initialize();
    }

    private void OnDisable()
    {
        isTalking = false;
        DestroyChoice();
    }

    private void Update()
    {
        if (Time.timeScale == 0)
            return;
        SetType();
        ContinueDialog();
        Debug.Log(dialogList.StartBranch);
    }

    private void Initialize()
    {
        if (isQuestDialog)
        {
            Quest quest = DataManager.Instance.GetQuest(questID);
            QuestManager.Instance.CheckQuestProgress(questID);
            QuestManager.Instance.UpdateActiveQuests();
            /*bool questInProgress = quest.Status == Quest.QuestState.Active;
            if (questInProgress)
            {
                for (
                    int i = 0;
                    i < questManager.questItemList[questID].ObjectiveItemList.Count;
                    i++
                )
                {
                    for (
                        int j = 0;
                        j < questManager.questItemList[questID].RewardItemList.Count;
                        j++
                    )
                        SetQuestComplete(
                            questManager.questItemList[questID].ObjectiveItemList[i],
                            questManager.questItemList[questID].RewardItemList[j],
                            questManager.questItemList[questID].RewardMoney
                        );
                }
            }*/
            switch (quest.Status)
            {
                case Quest.QuestState.Inactive:
                    dialogList.StartBranch = "DEFAULT";
                    break;
                case Quest.QuestState.Active:
                    dialogList.StartBranch = "ACTIVE";
                    break;
                case Quest.QuestState.Completed:
                    dialogList.StartBranch = "COMPLETED";
                    QuestManager.Instance.FinishQuest(questID);
                    QuestManager.Instance.GetRewards(questID);
                    break;
                case Quest.QuestState.Rewarded:
                    dialogList.StartBranch = "REWARDED";
                    break;
            }
        }
        else
            dialogList.StartBranch = "DEFAULT";
        currentBranchID = dialogList.StartBranch;
    }

    /* private void SetQuestComplete(
         QuestObjective_SO objectiveItem,
         QuestReward_SO rewardItem,
         int rewardMoney
     )
     {
         for (int i = 0; i < questManager.backpack.ItemList.Count; i++)
         {
             if (
                 objectiveItem.InBackpackItem == questManager.backpack.ItemList[i]
                 && objectiveItem.ItemHeld <= questManager.backpack.ItemList[i].ItemHeld
             )
             {
                 questManager.questList.QuestList[questID].Status = Quest.QuestState.Completed;
                 questManager.backpack.ItemList[i].ItemHeld -= objectiveItem.ItemHeld;
                 BackpackManager.Instance.AddMoney(rewardMoney);
                 if (rewardItem.InBackpackItem != null)
                     BackpackManager.Instance.AddItem(
                         rewardItem.InBackpackItem.ItemIndex,
                         DataManager.Instance.Backpack
                     );
             }
         }
     }*/

 

    private void SetType()
    {
        if (index >= dialogList.DialogList.Count)
        {
            if (inSelection)
                return;
            if (continueBool)
                gameObject.SetActive(false);
            return;
        }
        if (dialogList.DialogList[index].Branch != currentBranchID)
        {
            if (!inSelection)
                index++;
            return;
        }
        switch (dialogList.DialogList[index].Type)
        {
            case "TALK":
                if (continueBool && !inSelection)
                {
                    continueBool = false;
                    if (textFinished)
                    {
                        currentTextWaitTime = maxTextWaitTime;
                        StartCoroutine(SetText());
                    }
                    else
                        currentTextWaitTime = 0;
                }
                break;
            case "CHOICE":
                ChoiceMenu();
                break;
            case "MENU":
                if (continueBool)
                {
                    openMenu = true;
                    index++;
                }
                break;
            case "QUEST":
                QuestManager.Instance.ActivateQuest(questID);
                index++;
                break;
            case "CALL":
                dialogList.StartBranch = dialogList.DialogList[index].Order;
                if (continueBool)
                    gameObject.SetActive(false);
                break;
        }
    }

    private IEnumerator SetText()
    {
        textFinished = false;
        textLabel.text = "";
        SetCharacterInfo();
        for (int i = 0; i < dialogList.DialogList[index].Content.Length; i++)
        {
            textLabel.text += dialogList.DialogList[index].Content[i];
            yield return new WaitForSeconds(currentTextWaitTime);
        }
        textFinished = true;
        index++;
    }

    private void SetCharacterInfo()
    {
        /*switch (dialogList.DialogList[index].TheName)
        {
            case "PLAYER":
                faceImage.sprite = playerFace;
                break;
            case "NPC":
                faceImage.sprite = npcFace;
                break;
        }*/
        characterName.text = dialogList.DialogList[index].TheName;
    }

    private void ChoiceMenu()
    {
        inSelection = true;
        string buttonBranchID = dialogList.DialogList[index].Order;
        GameObject choice;
        choice = Instantiate(choiceButton, choiceManager.transform.position, Quaternion.identity);
        choice.transform.SetParent(choiceManager.transform, false);
        choice.GetComponentInChildren<Text>().text = dialogList.DialogList[index].Content;
        choice
            .GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                GetBranchID(buttonBranchID);
            });
        index++;
    }

    private void GetBranchID(string buttonBranchID)
    {
        if (buttonBranchID == "ACTIVATE")
            QuestManager.Instance.ActivateQuest(questID);
        currentBranchID = buttonBranchID;
        dialogList.StartBranch = buttonBranchID;
        DestroyChoice();
        inSelection = false;
        continueBool = true;
    }

    private void DestroyChoice()
    {
        for (int i = 0; i < choiceManager.transform.childCount; i++)
            Destroy(choiceManager.transform.GetChild(i).gameObject);
    }

    private void ContinueDialog()
    {
        if (
            (
                Input.GetKeyDown(KeyCode.KeypadEnter)
                || Input.GetMouseButtonDown(0)
                || Input.GetButtonDown("A")
            ) && !BlockContinue
        )
            continueBool = true;
    }
}
