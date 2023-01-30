using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ClueMenu : Menu
{
    [SerializeField]
    private Button firstButton;

    [SerializeField]
    private Button yesButton;

    [SerializeField]
    private Button noButton;
    public static int currentSaveID;

    protected override void Start()
    {
        base.Start();
        yesButton.onClick.AddListener(SaveData);
        noButton.onClick.AddListener(Close);
    }

    private void Update()
    {
        if (OpenBool && EventSystem.current.currentSelectedGameObject == null)
            EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
    }

    private void SaveData()
    {
        SaveLoadManager.Instance.Save(currentSaveID);
        if (SceneManager.GetActiveScene().name == "StartMenu")
            SaveLoadManager.Instance.Load(currentSaveID);
        Time.timeScale = 1;
    }
}
