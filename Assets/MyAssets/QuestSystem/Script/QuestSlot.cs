using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestSlot : MonoBehaviour
{
    [SerializeField]
    private Text slotName;

    public Text SlotName
    {
        get { return slotName; }
        set { slotName = value; }
    }

    public Quest MyQuest { get; set; }

    private void Awake()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        EventManager.Instance.DispatchEvent(EventDefinition.eventOnClickedToQuest, MyQuest);
    }
}
