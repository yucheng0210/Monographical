using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestObjectiveGrid : MonoBehaviour
{
    private Text objectiveText;
    private Image objectiveImage;
    public Text ObjectiveText
    {
        get { return objectiveText; }
        set { objectiveText = value; }
    }
    public Image ObjectiveImage
    {
        get { return objectiveImage; }
        set { objectiveImage = value; }
    }

    private void Awake()
    {
        objectiveText = GetComponentInChildren<Text>();
        objectiveImage = GetComponentInChildren<Image>();
        //objectiveText.text = "";
    }

    private void OnEnable()
    {
        objectiveText.text = "";
        objectiveImage.sprite = null;
    }
}
