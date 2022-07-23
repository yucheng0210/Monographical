using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveMenu : Menu
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            if (!OpenBool)
                Open();
            else
                Close();
        }
    }
}
