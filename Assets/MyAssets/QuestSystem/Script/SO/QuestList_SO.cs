using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestList_SO : ScriptableObject
{
    [SerializeField]
    private List<Quest> questList = new List<Quest>();
    public List<Quest> QuestList
    {
        get { return questList; }
        set { questList = value; }
    }
}
