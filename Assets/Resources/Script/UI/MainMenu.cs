using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : Menu
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!OpenBool)
                Open();
            else
                Close();
        }
    }
}
