using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveOnClicked : MonoBehaviour
{
    private Button button;
    private Text buttonText;
    private int saveID;
#region "SaveManager"
    /*
    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<Text>();
    }

    private void Start()
    {
        button.onClick.AddListener(SaveSuccessfully);
        saveID = gameObject.transform.GetSiblingIndex();
        buttonText.text = SaveManager.Instance.HasData(saveID);
    }

    private void OnEnable()
    {
        buttonText.text = SaveManager.Instance.HasData(saveID);
    }

    private void SaveSuccessfully()
    {
        SaveManager.Instance.SavePlayerData(saveID);
        buttonText.text = SaveManager.Instance.HasData(saveID);
    }
    */
    #endregion
    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<Text>();
    }

    private void Start()
    {
        button.onClick.AddListener(SaveData);
    }

    private void OnEnable()
    {
        saveID = gameObject.transform.GetSiblingIndex();
        buttonText.text = SaveLoadManager.Instance.GetDataName(saveID);
    }

    private void SaveData()
    {
        if (SaveLoadManager.Instance.GetDataName(saveID) != "NODATA")
            ClueMenu.currentSaveID = saveID;
        else
        {
            SaveLoadManager.Instance.Save(saveID);
            Time.timeScale = 1;
        }
    }
}
