using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookMenu : Menu
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (!OpenBool)
                Open();
            else
                Close();
        }
    }
}
