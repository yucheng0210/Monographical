using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UIBase : MonoBehaviour, IObserver
{
    [SerializeField]
    protected GameObject openMenu;

    [SerializeField]
    protected Button touchButton;
    [SerializeField]
    private bool escOpenBool = false;
    [SerializeField]
    private bool escCloseBool = false;
    public bool OpenBool { get; set; }
    private bool shutDown;

    public enum ActionType
    {
        Open,
        Close
    }

    public ActionType actionType;

    protected virtual void Start()
    {
        UIManager.Instance.UIDict.Add(this.GetType().Name, this);
        if (touchButton != null)
            AddOnClickListener();
    }
    protected virtual void Update()
    {
        if (escOpenBool && !openMenu.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            Show();
        else if (escCloseBool && openMenu.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            Hide();
    }
    private void AddOnClickListener()
    {
        switch (actionType)
        {
            case ActionType.Open:
                touchButton.onClick.AddListener(Show);
                break;
            case ActionType.Close:
                touchButton.onClick.AddListener(Hide);
                break;
        }
    }

    public virtual void Show()
    {
        if (shutDown)
            return;
        try
        {
            UIManager.Instance.MenuIsOpen = true;
            //EventSystem.current.SetSelectedGameObject(null);
            AudioManager.Instance.MenuEnterAudio();
            Time.timeScale = 0;
            openMenu.SetActive(true);
            OpenBool = true;
            openMenu.transform.SetAsLastSibling();
        }
        catch (Exception e)
        { }
    }

    public virtual void Hide()
    {
        if (shutDown)
            return;
        UIManager.Instance.MenuIsOpen = false;
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
        shutDown = loadingBool ? true : false;
        //openMenu.SetActive(false);
        //gameObject.SetActive(false);
    }
}
