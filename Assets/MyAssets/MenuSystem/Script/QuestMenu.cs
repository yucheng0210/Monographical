using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestMenu : Menu
{
    [SerializeField]
    private QuestUIManager questUIManager;

    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Escape) && OpenBool)
            Close();
    }

    public void QuestInitialize()
    {
        questUIManager.Initialize();
    }
}
