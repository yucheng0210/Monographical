using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MineSweaperMenu : UIBase
{
    [SerializeField]
    private Button quitButton;
    private Camera mainCamera;
    protected override void Start()
    {
        base.Start();
        EventManager.Instance.AddEventRegister(EventDefinition.eventDialogEvent, EventDialogEvent);
        quitButton.onClick.AddListener(Hide);
        mainCamera = Camera.main;
    }
    public override void Show()
    {
        //base.Show();
        openMenu.SetActive(true);
        OpenBool = true;
        EventManager.Instance.DispatchEvent(EventDefinition.eventPlayerCantMove, 1);
        mainCamera.gameObject.SetActive(false);
    }
    public override void Hide()
    {
        openMenu.SetActive(false);
        OpenBool = false;
        mainCamera.gameObject.SetActive(true);
        EventManager.Instance.DispatchEvent(EventDefinition.eventPlayerCantMove, 0);
    }
    private void EventDialogEvent(params object[] args)
    {
        if ((string)args[0] == "MINE")
            Show();
    }
}
