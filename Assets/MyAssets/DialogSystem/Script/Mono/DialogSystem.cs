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
        NPCFace;

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

    private List<Dialog_SO> dialogList = new List<Dialog_SO>();

    private Dialog_SO dialog;

    [SerializeField]
    private string branchID = "DEFAULT";
    private bool continueBool;
    public bool OpenMenu
    {
        get { return openMenu; }
        set { openMenu = value; }
    }

    private void Awake()
    {
        currentTextWaitTime = maxTextWaitTime;
        GetTextFromFile(textFile);
    }

    private void OnEnable()
    {
        continueBool = false;
        branchID = "DEFAULT";
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
            string[] row = lineData[i].Split(new char[] { ',' });
            if (row[1] == "")
                break;
            dialog = ScriptableObject.CreateInstance<Dialog_SO>();
            dialog.Branch = row[0];
            dialog.Type = row[1];
            dialog.CharacterName = row[2];
            dialog.Dialog = row[3];
            dialogList.Add(dialog);
        }
        dialog.DialogList = dialogList;
    }

    private void SetType()
    {
        if (index >= dialogList.Count)
        {
            if (InSelection())
                return;
            if (continueBool)
                gameObject.SetActive(false);
            return;
        }
        if (dialogList[index].Branch != branchID)
        {
            if (!InSelection())
                index++;
            return;
        }
        switch (dialogList[index].Type)
        {
            case "TALK":
                if (continueBool)
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
        }
    }

    private IEnumerator SetText()
    {
        textFinished = false;
        textLabel.text = "";
        switch (dialogList[index].CharacterName)
        {
            case "PLAYER":
                faceImage.sprite = playerFace;
                break;
            case "NPC":
                faceImage.sprite = NPCFace;
                break;
            /*case "MENU":
                openMenu = true;
                gameObject.SetActive(false);
                break;
            case "CHOICE":
                index++;
                ChoiceMenu(int.Parse(textList[index]));
                //gameObject.SetActive(false);
                break;*/
        }
        for (int i = 0; i < dialogList[index].Dialog.Length; i++)
        {
            textLabel.text += dialogList[index].Dialog[i];
            yield return new WaitForSeconds(currentTextWaitTime);
        }
        textFinished = true;
        index++;
    }

    private void ChoiceMenu()
    {
        string buttonBranchID = dialogList[index].CharacterName;
        GameObject choice;
        choice = Instantiate(choiceButton, choiceManager.transform.position, Quaternion.identity);
        choice.transform.SetParent(choiceManager.transform, false);
        choice.GetComponentInChildren<Text>().text = dialogList[index].Dialog;
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
        branchID = buttonBranchID;
        DestroyChoice();
        if (index >= dialogList.Count)
            gameObject.SetActive(false);
        continueBool = true;
    }

    private void DestroyChoice()
    {
        for (int i = 0; i < choiceManager.transform.childCount; i++)
            Destroy(choiceManager.transform.GetChild(i).gameObject);
    }

    private bool InSelection()
    {
        if (choiceManager.transform.childCount > 0)
            return true;
        else
            return false;
    }

    private void ContinueDialog()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetMouseButtonDown(0))
            continueBool = true;
    }
}
