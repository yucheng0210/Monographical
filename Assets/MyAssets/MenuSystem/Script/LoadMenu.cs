using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadMenu : Menu
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F7))
        {
            if (!OpenBool)
                Open();
            else
                Close();
        }
    }
}
