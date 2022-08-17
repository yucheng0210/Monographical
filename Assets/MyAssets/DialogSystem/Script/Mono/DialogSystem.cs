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

    [SerializeField]
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

    private List<Dialog_SO> dialogList = new List<Dialog_SO>();

    private Dialog_SO dialog;

    [SerializeField]
    private string currentBranchID = "DEFAULT";
    private bool continueBool;
    private bool inSelection;

    [SerializeField]
    private QuestManager questManager;

    [SerializeField]
    private QusetUIManager qusetUIManager;
    public bool OpenMenu
    {
        get { return openMenu; }
        set { openMenu = value; }
    }
    public List<Dialog_SO> DialogList
    {
        get { return dialogList; }
    }

    private void Awake()
    {
        currentTextWaitTime = maxTextWaitTime;
        GetTextFromFile(textFile);
    }

    private void OnEnable()
    {
        currentTextWaitTime = maxTextWaitTime;
        continueBool = false;
        currentBranchID = "DEFAULT";
        index = 0;
        isTalking = true;
        textFinished = true;
        StartCoroutine(SetText());
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

    private void GetTextFromFile(TextAsset file)
    {
        dialogList.Clear();
        index = 0;
        string[] lineData = file.text.Split(new char[] { '\n' });
        for (int i = 1; i < lineData.Length - 1; i++)
        {
            //Debug.Log(lineData.Length);
            string[] row = lineData[i].Split(new char[] { ',' });
            if (row[1] == "")
                break;
            dialog = ScriptableObject.CreateInstance<Dialog_SO>();
            dialog.Branch = row[0];
            dialog.Type = row[1];
            dialog.TheName = row[2];
            dialog.Order = row[3];
            dialog.Content = row[4];
            dialogList.Add(dialog);
        }
        dialog.DialogList = dialogList;
    }

    private void SetType()
    {
        if (index >= dialogList.Count)
        {
            if (inSelection)
                return;
            if (continueBool)
                gameObject.SetActive(false);
            return;
        }
        if (dialogList[index].Branch != currentBranchID)
        {
            if (!inSelection)
                index++;
            return;
        }
        switch (dialogList[index].Type)
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
                questManager.QuestActive(this, index);
                if (continueBool)
                    gameObject.SetActive(false);
                break;
        }
    }

    private IEnumerator SetText()
    {
        textFinished = false;
        textLabel.text = "";
        switch (dialogList[index].TheName)
        {
            case "PLAYER":
                faceImage.sprite = playerFace;
                break;
            case "NPC":
                faceImage.sprite = npcFace;
                break;
        }
        for (int i = 0; i < dialogList[index].Content.Length; i++)
        {
            textLabel.text += dialogList[index].Content[i];
            yield return new WaitForSeconds(currentTextWaitTime);
        }
        textFinished = true;
        index++;
    }

    private void ChoiceMenu()
    {
        inSelection = true;
        string buttonBranchID = dialogList[index].Order;
        GameObject choice;
        choice = Instantiate(choiceButton, choiceManager.transform.position, Quaternion.identity);
        choice.transform.SetParent(choiceManager.transform, false);
        choice.GetComponentInChildren<Text>().text = dialogList[index].Content;
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
