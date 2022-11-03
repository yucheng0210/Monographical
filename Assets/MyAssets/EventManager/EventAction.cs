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
        GameStart,
        MenuOpen,
        AutoSave,
        Initialize
    }

    public ThisActionType thisActionType;

    private void Start()
    {
        ActionType();
    }

    private void ActionType()
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
            case ThisActionType.MenuOpen:
                EventManager.Instance.AddEventRegister(
                    EventDefinition.eventNextMainLine,
                    HandleAction
                );
                EventManager.Instance.DispatchEvent(EventDefinition.eventMenuOpen, this);
                break;
            case ThisActionType.AutoSave:
                EventManager.Instance.AddEventRegister(
                    EventDefinition.eventNextMainLine,
                    HandleAction
                );
                EventManager.Instance.DispatchEvent(EventDefinition.eventAutoSave, this);
                gameObject.SetActive(false);
                break;
            case ThisActionType.Initialize:
                EventManager.Instance.AddEventRegister(
                    EventDefinition.eventNextMainLine,
                    HandleAction
                );
                EventManager.Instance.DispatchEvent(EventDefinition.eventInitialize, this);
                gameObject.SetActive(false);
                break;
        }
    }

    private void OnDisable()
    {
        try
        {
            EventManager.Instance.DispatchEvent(EventDefinition.eventNextMainLine, this);
            EventManager.Instance.RemoveEventRegister(
                EventDefinition.eventNextMainLine,
                HandleAction
            );
        }
        catch { }
    }

    private void HandleAction(params object[] args)
    {
        gameObject.SetActive(false);
        if (!isEnd)
            nextAction.SetActive(true);
    }
}
