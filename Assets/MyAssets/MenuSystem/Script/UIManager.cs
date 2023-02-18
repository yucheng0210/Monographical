using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>, IObserver
{
    [SerializeField]
    private SceneFader deadImage;

    [SerializeField]
    private SceneFader wifeDeathImage;

    private void Start()
    {
        EventManager.Instance.AddEventRegister(EventDefinition.eventMainLine, HandleMainLine);
        GameManager.Instance.AddObservers(this);
    }

    private void OnDisable()
    {
        GameManager.Instance.RemoveObservers(this);
    }

    public void EndNotify()
    {
        deadImage.StartCoroutine(deadImage.FadeOutIn(2, 3, false));
    }

    public void SceneLoadingNotify(bool loadingBool)
    {
        //throw new System.NotImplementedException();
    }

    private void HandleMainLine(params object[] args)
    {
        if ((int)args[0] == 3)
            StartCoroutine(wifeDeathImage.FadeOutIn(0, 5, true));
    }
}
