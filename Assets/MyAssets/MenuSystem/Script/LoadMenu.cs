using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoadMenu : Menu
{
    [SerializeField]
    private Button firstButton;

    private void Update()
    {
        if (OpenBool && EventSystem.current.currentSelectedGameObject == null)
            EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
    }
}
