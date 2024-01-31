using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Saw : DungeonTrap
{
    [SerializeField]
    private Transform sawTrans;
    [SerializeField]
    private float moveRange = 3;
    [SerializeField]
    private float moveOnceDuration;
    [SerializeField]
    private float rotateOnceDuration;

    protected override void Initialize()
    {
        sawTrans.DOLocalRotate(new Vector3(360f, 0f, 0f), rotateOnceDuration, RotateMode.FastBeyond360).SetLoops(-1)
        .SetEase(Ease.Linear);
        sawTrans.parent.DOLocalMoveZ(sawTrans.localPosition.z + moveRange, moveOnceDuration).SetLoops(-1, LoopType.Yoyo)
        .SetEase(Ease.Linear);
    }
}
