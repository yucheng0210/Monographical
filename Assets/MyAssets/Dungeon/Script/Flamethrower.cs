using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamethrower : DungeonTrap
{
    [SerializeField]
    private float attackOnceDuration;
    [SerializeField]
    private float attackCoolDown;
    [SerializeField]
    private Vector3 attackSize;
    private Collider myCollider;
    private Transform fireTrans;
    protected override void Initialize()
    {
        fireTrans = transform.GetChild(0);
        myCollider = GetComponent<BoxCollider>();
        StartCoroutine(Attack());
    }
    private void FixedUpdate()
    {
        if (fireTrans.gameObject.activeSelf)
        {
            //攻击者位置指向目标位置的向量
            Vector3 direction = Main.Manager.GameManager.Instance.PlayerTrans.position - fireTrans.position;
            //点乘结果 如果大于0表示目标在攻击者前方
            float dot = Vector3.Dot(fireTrans.forward, direction);
            //小于0表示在攻击者后方 不在矩形攻击区域 返回false
            if (dot < 0)
                return;
            //direction在attacker正前方上的投影
            float forwardProject = Vector3.Project(direction, fireTrans.forward).magnitude;
            //大于矩形长度表示不在矩形攻击区域 返回false
            if (forwardProject > attackSize.z)
                return;
            //direction在attacker右方的投影
            float rightProject = Vector3.Project(direction, fireTrans.right).magnitude;
            //取绝对值与矩形宽度的一半进行比较
            if (Mathf.Abs(rightProject) <= attackSize.x * 0.5f)
            {
                EventManager.Instance.DispatchEvent(EventDefinition.eventIsHited, myCollider, TrapData);
            }
        }
    }
    /*private void OnDrawGizmosSelected()
    {
        // 獲取攻擊者位置
        Vector3 attackerPosition = fireTrans.position;

        // 計算攻擊區域的四個角
        Vector3 frontTopLeft = attackerPosition + fireTrans.forward * attackSize.z * 0.5f + fireTrans.right * attackSize.x * 0.5f;
        Vector3 frontTopRight = attackerPosition + fireTrans.forward * attackSize.z * 0.5f - fireTrans.right * attackSize.x * 0.5f;
        Vector3 frontBottomLeft = attackerPosition - fireTrans.forward * attackSize.z * 0.5f + fireTrans.right * attackSize.x * 0.5f;
        Vector3 frontBottomRight = attackerPosition - fireTrans.forward * attackSize.z * 0.5f - fireTrans.right * attackSize.x * 0.5f;

        // 畫出攻擊區域的邊框
        Debug.DrawLine(frontTopLeft, frontTopRight, Color.red);
        Debug.DrawLine(frontTopRight, frontBottomRight, Color.red);
        Debug.DrawLine(frontBottomRight, frontBottomLeft, Color.red);
        Debug.DrawLine(frontBottomLeft, frontTopLeft, Color.red);
    }*/
    private IEnumerator Attack()
    {
        fireTrans.gameObject.SetActive(true);
        yield return new WaitForSeconds(attackOnceDuration);
        fireTrans.gameObject.SetActive(false);
        yield return new WaitForSeconds(attackCoolDown);
        StartCoroutine(Attack());
    }

}
