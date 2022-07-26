using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadOnClicked : MonoBehaviour
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
        button.onClick.AddListener(LoadSuccessfully);
    }

    private void LoadSuccessfully()
    {
        SaveManager.Instance.LoadPlayerData();
    }
}
