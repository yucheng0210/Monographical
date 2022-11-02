using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionMenu : Menu
{
    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Escape) && OpenBool)
            Close();
    }
}
