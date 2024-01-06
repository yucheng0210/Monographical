using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ClueMenu : UIBase
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
        noButton.onClick.AddListener(Hide);
    }

    private void SaveData()
    {
        SaveLoadManager.Instance.Save(currentSaveID);
        if (SceneManager.GetActiveScene().name == "StartMenu")
            SaveLoadManager.Instance.Load(currentSaveID);
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
