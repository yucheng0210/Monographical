using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenu : Menu
{
    /* [SerializeField]
     private Button[] buttons;*/

    protected override void Start()
    {
        base.Start();
        /*buttons[0].onClick.AddListener(Close);
        buttons[1].onClick.AddListener(BackToStartMenu);*/
    }

    private void Update()
    {
        /*if (OpenBool && EventSystem.current.currentSelectedGameObject == null)
            EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);*/
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("+"))
        {
            if (!OpenBool)
                Open();
            else
                Close();
        }
    }

    /*private void BackToStartMenu()
    {
        //SaveLoadManager.Instance.AutoSave();
        Time.timeScale = 1;
        StartCoroutine(SceneController.Instance.Transition("StartMenu"));
        SaveLoadManager.Instance.AutoSave();
    }*/
}
