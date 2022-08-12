using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField]
    private TextAsset textFile;
    private List<Quest_SO> questList = new List<Quest_SO>();

    [SerializeField]
    private Inventory_SO backpack;

    private QusetUIManager qusetUIManager;
    private string[] lineData;
    public Inventory_SO Backpack
    {
        get { return backpack; }
        set { backpack = value; }
    }
    public List<Quest_SO> QuestList
    {
        get { return questList; }
    }

    private void Awake()
    {
        qusetUIManager = GetComponent<QusetUIManager>();
    }

    public void QuestActive(DialogSystem dialogSystem, int index)
    {
        QuestList[int.Parse(dialogSystem.DialogList[index].Content)].Status = 1;
        qusetUIManager.RefreshItem();
    }
}
