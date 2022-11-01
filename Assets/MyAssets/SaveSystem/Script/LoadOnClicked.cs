using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadOnClicked : MonoBehaviour
{
    private Button button;
    private Text buttonText;
    private int loadID;
#region "SaveManager"
/*    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<Text>();
    }

    private void Start()
    {
        button.onClick.AddListener(LoadSuccessfully);
        loadID = gameObject.transform.GetSiblingIndex();
        buttonText.text = SaveManager.Instance.HasData(loadID);
    }

    private void OnEnable()
    {
        buttonText.text = SaveManager.Instance.HasData(loadID);
    }

    private void LoadSuccessfully()
    {
        SaveManager.Instance.LoadPlayerData(loadID);
        buttonText.text = SaveManager.Instance.HasData(loadID);
    }*/
    #endregion
    private void Awake() 
    {
    button=GetComponent<Button>();    
    }
}
