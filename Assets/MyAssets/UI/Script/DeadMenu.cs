using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadMenu : UIBase
{
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = openMenu.GetComponent<CanvasGroup>();
    }

    protected override void Start()
    {
        base.Start();
        EventManager.Instance.AddEventRegister(EventDefinition.eventGameOver, EventGameOver);
    }

    private void EventGameOver(params object[] args)
    {
        StartCoroutine(UIManager.Instance.FadeOutIn(canvasGroup, 2, 3, false, 1));
    }
}
