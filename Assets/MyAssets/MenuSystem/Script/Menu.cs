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

    private void Awake()
    {
        openMenu.SetActive(false);
        if (touchButton != null)
            AddEventTriggerListener();
    }

    private void Start()
    {
        GameManager.Instance.AddObservers(this);
    }

    private void AddEventTriggerListener()
    {
        EventTrigger eventTrigger = touchButton.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Select;
        entry.callback.AddListener(
            (functionIWant) =>
            {
                TouchAudio();
            }
        );
        eventTrigger.triggers.Add(entry);
    }

    public virtual void Open()
    {
        if (shutDown)
            return;
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

    public void EndNotify()
    {
        shutDown = true;
    }

    public void SceneLoadingNotify(bool loadingBool)
    {
        shutDown = loadingBool ? true : false;
    }
}
