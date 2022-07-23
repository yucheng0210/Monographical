using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Menu : MonoBehaviour
{
    [SerializeField]
    private GameObject openMenu;
    private bool openBool;
    public bool OpenBool { get; set; }

    private void Awake()
    {
        openMenu.SetActive(false);
    }

    protected virtual void Open()
    {
        AudioManager.Instance.MenuEnterAudio();
        Time.timeScale = 0;
        openMenu.SetActive(true);
        OpenBool = true;
    }

    protected virtual void Close()
    {
        AudioManager.Instance.MenuExitAudio();
        Time.timeScale = 1;
        openMenu.SetActive(false);
        OpenBool = false;
    }

    protected virtual void TouchAudio()
    {
        AudioManager.Instance.ButtonTouchAudio();
    }
}
