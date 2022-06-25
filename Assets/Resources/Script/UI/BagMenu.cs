using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagMenu : Menu
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!OpenBool)
                Open();
            else
                Close();
        }
    }
}
