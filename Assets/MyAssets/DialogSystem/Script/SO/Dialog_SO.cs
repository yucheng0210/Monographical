using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "NewDialog", menuName = "Dialog/NewDialog")]
public class Dialog_SO : ScriptableObject
{
    [SerializeField]
    private string branch;

    [SerializeField]
    private string type;

    [SerializeField]
    private string theName;

    [SerializeField]
    private string content;

    [SerializeField]
    private List<Dialog_SO> dialogList = new List<Dialog_SO>();
    #region Read from Dialog
    public string Branch
    {
        get { return branch; }
        set { branch = value; }
    }
    public string Type
    {
        get { return type; }
        set { type = value; }
    }
    public string TheName
    {
        get { return theName; }
        set { theName = value; }
    }
    public string Content
    {
        get { return content; }
        set { content = value; }
    }
    public List<Dialog_SO> DialogList
    {
        get { return dialogList; }
        set { dialogList = value; }
    }
    #endregion
}
