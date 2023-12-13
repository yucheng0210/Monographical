using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    [SerializeField]
    private float restoreTime;

    [SerializeField]
    private float hitTimeScale = 0.2f;
    public bool IsHitStop { get; private set; }

    private void Update()
    {
        Debug.Log(Time.timeScale);
        if (Menu.menuIsOpen && Time.timeScale != 0)
            Time.timeScale = 0;
    }

    public void StopTime()
    {
        if (IsHitStop && Menu.menuIsOpen)
            return;
        Time.timeScale = hitTimeScale;
        StartCoroutine(WaitRestoreTime());
    }

    private IEnumerator WaitRestoreTime()
    {
        IsHitStop = true;
        yield return new WaitForSecondsRealtime(restoreTime);
        Time.timeScale = 1;
        IsHitStop = false;
    }
}
