using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedSprintEffect : MonoBehaviour
{
    private ParticleSystem myPrticle;
    private void Awake()
    {
        myPrticle = GetComponent<ParticleSystem>();
    }
    private void Update()
    {
        myPrticle.startRotation = transform.parent.eulerAngles.y;
    }
}