using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Axe : DungeonTrap
{
    [SerializeField]
    private Vector3 rotateRange;
    [SerializeField]
    private float rotateOnceDuration;

    protected override void Initialize()
    {
        transform.DOLocalRotate(transform.localEulerAngles + rotateRange, rotateOnceDuration)
       .SetLoops(-1, LoopType.Yoyo)
       .SetEase(Ease.InOutQuad);
    }

}
