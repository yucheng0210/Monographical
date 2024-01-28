using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    public bool IsHitStop { get; private set; }

    private void Update()
    {
        if (Menu.menuIsOpen && Time.timeScale != 0)
            Time.timeScale = 0;
    }

    public void StopTime(float hitTimeScale, float restoreTime)
    {
        if (IsHitStop && Menu.menuIsOpen)
            return;
        Time.timeScale = hitTimeScale;
        StartCoroutine(WaitRestoreTime(restoreTime));
    }

    private IEnumerator WaitRestoreTime(float restoreTime)
    {
        IsHitStop = true;
        yield return new WaitForSecondsRealtime(restoreTime);
        Time.timeScale = 1;
        IsHitStop = false;
    }
}
