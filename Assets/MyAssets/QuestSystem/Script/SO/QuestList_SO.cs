using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestList_SO : ScriptableObject
{
    [SerializeField]
    private List<Quest_SO> questList = new List<Quest_SO>();
    public List<Quest_SO> QuestList
    {
        get { return questList; }
        set { questList = value; }
    }
}
