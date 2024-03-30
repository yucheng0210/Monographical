using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadMenu : UIBase
{
    [SerializeField]
    private Button firstButton;

    protected override void Start()
    {
        base.Start();
        EventManager.Instance.AddEventRegister(EventDefinition.eventSceneLoading, EventSceneLoading);
        gameObject.SetActive(true);
    }

    private void EventSceneLoading(params object[] args)
    {
        if (SceneManager.GetActiveScene().name == "StartMenu")
        {
            openMenu.SetActive(false);
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventSceneLoading, EventSceneLoading);
        }
    }
}
