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

    public bool IsQuestDialog { get; set; }

    public int QuestID { get; set; }

    public string DialogName { get; set; }
    private string currentBranchID = "DEFAULT";
    private bool continueBool;
    private bool inSelection;

    public bool OpenMenu
    {
        get { return openMenu; }
        set { openMenu = value; }
    }
    public bool BlockContinue { get; set; }

    private void Start()
    {
        EventManager.Instance.AddEventRegister(
            EventDefinition.eventQuestActivate,
            EventQuestActivate
        );
        EventManager.Instance.AddEventRegister(
            EventDefinition.eventQuestCompleted,
            EventQuestCompleted
        );
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
        //SetCharacterInfo();
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
        if (IsQuestDialog)
        {
            QuestManager.Instance.UpdateActiveQuests();
            QuestManager.Instance.CheckQuestProgress(QuestID);
        }
    }

    private void SetType()
    {
        if (index >= DataManager.Instance.DialogList[DialogName].Count)
        {
            if (inSelection)
                return;
            if (continueBool)
                gameObject.SetActive(false);
            return;
        }
        if (DataManager.Instance.DialogList[DialogName][index].Branch != currentBranchID)
        {
            if (!inSelection)
                index++;
            return;
        }
        switch (DataManager.Instance.DialogList[DialogName][index].Type)
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
                QuestManager.Instance.ActivateQuest(QuestID);
                EventManager.Instance.DispatchEvent(EventDefinition.eventQuestNPCMove, QuestID);
                index++;
                break;
            case "CALL":
                if (currentBranchID == "REWARDED")
                    QuestManager.Instance.GetRewards(QuestID);
                if (DataManager.Instance.DialogList[DialogName][index].Order == "PLAYERCANMOVE")
                    EventManager.Instance.DispatchEvent(EventDefinition.eventPlayerCantMove, 0);
                if (continueBool)
                {
                    gameObject.SetActive(false);
                    currentBranchID = DataManager.Instance.DialogList[DialogName][index].Order;
                }
                break;
        }
    }

    private IEnumerator SetText()
    {
        textFinished = false;
        textLabel.text = "";
        SetCharacterInfo();
        for (int i = 0; i < DataManager.Instance.DialogList[DialogName][index].Content.Length; i++)
        {
            textLabel.text += DataManager.Instance.DialogList[DialogName][index].Content[i];
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
        characterName.text = DataManager.Instance.DialogList[DialogName][index].TheName;
    }

    private void ChoiceMenu()
    {
        inSelection = true;
        string buttonBranchID = DataManager.Instance.DialogList[DialogName][index].Order;
        GameObject choice;
        choice = Instantiate(choiceButton, choiceManager.transform.position, Quaternion.identity);
        choice.transform.SetParent(choiceManager.transform, false);
        choice.GetComponentInChildren<Text>().text = DataManager.Instance.DialogList[DialogName][
            index
        ].Content;
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
            QuestManager.Instance.ActivateQuest(QuestID);
        currentBranchID = buttonBranchID;
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

    private void EventQuestActivate(params object[] args)
    {
        currentBranchID = "ACTIVE";
    }

    private void EventQuestCompleted(params object[] args)
    {
        currentBranchID = "COMPLETED";
    }
}
