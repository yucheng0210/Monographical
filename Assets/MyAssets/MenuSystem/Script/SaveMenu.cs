using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SaveMenu : Menu
{
    [SerializeField]
    private Button firstButton;

    [SerializeField]
    private ClueMenu clueMenu;

    [SerializeField]
    private GameObject saveButtonManager;

    protected override void Start()
    {
        base.Start();
        for (int i = 0; i < saveButtonManager.transform.childCount; i++)
        {
            if (
                saveButtonManager.transform
                    .GetChild(i)
                    .gameObject.GetComponentInChildren<Text>().text != "NODATA"
            )
                saveButtonManager.transform
                    .GetChild(i)
                    .gameObject.GetComponent<Button>()
                    .onClick.AddListener(OpenClueMenu);
        }
    }

    protected override void Update()
    {
        base.Update();
        if (OpenBool && EventSystem.current.currentSelectedGameObject == null && !clueMenu.OpenBool)
            EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
    }

    private void OpenClueMenu()
    {
        clueMenu.Open();
    }

}
