using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookMenu : Menu
{
    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (!OpenBool)
                Open();
            else
                Close();
        }
    }
}
