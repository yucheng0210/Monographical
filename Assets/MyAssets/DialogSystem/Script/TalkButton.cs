using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkButton : MonoBehaviour
{
    public GameObject button;
    [SerializeField]
    private DialogSystem talkUI;
    [SerializeField]
    private string dialogName;
    [SerializeField]
    private bool isQuest;
    [SerializeField]
    private int questID;
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            talkUI.QuestID = questID;
            talkUI.DialogName = dialogName;
            talkUI.IsQuestDialog = isQuest;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !talkUI.gameObject.activeSelf)
            button.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            button.SetActive(false);
            talkUI.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (button.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            talkUI.gameObject.SetActive(true);
            button.gameObject.SetActive(false);
        }
    }
}
