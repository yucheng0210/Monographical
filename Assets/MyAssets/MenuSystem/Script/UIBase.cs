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
        GameManager.Instance.AddObservers(this);
        if (touchButton != null)
            AddOnClickListener();
    }

    private void OnDisable()
    {
        GameManager.Instance.RemoveObservers(this);
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
        UIManager.Instance.MenuIsOpen = true;
        EventSystem.current.SetSelectedGameObject(null);
        AudioManager.Instance.MenuEnterAudio();
        Time.timeScale = 0;
        openMenu.SetActive(true);
        OpenBool = true;
        openMenu.transform.SetAsLastSibling();
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
        Menu.menuIsOpen = false;
        shutDown = loadingBool ? true : false;
        openMenu.SetActive(false);
        //gameObject.SetActive(false);
    }
}
