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
        button.onClick.AddListener(SaveManager.Instance.SavePlayerData);
        button.onClick.AddListener(Refresh);
    }

    private void Refresh()
    {
        buttonText.text = "已保存";
    }
}
