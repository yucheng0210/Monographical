using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    [SerializeField]
    private bool isRestoreTime;

    [SerializeField]
    private float restoreTime;
    float testRealTime = 0;
    public bool IsHitStop { get; private set; }

    private void Update()
    {
        if (isRestoreTime && !Menu.menuIsOpen)
        {
            if (Time.timeScale < 1)
            {
                IsHitStop = true;
                Time.timeScale += Time.unscaledDeltaTime / restoreTime;
                testRealTime += Time.unscaledDeltaTime;
            }
            else
            {
                IsHitStop = false;
                Debug.Log(testRealTime);
                Time.timeScale = 1;
                isRestoreTime = false;
            }
        }
    }

    public void StopTime()
    {
        testRealTime = 0;
        Time.timeScale = 0;
        isRestoreTime = true;
    }
}
