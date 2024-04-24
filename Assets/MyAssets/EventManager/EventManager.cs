using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventManager : Singleton<EventManager>
{
    public delegate void EventHandler(params object[] args);
    private Dictionary<string, EventHandler> eventListenters = new Dictionary<string, EventHandler>();
    protected override void Awake()
    {
        base.Awake();
        AddEventRegister(EventDefinition.eventSceneLoading, EventSceneLoading);
        DontDestroyOnLoad(this);
    }
    public void AddEventRegister(string eventName, EventHandler handler)
    {
        if (handler == null)
            return;
        if (eventListenters.ContainsKey(eventName))
            eventListenters[eventName] += handler;
        else
            eventListenters.Add(eventName, handler);
    }

    public void RemoveEventRegister(string eventName, EventHandler handler)
    {
        if (handler == null)
            return;
        if (eventListenters.ContainsKey(eventName))
        {
            eventListenters[eventName] -= handler;
            if (eventListenters[eventName] == null)
                eventListenters.Remove(eventName);
        }
    }

    public void DispatchEvent(string eventName, params object[] objs)
    {
        if (eventListenters.ContainsKey(eventName))
        {
            EventHandler eventHandler = eventListenters[eventName];
            if (eventHandler != null)
                eventHandler(objs);
        }
    }

    //刪除觸發事件對應的所有執行事件
    public void ClearEvents(string eventName)
    {
        if (eventListenters.ContainsKey(eventName))
            eventListenters.Remove(eventName);
    }
    private void EventSceneLoading(params object[] args)
    {
        eventListenters.Clear();
        AddEventRegister(EventDefinition.eventSceneLoading, EventSceneLoading);
        AddEventRegister(EventDefinition.eventDialogEvent, EventDialogEvent);
    }
    private void EventDialogEvent(params object[] args)
    {
        if ((string)args[0] == "CHANGESCENE" && SceneManager.GetActiveScene().name == "Prologue_2")
            StartCoroutine(SceneController.Instance.Transition("ChapterOne"));
    }
}
