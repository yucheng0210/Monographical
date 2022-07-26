using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveOnClicked : MonoBehaviour
{
    private Button button;
    private Text buttonText;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<Text>();
    }

    private void Start()
    {
        button.onClick.AddListener(SaveSuccessfully);
    }

    private void SaveSuccessfully()
    {
        buttonText.text = "已保存";
        SaveManager.Instance.SavePlayerData();
    }
}
