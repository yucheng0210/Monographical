using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventMainLine : MonoBehaviour
{
    [SerializeField]
    private bool isEnd;
    private int mainLineID;

    private void Start()
    {
        mainLineID = gameObject.transform.GetSiblingIndex();
        //if (mainLineID == 0)
        EventManager.Instance.AddEventRegister(
            EventDefinition.eventNextMainLine,
            HandleNextMainLine
        );
        EventManager.Instance.DispatchEvent(EventDefinition.eventMainLine, mainLineID);
    }

    private void OnDisable()
    {
        try
        {
            EventManager.Instance.DispatchEvent(EventDefinition.eventNextMainLine, this);
        }
        catch { }
    }

    private void HandleNextMainLine(params object[] args)
    {
        EventManager.Instance.RemoveEventRegister(
            EventDefinition.eventNextMainLine,
            HandleNextMainLine
        );
        if (!isEnd)
            gameObject.transform.parent.GetChild(mainLineID + 1).gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
