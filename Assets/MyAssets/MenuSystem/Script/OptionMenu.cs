using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionMenu : Menu
{
    [SerializeField]
    private Button[] buttons;
    private float y;
    private int selectionBoxIndex = 0;

    private void Update()
    {
        //y = Input.GetAxis("Vertical");
        if ((Input.GetKeyDown(KeyCode.Escape)||Input.GetButtonDown("B")) && OpenBool)
            Close();
        /*if (OpenBool || y == 0)
        {
            if (selectionBoxIndex > buttons.Length - 1)
                selectionBoxIndex = 0;
            if (selectionBoxIndex < 0)
                selectionBoxIndex = buttons.Length - 1;
            if (y > 0)
            {
                EventSystem.current.SetSelectedGameObject(buttons[selectionBoxIndex].gameObject);
                selectionBoxIndex--;
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(buttons[selectionBoxIndex].gameObject);
                selectionBoxIndex++;
            }
        }*/
    }

    public override void Open()
    {
        base.Open();
        /*EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);*/
    }
}
