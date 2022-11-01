using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameExit : MonoBehaviour
{
    [SerializeField]
    private Button exitButton;

    private void Awake()
    {
        exitButton.onClick.AddListener(ExitGame);
    }

    public void ExitGame()
    {
        //EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
