using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameExit : MonoBehaviour
{
    public void Exit()
    {
        EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
