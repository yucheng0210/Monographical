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
    private GameObject arrow;
    private float distanceToTarget;
    private bool move_flag = true;
    private Transform m_trans;

    private void Shoot()
    {
        arrow.transform.parent = null;
        targetPos = Player.transform.position + new Vector3(0, 0.5f, 0);
        m_trans = arrow.transform;
        distanceToTarget = Vector3.Distance(m_trans.position, targetPos);
        StartCoroutine(Parabola());
    }

    IEnumerator Parabola()
    {
        while (move_flag)
        {
            // 朝向目标, 以计算运动
            m_trans.LookAt(targetPos);
            // 根据距离衰减 角度
            float angle =
                Mathf.Min(1, Vector3.Distance(m_trans.position, targetPos) / distanceToTarget) * 45;
            // 旋转对应的角度（线性插值一定角度，然后每帧绕X轴旋转）
            m_trans.rotation =
                m_trans.rotation * Quaternion.Euler(Mathf.Clamp(-angle, -42, 42), 0, 0);
            // 当前距离目标点
            float currentDist = Vector3.Distance(m_trans.position, targetPos);
            // 很接近目标了, 准备结束循环
            if (currentDist < min_distance)
            {
                move_flag = false;
            }
            // 平移 (朝向Z轴移动)
            m_trans.Translate(Vector3.forward * Mathf.Min(speed * Time.deltaTime, currentDist));
            // 暂停执行, 等待下一帧再执行while
            yield return null;
        }
        if (move_flag == false)
        {
            // 使自己的位置, 跟[目标点]重合
            m_trans.position = targetPos;
            // [停止]当前协程任务,参数是协程方法名
            StopCoroutine(Parabola());
            Debug.Log("stop");
            // 销毁脚本
            //GameObject.Destroy(this);
        }
    }
}
