using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    public delegate void EventDelegate(object[] args);
    private Dictionary<string, Dictionary<int, EventDelegate>> eventListenters = new Dictionary<
        string,
        Dictionary<int, EventDelegate>
    >();

    public void AddEventRegister(string eventName, EventDelegate handler)
    {
        if (handler == null)
            return;
        if (!eventListenters.ContainsKey(eventName))
            eventListenters.Add(eventName, new Dictionary<int, EventDelegate>());
        var handlerDic = eventListenters[eventName];
        var handlerHash = handler.GetHashCode();
        if (handlerDic.ContainsKey(handlerHash))
            handlerDic.Remove(handlerHash);
        eventListenters[eventName].Add(handlerHash.GetHashCode(), handler);
    }

    public void RemoveEventRegister(string eventName, EventDelegate handler)
    {
        if (handler == null)
            return;
        if (eventListenters.ContainsKey(eventName))
        {
            eventListenters[eventName].Remove(handler.GetHashCode());
            if (eventListenters[eventName] == null || eventListenters[eventName].Count == 0)
                eventListenters.Remove(eventName);
        }
    }

    public void DispatchEvent(string eventName, params object[] objs)
    {
        if (eventListenters.ContainsKey(eventName))
        {
            var handlerDic = eventListenters[eventName];
            if (handlerDic != null && handlerDic.Count > 0)
            {
                var dic = new Dictionary<int, EventDelegate>(handlerDic);
                foreach (var i in dic.Values)
                {
                    try
                    {
                        i(objs);
                    }
                    catch (System.Exception)
                    {
                        throw;
                    }
                }
            }
        }
    }

    //刪除觸發事件對應的所有執行事件
    public void ClearEvents(string eventName)
    {
        if (eventListenters.ContainsKey(eventName))
            eventListenters.Remove(eventName);
    }
}
