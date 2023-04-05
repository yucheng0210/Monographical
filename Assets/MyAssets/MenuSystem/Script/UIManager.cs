using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>, IObserver
{
    /*[SerializeField]
    private SceneFader deadImage;

    [SerializeField]
    private SceneFader wifeDeathImage;*/
    public Dictionary<string, UIBase> UIDict { get; set; }
    public bool MenuIsOpen { get; set; }

    protected override void Awake()
    {
        base.Awake();
        UIDict = new Dictionary<string, UIBase>();
        MenuIsOpen = false;
    }

    private void Start()
    {
        //EventManager.Instance.AddEventRegister(EventDefinition.eventMainLine, HandleMainLine);
        GameManager.Instance.AddObservers(this);
    }

    private void OnDisable()
    {
        GameManager.Instance.RemoveObservers(this);
    }

    public void ShowUI(string uiName)
    {
        UIDict[uiName].Show();
    }

    public void HideUI(string uiName)
    {
        UIDict[uiName].Hide();
    }

    public void HideAllUI()
    {
        foreach (var i in UIDict)
        {
            HideUI(i.Key);
        }
    }

    public void EndNotify()
    {
        //  deadImage.StartCoroutine(deadImage.FadeOutIn(2, 3, false));
    }

    public void SceneLoadingNotify(bool loadingBool)
    {
        //throw new System.NotImplementedException();
    }

    private void HandleMainLine(params object[] args)
    {
        /*if ((int)args[0] == 3)
            StartCoroutine(wifeDeathImage.FadeOutIn(0, 5, true));*/
    }
}
