using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventAnimation : MonoBehaviour
{
    [SerializeField]
    private GameObject nextAction;

    private void Start()
    {
        EventManager.Instance.AddEventRegister(EventDefinition.eventNextMainLine, HandleEvent);
    }

    private void OnDisable()
    {
        EventManager.Instance.DispatchEvent(EventDefinition.eventNextMainLine, this);
        EventManager.Instance.RemoveEventRegister(EventDefinition.eventNextMainLine, HandleEvent);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveEventRegister(EventDefinition.eventNextMainLine, HandleEvent);
    }

    private void HandleEvent(params object[] args)
    {
        nextAction.SetActive(true);
    }
}
