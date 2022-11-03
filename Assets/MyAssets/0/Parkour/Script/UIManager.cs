using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, IObserver
{
    [SerializeField]
    private SceneFader deadImage;

    [SerializeField]
    private SceneFader wifeDeathImage;

    private void Start()
    {
        EventManager.Instance.AddEventRegister(EventDefinition.eventMenuOpen, HandleMenuOpen);
        GameManager.Instance.AddObservers(this);
    }

    private void OnDisable()
    {
        GameManager.Instance.RemoveObservers(this);
    }

    public void EndNotify()
    {
        deadImage.StartCoroutine(deadImage.FadeOutIn(2, 3));
    }

    public void SceneLoadingNotify(bool loadingBool)
    {
//        throw new System.NotImplementedException();
    }

    private void HandleMenuOpen(params object[] args)
    {
        wifeDeathImage.StartCoroutine(wifeDeathImage.FadeOutIn(0, 5));
    }
}
