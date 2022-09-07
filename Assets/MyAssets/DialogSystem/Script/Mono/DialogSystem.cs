using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    [SerializeField]
    private Text textLabel;

    [SerializeField]
    private Image faceImage;

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
    private Dialog_SO dialog;

    [SerializeField]
    private string currentBranchID = "DEFAULT";
    private bool continueBool;
    private bool inSelection;

    [SerializeField]
    private QuestManager questManager;

    [SerializeField]
    private QuestUIManager questUIManager;

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

    private void Awake()
    {
        GetTextFromFile(textFile);
    }

    private void OnEnable()
    {
        textFinished = false;
        textLabel.text = "";
        currentTextWaitTime = maxTextWaitTime;
        continueBool = true;
        index = 0;
        isTalking = true;
        textFinished = true;
        SetImage();
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
    }

    private void Initialize()
    {
        if (isQuestDialog)
        {
            bool questInProgress = questManager.questList.QuestList[questID].Status == 1;
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
            }
            switch (questManager.questList.QuestList[questID].Status)
            {
                case 0:
                    dialogList.StartBranch = "DEFAULT";
                    break;
                case 2:
                    dialogList.StartBranch = "COMPLETE";
                    questManager.questList.QuestList[questID].Status = 3;
                    break;
                case 3:
                    dialogList.StartBranch = "FINAL";
                    break;
            }
        }
        else
            dialogList.StartBranch = "DEFAULT";
        currentBranchID = dialogList.StartBranch;
    }

    private void SetQuestComplete(
        QuestObjective_SO objectiveItem,
        QuestReward_SO rewardItem,
        int rewardMoney
    )
    {
        for (int i = 0; i < questManager.backpack.ItemList.Count; i++)
        {
            //Debug.Log(objectiveItem.InBackpackItem == questManager.backpack.ItemList[i]);
            // Debug.Log(objectiveItem.ItemHeld >= questManager.backpack.ItemList[i].ItemHeld);
            if (
                objectiveItem.InBackpackItem == questManager.backpack.ItemList[i]
                && objectiveItem.ItemHeld <= questManager.backpack.ItemList[i].ItemHeld
            )
            {
                questManager.questList.QuestList[questID].Status = 2;
                questManager.backpack.ItemList[i].ItemHeld -= objectiveItem.ItemHeld;
                backpackManager.AddMoney(rewardMoney);
                if (rewardItem.InBackpackItem != null)
                    backpackManager.AddItem(rewardItem.InBackpackItem);
            }
        }
    }

    private void GetTextFromFile(TextAsset file)
    {
        dialogList.DialogList.Clear();
        index = 0;
        string[] lineData = file.text.Split(new char[] { '\n' });
        for (int i = 1; i < lineData.Length - 1; i++)
        {
            string[] row = lineData[i].Split(new char[] { ',' });
            if (row[1] == "")
                break;
            dialog = ScriptableObject.CreateInstance<Dialog_SO>();
            dialog.Branch = row[0];
            dialog.Type = row[1];
            dialog.TheName = row[2];
            dialog.Order = row[3];
            dialog.Content = row[4];
            dialogList.DialogList.Add(dialog);
        }
        dialog.DialogList = dialogList.DialogList;
    }

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
                questManager.SetQuestActive(dialogList, index);
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
        SetImage();
        for (int i = 0; i < dialogList.DialogList[index].Content.Length; i++)
        {
            textLabel.text += dialogList.DialogList[index].Content[i];
            yield return new WaitForSeconds(currentTextWaitTime);
        }
        textFinished = true;
        index++;
    }

    private void SetImage()
    {
        switch (dialogList.DialogList[index].TheName)
        {
            case "PLAYER":
                faceImage.sprite = playerFace;
                break;
            case "NPC":
                faceImage.sprite = npcFace;
                break;
        }
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
            .onClick.AddListener(
                () =>
                {
                    GetBranchID(buttonBranchID);
                }
            );
        index++;
        //foreach (GameObject choice in choiceList) { }
    }

    private void GetBranchID(string buttonBranchID)
    {
        currentBranchID = buttonBranchID;
        DestroyChoice();
        /* if (index >= dialogList.Count)
            gameObject.SetActive(false);*/
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
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetMouseButtonDown(0))
            continueBool = true;
    }
}
