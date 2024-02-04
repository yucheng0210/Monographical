using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Spike : DungeonTrap
{
    [SerializeField]
    private Transform spikeTrans;
    [SerializeField]
    private float moveValue;
    [SerializeField]
    private float attackOnceDuration;
    [Header("攻擊冷卻")]
    [SerializeField]
    private float attackStartTime;
    [SerializeField]
    private float attackStayTime;
    [SerializeField]
    private float attackCoolDown;
    private Sequence sequence;
    protected override void Initialize()
    {
        StartCoroutine(Attack());
    }
    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(attackStartTime);
        sequence = DOTween.Sequence();
        sequence.Append(spikeTrans.DOLocalMoveY(spikeTrans.localPosition.y + moveValue, attackOnceDuration));
        sequence.AppendInterval(attackStayTime);
        sequence.Append(spikeTrans.DOLocalMoveY(spikeTrans.localPosition.y, attackOnceDuration * 2));
        sequence.AppendInterval(attackCoolDown);
        sequence.Play().SetLoops(-1);
    }
}
