using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Menu : MonoBehaviour, IObserver
{
    [SerializeField]
    protected GameObject openMenu;

    [SerializeField]
    protected Button touchButton;

    public bool OpenBool { get; set; }
    public static bool menuIsOpen;
    private bool shutDown;

    public enum ActionType
    {
        Open,
        Close
    }

    public ActionType actionType;

    private void Awake()
    {
        openMenu.SetActive(false);
    }

    protected virtual void Start()
    {
        GameManager.Instance.AddObservers(this);
        if (touchButton != null)
            AddOnClickListener();
    }

    private void AddOnClickListener()
    {
        switch (actionType)
        {
            case ActionType.Open:
                touchButton.onClick.AddListener(Open);
                break;
            case ActionType.Close:
                touchButton.onClick.AddListener(Close);
                break;
        }
    }

    public virtual void Open()
    {
        if (shutDown)
            return;
        EventSystem.current.SetSelectedGameObject(null);
        menuIsOpen = true;
        AudioManager.Instance.MenuEnterAudio();
        Time.timeScale = 0;
        openMenu.SetActive(true);
        OpenBool = true;
    }

    public virtual void Close()
    {
        if (shutDown)
            return;
        menuIsOpen = false;
        AudioManager.Instance.MenuExitAudio();
        Time.timeScale = 1;
        openMenu.SetActive(false);
        OpenBool = false;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public virtual void TouchAudio()
    {
        AudioManager.Instance.ButtonTouchAudio();
    }

    public void EndNotify()
    {
        shutDown = true;
    }

    public virtual void SceneLoadingNotify(bool loadingBool)
    {
        Menu.menuIsOpen = false;
        shutDown = loadingBool ? true : false;
        openMenu.SetActive(false);
        //gameObject.SetActive(false);
    }
}
