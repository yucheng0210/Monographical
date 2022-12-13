using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonAudio : MonoBehaviour
{
    private void Start()
    {
        AddEventTriggerListener();
    }

    private void AddEventTriggerListener()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            EventTrigger eventTrigger = gameObject.transform
                .GetChild(i)
                .gameObject.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Deselect;
            entry.callback.AddListener(
                (functionIWant) =>
                {
                    AudioManager.Instance.ButtonTouchAudio();
                }
            );
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener(
                (functionIWant) =>
                {
                    AudioManager.Instance.ButtonTouchAudio();
                }
            );
            eventTrigger.triggers.Add(entry);
        }
    }
}
