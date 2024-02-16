using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Knight : PatrolEnemy
{
    [SerializeField]
    private Vector3 jumpAttackSize;

    [SerializeField]
    private float minAniSpeed;

    [SerializeField]
    private float maxAniSpeed;

    public void JumpAttackColliderSwitch()
    {
        //攻击者位置指向目标位置的向量
        Vector3 direction = Player.transform.position - transform.position;
        //点乘结果 如果大于0表示目标在攻击者前方
        float dot = Vector3.Dot(transform.forward, direction);
        //小于0表示在攻击者后方 不在矩形攻击区域 返回false
        if (dot < 0)
            return;
        //direction在attacker正前方上的投影
        float forwardProject = Vector3.Project(direction, transform.forward).magnitude;
        //大于矩形长度表示不在矩形攻击区域 返回false
        if (forwardProject > jumpAttackSize.z)
            return;
        //direction在attacker右方的投影
        float rightProject = Vector3.Project(direction, transform.right).magnitude;
        //取绝对值与矩形宽度的一半进行比较
        if (Mathf.Abs(rightProject) <= jumpAttackSize.x * 0.5f)
        {
            EventManager.Instance.DispatchEvent(EventDefinition.eventIsHited, MyCollider, EnemyData);
        }
    }

    protected override void AdditionalAttack()
    {
        base.AdditionalAttack();
        if (!IsAttacking)
        {
            IsAttacking = true;
            int randomIndex = UnityEngine.Random.Range(1, 3);
            if (randomIndex == 1)
                Ani.SetTrigger("isMeleeAttack1");
            if (randomIndex == 2)
                Ani.SetTrigger("isMeleeAttack2");
            Ani.SetInteger("AttackMode", 0);
        }
    }

    public void ChangeAnimationSpeed(int count)
    {
        Ani.speed = count == 0 ? Mathf.Round(UnityEngine.Random.Range(minAniSpeed, maxAniSpeed) * 10) / 10.0f : 1;
    }

    protected override void UpdateState()
    {
        base.UpdateState();
        if (MyAnimatorStateInfo.IsName("JumpAttack"))
            EnemyData.PoiseAttack *= 2;
        else
            EnemyData.PoiseAttack = DataManager.Instance.CharacterList[EnemyData.CharacterID].PoiseAttack;
    }
    /* private void OnDrawGizmos()
     {
         Vector3 cornerA = transform.position + Vector3.left * jumpAttackSize.x * .5f;
         Vector3 cornerB = transform.position + Vector3.right * jumpAttackSize.x * .5f;
         Vector3 cornerC = cornerB + Vector3.forward * jumpAttackSize.z;
         Vector3 cornerD = cornerA + Vector3.forward * jumpAttackSize.z;
 
         Handles.DrawLine(cornerA, cornerB);
         Handles.DrawLine(cornerB, cornerC);
         Handles.DrawLine(cornerC, cornerD);
         Handles.DrawLine(cornerD, cornerA);
     }*/
}
