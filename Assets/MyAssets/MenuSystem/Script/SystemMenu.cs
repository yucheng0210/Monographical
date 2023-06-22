using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SystemMenu : UIBase
{
    [SerializeField]
    private Button exitButton;

    protected override void Start()
    {
        base.Start();
        exitButton.onClick.AddListener(ExitGame);
    }

    private void ExitGame()
    {
        SaveLoadManager.Instance.AutoSave();
        //UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
