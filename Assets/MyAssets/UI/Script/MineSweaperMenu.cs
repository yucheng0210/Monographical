using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
public class MineSweaperMenu : UIBase
{
    [SerializeField]
    private Button quitButton;
    [SerializeField]
    private Button wonQuitButton;
    [SerializeField]
    private Button loseQuitButton;
    [SerializeField]
    private Transform mineGroupTrans;
    [SerializeField]
    private PlayableDirector loadDirector = null;
    [SerializeField]
    private PlayableDirector unloadDirector = null;
    private Camera mainCamera;
    protected override void Start()
    {
        base.Start();
        EventManager.Instance.AddEventRegister(EventDefinition.eventDialogEvent, EventDialogEvent);
        quitButton.onClick.AddListener(Hide);
        wonQuitButton.onClick.AddListener(Hide);
        loseQuitButton.onClick.AddListener(Hide);
        mainCamera = Camera.main;
    }
    public override void Show()
    {
        //base.Show();
        StartCoroutine(LoadShow());
    }
    private IEnumerator LoadShow()
    {
        UIManager.Instance.MenuIsOpen = true;
        unloadDirector.gameObject.SetActive(false);
        if (loadDirector != null)
        {
            EventManager.Instance.DispatchEvent(EventDefinition.eventPlayerCantMove, 1);
            mainCamera.gameObject.SetActive(false);
            openMenu.SetActive(true);
            loadDirector.Stop();
            loadDirector.time = 0;
            loadDirector.Evaluate();
            loadDirector.Play();
            yield return new WaitForSecondsRealtime((float)loadDirector.duration);
        }
        OpenBool = true;
        for (int y = 0; y < Main.Manager.GameManager.Instance.MinePosList.GetLength(0); y++)
        {
            for (int x = 0; x < Main.Manager.GameManager.Instance.MinePosList.GetLength(1); x++)
            {
                if (Main.Manager.GameManager.Instance.MinePosList[y, x])
                    mineGroupTrans.GetChild(y).GetChild(x).gameObject.SetActive(true);
            }
        }
    }
    public override void Hide()
    {
        StartCoroutine(LoadHide());
    }
    private IEnumerator LoadHide()
    {
        UIManager.Instance.MenuIsOpen = false;
        unloadDirector.gameObject.SetActive(true);
        if (unloadDirector != null)
        {
            quitButton.onClick.RemoveAllListeners();
            wonQuitButton.onClick.RemoveAllListeners();
            loseQuitButton.onClick.RemoveAllListeners();
            unloadDirector.Stop();
            unloadDirector.time = 0;
            unloadDirector.Evaluate();
            unloadDirector.Play();
            yield return new WaitForSecondsRealtime((float)unloadDirector.duration);
        }
        openMenu.SetActive(false);
        OpenBool = false;
        mainCamera.gameObject.SetActive(true);
        EventManager.Instance.DispatchEvent(EventDefinition.eventPlayerCantMove, 0);
        quitButton.onClick.AddListener(Hide);
        wonQuitButton.onClick.AddListener(Hide);
        loseQuitButton.onClick.AddListener(Hide);
    }
    private void EventDialogEvent(params object[] args)
    {
        if ((string)args[0] == "MINE")
            Show();
    }
}
