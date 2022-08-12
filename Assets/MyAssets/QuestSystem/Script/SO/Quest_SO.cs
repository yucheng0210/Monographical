using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/New Quest")]
public class Quest_SO : ScriptableObject
{
    [SerializeField]
    private int id;

    [SerializeField]
    private string theName;

    [SerializeField]
    private string npc;

    [SerializeField]
    private string des;

    [SerializeField]
    private int status;

    [SerializeField]
    private string rewards;

    [SerializeField]
    private string task;

    [SerializeField]
    private int parent;
   #region "Read from Quest_SO"
    public int ID
    {
        get { return id; }
        set { id = value; }
    }
    public string TheName
    {
        get { return theName; }
        set { theName = value; }
    }
    public string NPC
    {
        get { return npc; }
        set { npc = value; }
    }
    public string Des
    {
        get { return des; }
        set { des = value; }
    }
    public int Status
    {
        get { return status; }
        set { status = value; }
    }
    public string Rewards
    {
        get { return rewards; }
        set { rewards = value; }
    }
    public string Task
    {
        get { return task; }
        set { task = value; }
    }
    public int Parent
    {
        get { return parent; }
        set { parent = value; }
    }
   #endregion
}
