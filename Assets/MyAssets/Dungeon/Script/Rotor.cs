using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Rotor : DungeonTrap
{
    [SerializeField]
    private Transform rotorTrans;
    [SerializeField]
    private float rotateOnceDuration;

    protected override void Initialize()
    {
        rotorTrans.DOLocalRotate(new Vector3(0f, 360f, 0f), rotateOnceDuration, RotateMode.FastBeyond360).SetLoops(-1)
       .SetEase(Ease.Linear);
    }
}
