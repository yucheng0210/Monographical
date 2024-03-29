using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (SceneManager.GetActiveScene().name == "ChapterOne")
            StartCoroutine(WaitForChangeScene("ChapterOne"));
        else
            StartCoroutine(WaitForChangeScene("StartMenu"));
    }
    private IEnumerator WaitForChangeScene(string sceneName)
    {
        yield return new WaitForSecondsRealtime(3f);
        AudioManager.Instance.DeathClue();
        yield return StartCoroutine(UIManager.Instance.FadeOutIn(canvasGroup, 2, 3, false, 0.5f));
        StartCoroutine(SceneController.Instance.Transition(sceneName));
    }
}
