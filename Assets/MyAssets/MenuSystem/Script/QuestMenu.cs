using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestMenu : Menu
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && OpenBool)
            Close();
    }
}
