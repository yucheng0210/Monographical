using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    [SerializeField]
    private float fadeTime;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        DontDestroyOnLoad(this);
        //canvasGroup.alpha = 0;
    }

    public IEnumerator FadeOut()
    {
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime / fadeTime;
            yield return null;
        }
    }

    public IEnumerator FadeIn()
    {
        while (canvasGroup.alpha != 0)
        {
            canvasGroup.alpha -= Time.unscaledDeltaTime / fadeTime;
            yield return null;
        }
    }

    public IEnumerator FadeOutIn(float firstWaitTime, float secondWaitTime, bool nextMainLineBool)
    {
        yield return new WaitForSecondsRealtime(firstWaitTime);
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime / fadeTime;
            yield return null;
        }
        yield return new WaitForSecondsRealtime(secondWaitTime);
        while (canvasGroup.alpha != 0)
        {
            canvasGroup.alpha -= Time.deltaTime / fadeTime;
            yield return null;
        }
        if (nextMainLineBool)
            EventManager.Instance.DispatchEvent(EventDefinition.eventNextMainLine, this);
        Destroy(gameObject);
    }
}
