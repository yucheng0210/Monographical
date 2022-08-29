using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealAnimation : MonoBehaviour
{
    private Animation ani;
    private float realTime;

    private void Awake()
    {
        ani = GetComponent<Animation>();
    }

    private void Update()
    {
        realTime += Time.unscaledDeltaTime;
        foreach (AnimationState animationState in ani)
        {
            if (ani.IsPlaying(ani.name))
                animationState.normalizedSpeed = realTime / animationState.speed;
        }
        ani.Sample();
    }
}
