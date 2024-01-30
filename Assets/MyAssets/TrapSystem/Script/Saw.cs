using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Saw : MonoBehaviour
{
    [SerializeField]
    private Transform sawTrans;
    [SerializeField]
    private float moveRange = 3;
    [SerializeField]
    private float moveOnceDuration;
    [SerializeField]
    private float rotateOnceDuration;
    private void Start()
    {
        Initialize();
    }
    private void Initialize()
    {
        sawTrans.DOLocalRotate(new Vector3(360f, 0f, 0f), rotateOnceDuration, RotateMode.FastBeyond360).SetLoops(-1)
        .SetEase(Ease.Linear);
        sawTrans.DOLocalMoveZ(sawTrans.localPosition.z + moveRange, moveOnceDuration).SetLoops(-1, LoopType.Yoyo)
        .SetEase(Ease.Linear);
    }
}
