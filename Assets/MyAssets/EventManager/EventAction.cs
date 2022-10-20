using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventAction : MonoBehaviour
{
    [SerializeField]
    private GameObject nextAction;

    [SerializeField]
    private bool isEnd;

    public enum ThisActionType
    {
        Conversation,
        Animation,
        GameStart
    }

    public ThisActionType thisActionType;

    private void Start()
    {
        switch (thisActionType)
        {
            case ThisActionType.Conversation:
                EventManager.Instance.AddEventRegister(
                    EventDefinition.eventNextMainLine,
                    HandleAction
                );
                break;
            case ThisActionType.Animation:
                EventManager.Instance.AddEventRegister(
                    EventDefinition.eventNextMainLine,
                    HandleAction
                );
                EventManager.Instance.DispatchEvent(EventDefinition.eventAnimation, this);
                break;
            case ThisActionType.GameStart:
                EventManager.Instance.AddEventRegister(
                    EventDefinition.eventNextMainLine,
                    HandleAction
                );
                EventManager.Instance.DispatchEvent(EventDefinition.eventGameStart, this);
                break;
        }
    }

    private void OnDisable()
    {
        EventManager.Instance.DispatchEvent(EventDefinition.eventNextMainLine, this);
        EventManager.Instance.RemoveEventRegister(EventDefinition.eventNextMainLine, HandleAction);
    }

    private void HandleAction(params object[] args)
    {
        gameObject.SetActive(false);
        if (!isEnd)
            nextAction.SetActive(true);
    }
}
