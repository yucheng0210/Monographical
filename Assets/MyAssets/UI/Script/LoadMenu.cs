using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoadMenu : UIBase
{
    [SerializeField]
    private Button firstButton;

    protected override void Start()
    {
        base.Start();
        EventManager.Instance.AddEventRegister(
            EventDefinition.eventSceneLoading,
            EventSceneLoading
        );
    }

    protected override void Update()
    {
        base.Update();
        /* if (OpenBool && EventSystem.current.currentSelectedGameObject == null)
             EventSystem.current.SetSelectedGameObject(firstButton.gameObject);*/
        if (OpenBool && Input.GetKeyDown(KeyCode.Escape))
            Hide();
    }

    private void EventSceneLoading(params object[] args)
    {
        gameObject.SetActive(false);
    }
}
