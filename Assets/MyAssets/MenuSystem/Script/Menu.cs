using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Menu : MonoBehaviour
{
    [SerializeField]
    private GameObject openMenu;
    public bool OpenBool { get; set; }
    public static bool menuIsOpen;

    private void Awake()
    {
        openMenu.SetActive(false);
    }

    public virtual void Open()
    {
        menuIsOpen = true;
        AudioManager.Instance.MenuEnterAudio();
        Time.timeScale = 0;
        openMenu.SetActive(true);
        OpenBool = true;
    }

    public virtual void Close()
    {
        menuIsOpen = false;
        AudioManager.Instance.MenuExitAudio();
        Time.timeScale = 1;
        openMenu.SetActive(false);
        OpenBool = false;
    }

    public virtual void TouchAudio()
    {
        AudioManager.Instance.ButtonTouchAudio();
    }
}
