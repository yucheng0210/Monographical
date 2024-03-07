using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;

public class Boss : PatrolEnemy
{
    [SerializeField]
    private GameObject anotherCollision;
    [Header("戰鬥額外特效")]
    [SerializeField]
    private GameObject rockExplosionEffect;
    [SerializeField]
    private GameObject sprintEffect;
    [Header("跳砍攻擊")]
    [SerializeField]
    private float axThrowingOnceDuration;
    [SerializeField]
    private Vector3 rotateDirection;

    [SerializeField]
    private float heightOffset;
    [SerializeField]
    private float jumpHeight;
    [SerializeField]
    private float jumpOnceDuration;
    [SerializeField]
    private float minStagnantTime;
    [SerializeField]
    private float maxStagnantTime;
    [SerializeField]
    private float chopOnceDuration;
    [SerializeField]
    private float chopDownImpulseForce;
    [Header("衝刺攻擊")]
    [SerializeField]
    private float sprintOffsetZ;
    [SerializeField]
    private float sprintOnceDuration;
    [Header("怒吼")]
    [SerializeField]
    private UnityEngine.Rendering.Volume mainVolumeProfile;
    [SerializeField]
    private float roarIntensity;
    [SerializeField]
    private float roarOnceDuration;
    private GameObject leftAxe;
    protected override void AdditionalAttack()
    {
        meleeAttackCount = 3;
        base.AdditionalAttack();
    }
    protected override void AdditionalLongDistanceAttack()
    {
        longDistanceAttackCount = 3;
        base.AdditionalLongDistanceAttack();
    }
    private IEnumerator Roar()
    {
        if (mainVolumeProfile.profile.TryGet(out RadialBlur radialBlur))
            radialBlur.intensity.Override(roarIntensity);
        myImpulse.GenerateImpulse();
        yield return new WaitForSeconds(roarOnceDuration);
        radialBlur.intensity.Override(0);
    }
    public override void ColliderSwitch(int switchCount)
    {
        base.ColliderSwitch(switchCount);
        if (switchCount == 1)
            anotherCollision.SetActive(true);
        else
            anotherCollision.SetActive(false);
    }
    public void SprintAttack()
    {
        Look(Player.transform.position);
        transform.DOMove(Player.transform.position, sprintOnceDuration);
        GameObject effect = Instantiate(sprintEffect, transform);
        effect.transform.localPosition += new Vector3(0, 0, sprintOffsetZ);
        StartCoroutine(SetParentNull(effect));
    }
    private IEnumerator SetParentNull(GameObject effect)
    {
        yield return new WaitForSeconds(sprintOnceDuration);
        effect.transform.SetParent(null);
    }
    public void AxThrowing()
    {
        leftAxe = Instantiate(anotherCollision.transform.parent.gameObject,
        anotherCollision.transform.parent.parent);
        leftAxe.transform.SetParent(null);
        anotherCollision.transform.parent.gameObject.SetActive(false);
        leftAxe.transform.DORotate(leftAxe.transform.eulerAngles + new Vector3(0, rotateDirection.y, rotateDirection.z), 0);
        leftAxe.transform.DORotate(leftAxe.transform.eulerAngles + rotateDirection, axThrowingOnceDuration
        , RotateMode.FastBeyond360);
        leftAxe.transform.DOMove(Player.transform.position + transform.up * heightOffset, axThrowingOnceDuration);
        StartCoroutine(GenerateRockBreak());
    }
    private IEnumerator GenerateRockBreak()
    {
        yield return new WaitForSeconds(axThrowingOnceDuration);
        Destroy(Instantiate(RockBreak, leftAxe.transform.position, Quaternion.identity), 5);
    }
    public void Jump()
    {
        MyBody.useGravity = false;
        myNavMeshAgent.enabled = false;
        transform.DOMoveY(transform.position.y + jumpHeight, jumpOnceDuration);
    }
    public void StopAnimation()
    {
        Ani.speed = 0.1f;
        StartCoroutine(ChopDown());
    }
    private IEnumerator ChopDown()
    {
        float currentStagnantTime = Random.Range(minStagnantTime, maxStagnantTime);
        yield return new WaitForSeconds(currentStagnantTime);
        Ani.speed = 1.5f;
        transform.DOMove(Player.transform.position, chopOnceDuration);
        yield return new WaitForSeconds(chopOnceDuration);
        MyBody.useGravity = true;
        myNavMeshAgent.enabled = true;
        Destroy(leftAxe);
        anotherCollision.transform.parent.gameObject.SetActive(true);
        Ani.speed = 1;
    }
    public void GenerateRockExplosion()
    {
        myImpulse.GenerateImpulse(chopDownImpulseForce);
        Destroy(Instantiate(rockExplosionEffect, transform.position, Quaternion.identity), 5);
    }
}
