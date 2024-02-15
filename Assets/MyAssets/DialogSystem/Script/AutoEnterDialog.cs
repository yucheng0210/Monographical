using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoEnterDialog : MonoBehaviour
{
    [SerializeField]
    private DialogSystem talkUI;
    [SerializeField]
    private string dialogName;
    private bool isEnter;
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player") && !isEnter)
        {
            isEnter = true;
            talkUI.DialogName = dialogName;
            talkUI.gameObject.SetActive(true);
            EventManager.Instance.DispatchEvent(EventDefinition.eventPlayerCantMove, 1);
        }
    }
}
