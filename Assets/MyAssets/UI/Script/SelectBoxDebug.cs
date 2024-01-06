using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectBoxDebug : MonoBehaviour
{
    [SerializeField]
    private Button firstButton;

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null && !Menu.menuIsOpen)
            EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
    }
}
