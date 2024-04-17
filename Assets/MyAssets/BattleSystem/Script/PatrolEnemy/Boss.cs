using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class Boss : PatrolEnemy
{
    [SerializeField]
    private GameObject anotherCollision;
    /* [SerializeField]
     private GameObject elbowCollision;*/
    [SerializeField]
    private Transform groundTrans;
    [SerializeField]
    private float groundOffsetY;
    [Header("戰鬥額外特效")]
    [SerializeField]
    private GameObject rockExplosionEffect;
    [SerializeField]
    private GameObject sprintEffect;
    [SerializeField]
    private GameObject magicCircle;
    [SerializeField]
    private GameObject zawarudoEffect;
    [SerializeField]
    private GameObject fireTornadoEffect;
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
    [Header("後跳")]
    [SerializeField]
    private float backJumpHeight;
    [SerializeField]
    private float backJumpOnceDuration;
    [SerializeField]
    private float backJumpDistance;
    [Header("衝刺攻擊")]
    [SerializeField]
    private float sprintOffsetZ;
    [SerializeField]
    private float sprintOnceDuration;
    [Header("魔法攻擊")]
    [SerializeField]
    private Transform fireSlashPoint;
    [SerializeField]
    private int fireSlashMaxRepeatChance;
    [SerializeField]
    private int fireSlashRepeatChanceAttenuation;
    [SerializeField]
    private Transform fireBallPointGroup;
    [SerializeField]
    private float fireBallHeightOffset;
    [SerializeField]
    private int fireBallMaxRepeatChance;
    [SerializeField]
    private int fireBallRepeatChanceAttenuation;
    [SerializeField]
    private int fireBallAdditionalChance;
    [SerializeField]
    private float magicCircleCount;
    [SerializeField]
    private float magicCircleRadius;
    [SerializeField]
    private float magicCircleMinimumSpacing;
    private List<GameObject> currentMagicCircleList = new List<GameObject>();

    [SerializeField]
    private float fireTornadoCount;
    [SerializeField]
    private float fireTornadoMaxSpeed;
    [SerializeField]
    private float fireTornadoRadius;
    [SerializeField]
    private float teleportMaxAngle;
    [SerializeField]
    private float teleportCoolDown;
    private bool canTeleport;
    [SerializeField]
    private Transform zawarudoTrans;
    private List<Transform> fireTornadoList = new List<Transform>();
    [Header("階段")]
    [SerializeField]
    private int bossStage;
    [SerializeField]
    private bool isSecondStage;

    [SerializeField]
    private CanvasGroup transitionCanvas;
    [SerializeField]
    private PlayableDirector theFirstStageTimeLine;
    [SerializeField]
    private PlayableDirector theSecondStageTimeLine;
    [Header("怒吼")]
    [SerializeField]
    private UnityEngine.Rendering.Volume mainVolumeProfile;
    [SerializeField]
    private float roarIntensity;
    [SerializeField]
    private float roarOnceDuration;
    private GameObject leftAxe;
    private int currentChance;
    private int chanceAttenuation;
    private int maxChance;
    private bool isStartAttack;
    protected override void Awake()
    {
        base.Awake();
        meleeAttackCount = 3;
        longDistanceAttackCount = 3;
        if (isSecondStage)
        {
            meleeAttackCount = 4;
            longDistanceAttackCount = 5;
            canTeleport = true;
        }
    }
    protected override void AdditionalAttack()
    {
        UpdateStage();
        if (!IsAttacking)
            ComboAttack();
        base.AdditionalAttack();
    }
    protected override void AdditionalLongDistanceAttack()
    {
        UpdateStage();
        if (!IsAttacking)
        {
            ComboAttack();
            int randomIndex = Random.Range(1, 100);
            if (fireBallAdditionalChance >= randomIndex)
            {
                IsAttacking = true;
                Ani.SetInteger("LongDistanceAttackType", 3);
                Ani.SetInteger("AttackMode", 0);
            }
        }
        base.AdditionalLongDistanceAttack();

    }
    protected override void UpdateState()
    {
        if (theSecondStageTimeLine.state == PlayState.Playing)
            return;
        base.UpdateState();
    }
    protected override void UpdateValue()
    {
        base.UpdateValue();
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
            EnemyData.CurrentHealth -= (int)(EnemyData.MaxHealth * 0.35f);
        if (Input.GetKeyDown(KeyCode.RightControl))
        {
            theFirstStageTimeLine.time = theFirstStageTimeLine.duration;
            theSecondStageTimeLine.time = theSecondStageTimeLine.duration;
        }
        if (theFirstStageTimeLine.state == PlayState.Playing || theSecondStageTimeLine.state == PlayState.Playing)
        {
            if (!Main.Manager.GameManager.Instance.PlayerAni.GetBool("Standby"))
                EventManager.Instance.DispatchEvent(EventDefinition.eventPlayerCantMove, 2);
        }
        else if (Main.Manager.GameManager.Instance.PlayerAni.GetBool("Standby"))
            EventManager.Instance.DispatchEvent(EventDefinition.eventPlayerCantMove, 0);
        if (!isStartAttack)
        {
            CurrentCoolDown = 0;
            if (theFirstStageTimeLine.state != PlayState.Playing && !IsAttacking && !isSecondStage)
            {
                isStartAttack = true;
                IsAttacking = true;
                Ani.SetInteger("LongDistanceAttackType", 1);
                Ani.SetInteger("AttackMode", 2);
            }

        }
        fireBallPointGroup.LookAt(Player.transform.position + Player.transform.up * fireBallHeightOffset);
        fireSlashPoint.LookAt(Player.transform.position + Player.transform.up * fireBallHeightOffset);
        if (isSecondStage)
            Ani.SetBool("CanTeleport", canTeleport);
    }
    private void UpdateStage()
    {
        if (!isSecondStage)
        {
            if (EnemyData.CurrentHealth <= EnemyData.MaxHealth * 0.4f)
                TheFirstStage_2();
            else if (EnemyData.CurrentHealth <= EnemyData.MaxHealth * 0.7f)
                TheFirstStage_1();
        }
        else if (theSecondStageTimeLine.state != PlayState.Playing)
        {
            /*if (EnemyData.CurrentHealth <= EnemyData.MaxHealth * 0.4f)
                TheSecondStage_2();*/
            TheSecondStage_1();
            UpdateFireTornadoPos();
        }
    }
    private void TheFirstStage_1()
    {
        if (bossStage != 0)
            return;
        if (MyAnimatorStateInfo.tagHash == Animator.StringToHash("Attack"))
            return;
        if (!IsAttacking && Ani.GetInteger("AttackMode") == 2)
        {
            bossStage++;
            CurrentCoolDown = 0;
            Ani.SetInteger("AttackMode", 2);
            IsAttacking = true;
            Ani.SetInteger("LongDistanceAttackType", 4);
            Ani.SetInteger("AttackMode", 0);
            longDistanceAttackCount++;
        }
    }
    private void TheFirstStage_2()
    {
        if (bossStage != 1)
            return;
        if (MyAnimatorStateInfo.tagHash == Animator.StringToHash("Attack"))
            return;
        if (!IsAttacking)
        {
            bossStage++;
            CurrentCoolDown = 0;
            Ani.SetInteger("AttackMode", 2);
            IsAttacking = true;
            Ani.SetInteger("LongDistanceAttackType", 5);
            Ani.SetInteger("AttackMode", 0);
            longDistanceAttackCount++;
        }
    }
    private void TheSecondStage_1()
    {
        if (bossStage != 0)
            return;
        bossStage++;
        if (MyAnimatorStateInfo.tagHash == Animator.StringToHash("Attack"))
            return;
        for (int i = 0; i < fireTornadoCount; i++)
        {
            Vector3 randomPos = Random.insideUnitSphere * fireTornadoRadius;
            randomPos += transform.position + transform.forward * 5;
            randomPos.y = transform.position.y;
            Transform fireTornado = Instantiate(fireTornadoEffect, randomPos, Quaternion.identity).transform;
            fireTornadoList.Add(fireTornado);
        }
        AudioManager.Instance.BattleAudio();
    }
    private void TheSecondStage_2()
    {
        if (bossStage != 0)
            return;
        if (MyAnimatorStateInfo.tagHash == Animator.StringToHash("Attack"))
            return;
        if (!IsAttacking)
        {
            CurrentCoolDown = 0;
            bossStage++;
            IsAttacking = true;
            Ani.SetBool("isZawarudo", true);
        }
    }
    public void CreateZawarudo()
    {
        Instantiate(zawarudoEffect, zawarudoTrans);
        Ani.SetBool("isZawarudo", false);
    }
    private void UpdateFireTornadoPos()
    {
        for (int i = 0; i < fireTornadoList.Count; i++)
        {
            Vector3 direction = Random.insideUnitSphere.normalized;
            direction.y = 0;
            float currentSpeed = Random.Range(0, fireTornadoMaxSpeed);
            fireTornadoList[i].position += direction * currentSpeed;
        }
    }
    protected override IEnumerator Death()
    {
        if (!isSecondStage)
        {
            bossStage = 3;
            AudioManager.Instance.BGMSource.Stop();
            StartCoroutine(UIManager.Instance.FadeOutIn(transitionCanvas, 0, 1, false, 0.5f));
            yield return new WaitForSecondsRealtime(1.2f);
            theSecondStageTimeLine.Play();
            yield return new WaitForSecondsRealtime(0.8f);
            gameObject.SetActive(false);
        }
        else
            StartCoroutine(base.Death());
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
    /*public void ElbowsColliderSwitch(int id)
    {
        if (id == 1)
            elbowCollision.SetActive(true);
        else
            elbowCollision.SetActive(false);

    }*/
    public void SetRighttAxe(int count)
    {
        if (count == 1 && !Ani.GetBool("isRepeat"))
            Collision.transform.parent.gameObject.SetActive(true);
        else
            Collision.transform.parent.gameObject.SetActive(false);
    }
    public void RepeatAttack()
    {
        int randomIndex = Random.Range(1, 100);
        if (!Ani.GetBool("isRepeat"))
        {
            switch (Ani.GetInteger("LongDistanceAttackType"))
            {
                case 3:
                    maxChance = fireBallMaxRepeatChance;
                    currentChance = maxChance;
                    chanceAttenuation = fireBallRepeatChanceAttenuation;
                    break;
                case 4:
                    maxChance = fireSlashMaxRepeatChance;
                    currentChance = maxChance;
                    chanceAttenuation = fireSlashRepeatChanceAttenuation;
                    break;
            }
        }
        if (currentChance >= randomIndex)
        {
            Ani.SetBool("isRepeat", true);
            currentChance -= chanceAttenuation;
        }
        else
        {
            Ani.SetBool("isRepeat", false);
            currentChance = maxChance;
        }
    }
    private void ComboAttack()
    {
        int randomIndex = Random.Range(1, 100);
        if (randomIndex >= 50)
            Ani.SetBool("isCombo", true);
        else
            Ani.SetBool("isCombo", false);
    }
    public void Move(float duration)
    {
        transform.DOMove(Player.transform.position, duration);
    }
    public void MagicCircleAttack()
    {
        currentMagicCircleList.Clear();
        for (int i = 0; i < magicCircleCount; i++)
        {
            Vector3 randomPosition = Random.insideUnitSphere * magicCircleRadius;
            randomPosition += transform.position;
            randomPosition.y = transform.position.y;
            bool isValidPosition = true;
            for (int j = 0; j < currentMagicCircleList.Count; j++)
            {
                if (Vector3.Distance(currentMagicCircleList[j].transform.position, randomPosition) < magicCircleMinimumSpacing)
                {
                    isValidPosition = false;
                    break;
                }
            }
            if (isValidPosition)
            {
                GameObject newEffect = Instantiate(magicCircle, randomPosition, Quaternion.identity);
                currentMagicCircleList.Add(newEffect);
            }
            else
                i--;
        }
        StartCoroutine(WaitForMagicCircleAttack());
    }
    private IEnumerator WaitForMagicCircleAttack()
    {
        yield return new WaitForSeconds(6);
        for (int i = 0; i < currentMagicCircleList.Count; i++)
        {
            BoxCollider myCollider = currentMagicCircleList[i].AddComponent<BoxCollider>();
            myCollider.center = Vector3.zero;
            myCollider.size = new Vector3(5, 1, 5);
            myCollider.isTrigger = true;
        }
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
        effect.GetComponent<BoxCollider>().enabled = false;
        Destroy(effect, 5);
    }
    public void AxThrowing()
    {
        leftAxe = Instantiate(anotherCollision.transform.parent.gameObject,
        anotherCollision.transform.parent.parent);
        leftAxe.transform.SetParent(null);
        anotherCollision.transform.parent.gameObject.SetActive(false);
        leftAxe.GetComponentInChildren<BoxCollider>().gameObject.SetActive(true);
        leftAxe.GetComponentInChildren<BoxCollider>().gameObject.layer = LayerMask.NameToLayer("Attack");
        leftAxe.transform.DORotate(leftAxe.transform.eulerAngles + new Vector3(0, rotateDirection.y, rotateDirection.z), 0);
        leftAxe.transform.DORotate(leftAxe.transform.eulerAngles + rotateDirection, axThrowingOnceDuration
        , RotateMode.FastBeyond360);
        leftAxe.transform.DOMove(Player.transform.position + transform.up * heightOffset, axThrowingOnceDuration);
        StartCoroutine(GenerateRockBreak());
    }
    private IEnumerator GenerateRockBreak()
    {
        yield return new WaitForSeconds(axThrowingOnceDuration);
        myImpulse.GenerateImpulse();
        Vector3 pos = new Vector3(leftAxe.transform.position.x, groundTrans.position.y + groundOffsetY, leftAxe.transform.position.z);
        Destroy(leftAxe.GetComponentInChildren<BoxCollider>(), 0.1f);
        Destroy(Instantiate(RockBreak, pos, Quaternion.identity), 5);
    }
    public void Jump()
    {
        MyBody.useGravity = false;
        myNavMeshAgent.enabled = false;
        transform.DOMoveY(transform.position.y + jumpHeight, jumpOnceDuration);
    }
    public void BackJump(int count)
    {
        if (count == 1)
        {
            MyBody.useGravity = false;
            myNavMeshAgent.enabled = false;
            StartCoroutine(CalculateBackJump());
        }
        else
        {
            MyBody.useGravity = true;
            myNavMeshAgent.enabled = true;
        }
    }
    private IEnumerator CalculateBackJump()
    {
        transform.DOMove
        (transform.position + -transform.forward * backJumpDistance / 2 + transform.up * backJumpDistance, backJumpOnceDuration / 2);
        yield return new WaitForSecondsRealtime(backJumpOnceDuration / 2);
        transform.DOMove
        (transform.position + -transform.forward * backJumpDistance / 2 + -transform.up * backJumpDistance, backJumpOnceDuration / 2);

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
        Vector3 pos =
        new Vector3(Collision.transform.position.x, groundTrans.position.y + groundOffsetY, Collision.transform.position.z);
        GameObject effect = Instantiate(rockExplosionEffect, pos, Quaternion.identity);
        Destroy(effect.GetComponentInChildren<BoxCollider>(), 0.5f);
        Destroy(effect, 5);
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

}
