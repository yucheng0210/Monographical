using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    public Text textLabel;
    public Image faceImage;
    public Sprite playerFace,
        nPCFace;
    public TextAsset textFile;
    public int index;
    public float maxTextWaitTime;
    public float currentTextWaitTime;
    private bool textFinished;
    List<string> textList = new List<string>();
    public static bool isTalking;

    private void Awake()
    {
        currentTextWaitTime = maxTextWaitTime;
        GetTextFromFile(textFile);
    }

    private void OnEnable()
    {
        isTalking = true;
        textFinished = true;
        StartCoroutine(SetText());
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetMouseButtonDown(0)))
        {
            if (textFinished)
            {
                currentTextWaitTime = maxTextWaitTime;
                if (index == textList.Count)
                {
                    gameObject.SetActive(false);
                    index = 0;
                    isTalking = false;
                }
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
        {
            textList.Add(line);
        }
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
                faceImage.sprite = nPCFace;
                index++;
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
    private void ChoicesMenu()
    {
        
    }
}
