using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestSlot : MonoBehaviour
{
    [SerializeField]
    private Text slotName;
    [SerializeField]
    private GameObject exclamationMark;
    public Text SlotName
    {
        get { return slotName; }
        set { slotName = value; }
    }

    public Quest MyQuest { get; set; }

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClicked);
    }
    private void Start()
    {
        if (!MyQuest.IsNewQuest)
            Destroy(exclamationMark);
    }
    private void OnClicked()
    {
        EventManager.Instance.DispatchEvent(EventDefinition.eventOnClickedToQuest, MyQuest);
        MyQuest.IsNewQuest = false;
        Destroy(exclamationMark);
    }
}
