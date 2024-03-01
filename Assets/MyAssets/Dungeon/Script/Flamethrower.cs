using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamethrower : DungeonTrap
{
    [SerializeField]
    private float attackOnceDuration;
    [SerializeField]
    private float attackCoolDown;
    private Collider myCollider;
    private Transform fireTrans;
    protected override void Initialize()
    {
        fireTrans = transform.GetChild(0);
        myCollider = GetComponent<BoxCollider>();
        StartCoroutine(Attack());
    }
    private void OnTriggerStay(Collider collider)
    {
        if (collider.CompareTag("Player") && fireTrans.gameObject.activeSelf)
            EventManager.Instance.DispatchEvent(EventDefinition.eventIsHited, myCollider, TrapData);
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
