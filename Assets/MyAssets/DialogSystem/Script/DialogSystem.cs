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
    List<string> textList = new List<string>();
    private bool openMenu;
    public static bool isTalking;
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
        index = 0;
        isTalking = true;
        textFinished = true;
        StartCoroutine(SetText());
    }

    private void OnDisable()
    {
        isTalking = false;
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetMouseButtonDown(0)))
        {
            if (textFinished)
            {
                currentTextWaitTime = maxTextWaitTime;
                if (index == textList.Count)
                    gameObject.SetActive(false);
                else
                    StartCoroutine(SetText());
            }
            else
                currentTextWaitTime = 0;
        }
    }

    private void GetTextFromFile(TextAsset file)
    {
        textList.Clear();
        index = 0;
        var lineData = file.text.Split('\n');
        foreach (var line in lineData)
            textList.Add(line);
    }

    IEnumerator SetText()
    {
        textFinished = false;
        textLabel.text = "";
        switch (textList[index])
        {
            case "A":
                faceImage.sprite = playerFace;
                index++;
                break;
            case "B":
                faceImage.sprite = NPCFace;
                index++;
                break;
            case "MENU":
                openMenu = true;
                gameObject.SetActive(false);
                break;
        }
        for (int i = 0; i < textList[index].Length; i++)
        {
            textLabel.text += textList[index][i];
            yield return new WaitForSeconds(currentTextWaitTime);
        }
        textFinished = true;
        index++;
    }

    private void ChoicesMenu() { }
}
