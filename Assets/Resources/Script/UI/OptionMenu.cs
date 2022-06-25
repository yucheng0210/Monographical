using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionMenu : Menu
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && OpenBool)
            Close();
    }
}
