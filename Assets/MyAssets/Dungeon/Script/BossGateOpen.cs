using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BossGateOpen : OpenDoor
{
    [SerializeField]
    private CanvasGroup fadeMenu;
    [SerializeField]
    private Transform playerStartTrans;
    [SerializeField]
    private PlayableDirector bossStage_1TimeLine;
    protected override IEnumerator Open()
    {
        AudioManager.Instance.OpenDoor();
        StartCoroutine(base.Open());
        StartCoroutine(UIManager.Instance.FadeOutIn(fadeMenu, 0, 3, false, 4f));
        yield return new WaitForSecondsRealtime(4);
        Main.Manager.GameManager.Instance.PlayerTrans.position = playerStartTrans.position;
        yield return new WaitForSecondsRealtime(3.75f);
        bossStage_1TimeLine.Play();
    }
}
