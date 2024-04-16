using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Archer : PatrolEnemy
{
    // 目标点Transform
    private Vector3 targetPos;

    // 运动速度
    [Header("弓箭")]
    [SerializeField]
    private float speed = 10;

    // 最小接近距离, 以停止运动
    [SerializeField]
    private float min_distance = 0.5f;

    [SerializeField]
    private GameObject arrowPrefab;
    [SerializeField]
    private GameObject meleeAttackEffect;

    [SerializeField]
    private Transform arrowTrans;

    [SerializeField]
    private bool canDraw = true;

    [SerializeField]
    private Vector3 targetOffset;

    [SerializeField]
    private Animator bowAni;

    [SerializeField]
    private bool advancedAI;
    private float distanceToTarget;
    private Transform m_trans;
    private GameObject arrow;
    private Arrow arrowFlag;

    protected override void UpdateState()
    {
        base.UpdateState();
        //Debug.Log(CurrentCoolDown);
        if (Warning && arrowTrans.childCount < 1 && canDraw)
        {
            canDraw = false;
            Ani.SetTrigger("isDraw");
        }
        if (!Warning)
            BowDrawSwitch(0);
    }

    public void BowDrawSwitch(int switchCount)
    {
        if (switchCount == 1)
            bowAni.SetBool("isDraw", true);
        else
            bowAni.SetBool("isDraw", false);
    }

    public void BowShoot()
    {
        bowAni.SetTrigger("isShoot");
    }

    private void Shoot()
    {
        arrow.transform.parent = null;
        arrowFlag.gameObject.SetActive(true);
        targetPos = Player.transform.position + targetOffset;
        if (advancedAI)
            targetPos += Player.GetComponent<Rigidbody>().velocity / (speed / Distance);
        m_trans = arrow.transform;
        distanceToTarget = Vector3.Distance(m_trans.position, targetPos);
        IsAttacking = true;
        StartCoroutine(Parabola());
    }

    public void MeleeAttackCollider()
    {
        IsAttacking = true;
        Instantiate(meleeAttackEffect, transform);
        EventManager.Instance.DispatchEvent(EventDefinition.eventIsHited, MyCollider, EnemyData);
    }
    public void CreateArrow()
    {
        arrow = Instantiate(arrowPrefab, arrowTrans);
        arrowFlag = arrow.GetComponentInChildren<Arrow>();
        arrowFlag.EnemyData = EnemyData;
        arrowFlag.gameObject.SetActive(false);
        canDraw = true;
    }

    private IEnumerator Parabola()
    {
        float accumulateTime = 0;
        bool arriveDestination = false;
        while (!arrowFlag.IsHit)
        {
            accumulateTime += Time.deltaTime;
            /*if (Vector3.Distance(m_trans.position, targetPos) <= min_distance && !arriveDestination)
                arriveDestination = true;
            if (arriveDestination)
                 m_trans.Translate(Vector3.forward * Mathf.Min(speed * Time.deltaTime, currentDist));
            else
            {*/
            // 朝向目标, 以计算运动
            if (m_trans != null)
            {
                // 当前距离目标点
                float currentDist = Vector3.Distance(m_trans.position, targetPos);
                if (!arriveDestination && currentDist <= min_distance)
                    arriveDestination = true;
                if (!arriveDestination)
                {
                    m_trans.LookAt(targetPos);
                    // 根据距离衰减 角度
                    float angle = Mathf.Min(1, Vector3.Distance(m_trans.position, targetPos) / distanceToTarget) * 30;
                    // 旋转对应的角度（线性插值一定角度，然后每帧绕X轴旋转）
                    m_trans.rotation *= Quaternion.Euler(Mathf.Clamp(-angle, -42, 42), 0, 0);
                    // 很接近目标了, 准备结束循环
                }
                // 平移 (朝向Z轴移动)
                m_trans.Translate(Vector3.forward * speed * Time.deltaTime);
            }
            // 暂停执行, 等待下一帧再执行while
            //}
            yield return null;
        }
        // 使自己的位置, 跟[目标点]重合
        // m_trans.position = targetPos;
        // [停止]当前协程任务,参数是协程方法名
        Debug.Log(accumulateTime);
        //Destroy(m_trans.parent.gameObject);
        // 销毁脚本
        //GameObject.Destroy(this);
    }
}
