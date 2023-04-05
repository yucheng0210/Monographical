using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestMenu : UIBase
{
    [SerializeField]
    private QuestUIManager questUIManager;

    public void QuestInitialize()
    {
        questUIManager.Initialize();
    }
}
