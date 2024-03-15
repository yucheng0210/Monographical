using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
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
    [SerializeField]
    private GameObject magicCircle;
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
    [Header("魔法攻擊")]
    [SerializeField]
    private Transform golemPoint;
    [SerializeField]
    private int golemMaxRepeatChance;
    [SerializeField]
    private int golemRepeatChanceAttenuation;
    [SerializeField]
    private Transform fireBallPointGroup;
    [SerializeField]
    private float fireBallHeightOffset;
    [SerializeField]
    private float fireBallAngleOffset;
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
    [Header("第二階段")]
    [SerializeField]
    private bool isSecondStage;

    [Header("怒吼")]
    [SerializeField]
    private UnityEngine.Rendering.Volume mainVolumeProfile;
    [SerializeField]
    private float roarIntensity;
    [SerializeField]
    private float roarOnceDuration;
    [SerializeField]
    private CanvasGroup transitionCanvas;
    [SerializeField]
    private PlayableDirector theSecondStageTimeLine;
    private GameObject leftAxe;
    private int currentChance;
    private int chanceAttenuation;
    private int maxChance;
    private int bossStage;
    protected override void Awake()
    {
        base.Awake();
        meleeAttackCount = 2;
        longDistanceAttackCount = 3;
        if (isSecondStage)
            longDistanceAttackCount = 5;
    }
    protected override void AdditionalAttack()
    {
        if (!IsAttacking)
            ComboAttack();
        base.AdditionalAttack();
    }
    protected override void AdditionalLongDistanceAttack()
    {
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
    protected override void UpdateValue()
    {
        base.UpdateValue();
        golemPoint.localPosition = new Vector3(golemPoint.localPosition.x, Distance / 15, golemPoint.localPosition.z);
        fireBallPointGroup.LookAt(Player.transform.position + Player.transform.up * fireBallHeightOffset);
    }
    protected override void UpdateState()
    {
        base.UpdateState();
        UpdateStage();
    }
    private void UpdateStage()
    {
        if (EnemyData.CurrentHealth <= EnemyData.MaxHealth * 0.4f)
            TheFirstStage_2();
        else if (EnemyData.CurrentHealth <= EnemyData.MaxHealth * 0.7f)
            TheFirstStage_1();
        if (Input.GetKeyDown(KeyCode.E))
            EnemyData.CurrentHealth -= (int)(EnemyData.MaxHealth * 0.35f);
    }
    private void TheFirstStage_1()
    {
        if (bossStage != 0)
            return;
        bossStage++;
        if (MyAnimatorStateInfo.tagHash == Animator.StringToHash("Attack"))
            return;
        Ani.SetInteger("AttackMode", 2);
        if (!IsAttacking)
        {
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
        bossStage++;
        if (MyAnimatorStateInfo.tagHash == Animator.StringToHash("Attack"))
            return;
        Ani.SetInteger("AttackMode", 2);
        if (!IsAttacking)
        {
            IsAttacking = true;
            Ani.SetInteger("LongDistanceAttackType", 5);
            Ani.SetInteger("AttackMode", 0);
            longDistanceAttackCount++;
        }
    }
    protected override IEnumerator Death()
    {
        if (bossStage <= 2)
        {
            bossStage = 3;
            StartCoroutine(UIManager.Instance.FadeOutIn(transitionCanvas, 0, 1, false, 0.5f));
            yield return new WaitForSecondsRealtime(1.2f);
            theSecondStageTimeLine.Play();
            yield return new WaitForSecondsRealtime(0.8f);
            gameObject.SetActive(false);
        }
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
                    maxChance = golemMaxRepeatChance;
                    currentChance = maxChance;
                    chanceAttenuation = golemRepeatChanceAttenuation;
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
        List<Vector3> currentMagicCircleList = new List<Vector3>();
        for (int i = 0; i < magicCircleCount; i++)
        {
            Vector3 randomPosition = Random.insideUnitSphere * magicCircleCount;
            randomPosition += transform.position;
            randomPosition.y = transform.position.y;
            bool isValidPosition = true;
            for (int j = 0; j < currentMagicCircleList.Count; j++)
            {
                if (Vector3.Distance(currentMagicCircleList[j], randomPosition) < magicCircleMinimumSpacing)
                {
                    isValidPosition = false;
                    break;
                }
            }
            if (isValidPosition)
            {
                GameObject newEffect = Instantiate(magicCircle, randomPosition, Quaternion.identity);
                currentMagicCircleList.Add(newEffect.transform.position);
            }
            else
                i--;
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
        Destroy(effect, 5);
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
        myImpulse.GenerateImpulse();
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
