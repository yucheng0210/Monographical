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
    [SerializeField]
    private Vector3 rotateDirection;
    protected override void Initialize()
    {
        if (rotorTrans == null)
            rotorTrans = transform;
        rotorTrans.DOLocalRotate(rotorTrans.localEulerAngles + rotateDirection, rotateOnceDuration, RotateMode.FastBeyond360)
        .SetLoops(-1)
       .SetEase(Ease.Linear);
    }
}
