using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogList", menuName = "Dialog/NewDialogList")]
public class DialogList_SO : ScriptableObject
{
    [SerializeField]
    private string startBranch = "DEFAULT";

    private List<Dialog_SO> dialogList = new List<Dialog_SO>();
    public string StartBranch
    {
        get { return startBranch; }
        set { startBranch = value; }
    }
    public List<Dialog_SO> DialogList
    {
        get { return dialogList; }
        set { dialogList = value; }
    }
}
