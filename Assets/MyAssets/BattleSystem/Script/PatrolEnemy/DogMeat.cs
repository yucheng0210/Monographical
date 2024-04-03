using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class DogMeat : PatrolEnemy
{
    [SerializeField]
    private GameObject anotherCollision;
    [SerializeField]
    private float teleportMaxAngle;
    [SerializeField]
    private float teleportCoolDown;
    private bool canTeleport;
    protected override IEnumerator InitialRegister()
    {
        yield return StartCoroutine(base.InitialRegister());
        ShutDown = true;
        EventManager.Instance.AddEventRegister(EventDefinition.eventDialogEvent, EventDialogEvent);
    }
    public override void ColliderSwitch(int switchCount)
    {
        base.ColliderSwitch(switchCount);
        if (switchCount == 1)
            anotherCollision.SetActive(true);
        else
            anotherCollision.SetActive(false);
    }
    protected override void AdditionalAttack()
    {
        meleeAttackCount = 1;
        base.AdditionalAttack();
    }
    protected override void AdditionalLongDistanceAttack()
    {
        longDistanceAttackCount = 1;
        base.AdditionalLongDistanceAttack();
    }
    public void Teleport()
    {
        canTeleport = false;
        Ani.SetBool("CanTeleport", canTeleport);
        transform.LookAt(Player.transform);
        float randomAngle = Random.Range(-teleportMaxAngle, teleportMaxAngle);
        transform.DORotate
        (new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + randomAngle, transform.eulerAngles.z), 0);
        transform.DOMove(transform.position + transform.forward * 12.5f, 0.2f);
        IsAttacking = false;
        Ani.ResetTrigger("isHited");
        Ani.SetInteger("MeleeAttackType", 0);
        Ani.SetInteger("LongDistanceAttackType", 0);
        //CurrentCoolDown = 0f;
        StartCoroutine(RecoverTeleportCoolDown());
        Debug.Log("teleport");
    }
    private IEnumerator RecoverTeleportCoolDown()
    {
        yield return new WaitForSecondsRealtime(teleportCoolDown);
        canTeleport = true;
    }
    private void EventDialogEvent(params object[] args)
    {
        if ((string)args[0] == "BOSS")
            ShutDown = false;
    }
}
