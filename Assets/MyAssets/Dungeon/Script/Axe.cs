using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Axe : DungeonTrap
{
    [SerializeField]
    private float rotateRange;
    [SerializeField]
    private float rotateOnceDuration;

    protected override void Initialize()
    {
        transform.DOLocalRotate
        (new Vector3(transform.localEulerAngles.x + rotateRange,
         transform.localEulerAngles.y, transform.localEulerAngles.z), rotateOnceDuration)
       .SetLoops(-1, LoopType.Yoyo)
       .SetEase(Ease.InOutQuad);
    }

}
