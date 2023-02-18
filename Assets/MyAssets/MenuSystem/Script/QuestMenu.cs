using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestMenu : UIBase
{
    [SerializeField]
    private QuestUIManager questUIManager;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && OpenBool)
            Close();
    }

    public void QuestInitialize()
    {
        questUIManager.Initialize();
    }
}
