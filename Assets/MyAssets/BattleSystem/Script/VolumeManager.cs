using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class VolumeManager : MonoBehaviour
{
    public static VolumeManager Instance { get; private set; }
    private FloatParameter vp; // 当前参数

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UnityEngine.Rendering.Volume volume = GetComponent<UnityEngine.Rendering.Volume>();

        List<VolumeComponent> list = volume.profile.components;
        // 获取第一个float参数
        vp = (FloatParameter)list[0].parameters[0];
    }

    /// <summary>
    /// 径向模糊控制
    /// </summary>
    /// <param name="From">初始值</param>
    /// <param name="To">目标值</param>
    /// <param name="Duration">时间</param>
    /// <param name="StartDelay">延迟</param>
    public void DoRadialBlur(float From, float To, float Duration, float StartDelay)
    {
        StartCoroutine(FadeCoroutine(From, To, Duration, StartDelay));
    }

    IEnumerator FadeCoroutine(float From, float To, float Duration, float StartDelay)
    {
        if (StartDelay > 0)
            yield return new WaitForSeconds(StartDelay);

        float t = 0;
        while (t < 1)
        {
            float v = Mathf.Lerp(From, To, t);
            vp.value = v;
            t += Time.deltaTime / Duration;
            yield return null;
        }
        vp.value = To;
        t = 0;
        while (t < 1)
        {
            float v = Mathf.Lerp(To, From, t);
            vp.value = v;
            t += Time.deltaTime / Duration;
            yield return null;
        }
        vp.value = From;
    }
}
