using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameExit : MonoBehaviour
{
    [SerializeField]
    private Button exitButton;

    private void Awake()
    {
        exitButton.onClick.AddListener(ExitGame);
        AddEventTriggerListener();
    }

    private void AddEventTriggerListener()
    {
        EventTrigger eventTrigger = exitButton.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Select;
        entry.callback.AddListener(
            (functionIWant) =>
            {
                TouchAudio();
            }
        );
        eventTrigger.triggers.Add(entry);
    }

    public void ExitGame()
    {
        EditorApplication.isPlaying = false;
        Application.Quit();
    }

    private void TouchAudio()
    {
        AudioManager.Instance.ButtonTouchAudio();
    }
}
