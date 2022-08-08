using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestMenu : Menu
{
    [SerializeField]
    private DialogSystem questDialog;

    private void Update()
    {
        if (questDialog.OpenMenu && !OpenBool)
        {
            Open();
            questDialog.OpenMenu = false;
        }
    }
}
