using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.AI;
using DG.Tweening;
using System.Diagnostics;
public abstract class PatrolEnemy : MonoBehaviour, IObserver
{
    public Animator Ani { get; set; }
    public AnimatorStateInfo MyAnimatorStateInfo { get; set; }
    public Rigidbody MyBody { get; set; }
    private CapsuleCollider capsuleCollider;
    private Canvas myCamera;
    public NavMeshAgent myNavMeshAgent { get; set; }
    //private LookAtIK lookAtIK;

    [Header("移動參數")]
    [SerializeField]
    private float walkSpeed = 50f;

    [SerializeField]
    private float runSpeed = 200;

    [SerializeField]
    private float strafeSpeed = 30;

    /*[SerializeField]
    private float dashSpeed = 300;

    [SerializeField]
    private float meleeSpeed = 80;*/

    [SerializeField]
    private float turnSpeed = 3;

    [SerializeField]
    private float beakBackForce = 2;
    [Header("Nav移動參數")]
    [SerializeField]
    private float navWalkSpeed = 50f;

    [SerializeField]
    private float navRunSpeed = 200;
    [Header("AI巡邏半徑參數")]
    [SerializeField]
    private float visionAngle = 15;
    [SerializeField]
    private float wanderRadius = 15;
    [SerializeField]
    private List<Transform> navPointList = new List<Transform>();

    [SerializeField]
    private float chaseRadius = 12;

    [SerializeField]
    private float strafeRadius = 7;

    [SerializeField]
    private float attackRadius = 4f;

    [SerializeField]
    protected float meleeAttackRadius = 2.5f;

    [SerializeField]
    private float backWalkRadius = 1;

    [SerializeField]
    private float turnBackRadius = 12;

    [Header("冷卻")]
    [SerializeField]
    private float maxCoolDown = 5.0f;

    [SerializeField]
    private float minCoolDown = 3f;

    public float CurrentCoolDown { get; set; }

    [Header("狀態")]
    [SerializeField]
    private bool isOnGrounded;

    [SerializeField]
    private bool shutDown = true;

    public bool Warning { get; set; }

    [SerializeField]
    private bool lockMove;

    [SerializeField]
    private bool isMeleeAttack;

    [SerializeField]
    private bool isBack;

    [SerializeField]
    private float distance;

    [SerializeField]
    private float wanderDistance;
    [SerializeField]
    private int currentNavPoint;
    [SerializeField]
    private bool isBlock;

    [Header("戰鬥特效")]
    [SerializeField]
    private GameObject rockBreak;

    [SerializeField]
    private GameObject hitSpark;
    [SerializeField]
    private GameObject hitDistortion;
    [Header("AI")]
    [SerializeField]
    private EnemyAI currentAI;
    [SerializeField]
    private EnemyState currentState;
    [SerializeField]
    private bool cantStrafe;
    [Header("變速攻擊")]
    [SerializeField]
    private float minAniSpeed = 0.8f;

    [SerializeField]
    private float maxAniSpeed = 1.5f;

    [Header("其他")]
    [SerializeField]
    private int enemyID;
    [SerializeField]
    private GameObject collision;

    [SerializeField]
    private Slider healthSlider;

    [SerializeField]
    private List<GameObject> dropItemList;

    [SerializeField]
    private float capsuleOffset = 0.3f;

    [SerializeField]
    private bool canExecution;
    [Header("盾反")]
    [SerializeField]
    private AnimationCurve blockSpeedCurve;
    [SerializeField]
    private float blockTimer = 0;
    [Header("負面狀態")]
    [SerializeField]
    private List<GameObject> negativeEffectList = new List<GameObject>();
    [SerializeField]
    private List<GameObject> attributeAttackList = new List<GameObject>();
    [SerializeField]
    private bool isNegative;
    private PlayableDirector playableDirector;
    private Vector3 movement,
        startPos;
    private Quaternion targetRotation;
    private float angle;
    private int attack = Animator.StringToHash("AttackMode");
    private int isHited = Animator.StringToHash("isHited");
    private int isLosePoise = Animator.StringToHash("isLosePoise");
    protected CinemachineImpulseSource myImpulse;
    public bool IsAttacking { get; set; }
    public Character EnemyData { get; private set; }
    public CharacterState EnemyCharacterState { get; set; }
    public CharacterState AttackerCharacterState { get; set; }
    public bool ShutDown
    {
        get { return shutDown; }
        set { shutDown = value; }
    }
    private int playerAttackLayer;
    private float direction;
    private float forward;
    protected int meleeAttackCount;
    protected int longDistanceAttackCount;
    public GameObject Player { get; private set; }
    public Collider MyCollider { get; private set; }
    public float Distance
    {
        get { return distance; }
    }
    public GameObject RockBreak
    {
        get { return rockBreak; }
        set { rockBreak = value; }
    }
    public GameObject Collision
    {
        get { return collision; }
        set { collision = value; }
    }
    private enum EnemyState
    {
        Wander,
        Nav,
        Stand,
        Chase,
        Strafe,
        Attack,
        Attacking,
        BackWalk,
        Turn,
        TurnBack,
        BeakBack,
    }
    private enum EnemyAI
    {
        Wander,
        Nav,
        Stand
    }


    protected virtual void Awake()
    {
        StartCoroutine(InitialState());
    }

    protected virtual void Start()
    {
        StartCoroutine(InitialRegister());
    }

    private void Update()
    {
        // TODO: 戰鬥模式更改，會緩緩地靠近對手，轉向動畫
        if (Time.timeScale == 0)
        {
            AnimationRealTime(false);
            return;
        }
        if (shutDown)
            return;
        OnGrounded();
        if (isOnGrounded)
        {
            StateSwitch();
            UpdateValue();
            UpdateState();
            if (lockMove)
                movement = Vector3.zero;
        }
    }

    protected virtual void FixedUpdate()
    {
        if (shutDown)
            return;
        if (isOnGrounded)
            MyBody.velocity = movement * Time.fixedDeltaTime;
    }
    protected virtual IEnumerator InitialRegister()
    {
        yield return null;
        blockTimer = 1;
        Main.Manager.GameManager.Instance.EnemyList.Add(EnemyData);
        //AudioManager.Instance.MainAudio();
        Player = Main.Manager.GameManager.Instance.PlayerTrans.gameObject;
        shutDown = false;
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerBlock, EventPlayerBlock);
        EventManager.Instance.AddEventRegister(EventDefinition.eventAttributeAttack, EventAttributeAttack);
        //myNavMeshAgent.SetDestination(navPointList[1].position);
    }
    protected virtual IEnumerator InitialState()
    {
        yield return null;
        movement = Vector3.zero;
        Ani = GetComponent<Animator>();
        MyBody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        MyCollider = GetComponent<Collider>();
        startPos = transform.position;
        myCamera = GetComponentInChildren<Canvas>();
        myCamera.worldCamera = Camera.main;
        myImpulse = GetComponent<CinemachineImpulseSource>();
        RecoverAttackCoolDown();
        EnemyData = DataManager.Instance.CharacterList[enemyID].Clone();
        EnemyData.CurrentHealth = EnemyData.MaxHealth;
        playerAttackLayer = LayerMask.NameToLayer("PlayerAttack");
        myNavMeshAgent = GetComponent<NavMeshAgent>();
        switch (currentAI)
        {
            case EnemyAI.Wander:
                currentState = EnemyState.Wander;
                break;
            case EnemyAI.Nav:
                currentState = EnemyState.Nav;
                break;
            case EnemyAI.Stand:
                currentState = EnemyState.Stand;
                break;

        }
        /*EnemyCharacterState = GetComponent<CharacterState>();
        AttackerCharacterState = Player.GetComponent<CharacterState>();
        //lookAtIK = GetComponent<LookAtIK>();
        EnemyCharacterState.CurrentHealth = EnemyCharacterState.MaxHealth;
        EnemyCharacterState.CurrentDefence = EnemyCharacterState.BaseDefence;
        EnemyCharacterState.CurrentPoise = EnemyCharacterState.MaxPoise;*/
        //lookAtIK.solver.target = Player.transform.GetChild(0).transform;
    }

    protected virtual void UpdateValue()
    {
        distance = Vector3.Distance(transform.position, Player.transform.position);
        wanderDistance = Vector3.Distance(transform.position, startPos);
        angle = Vector3.Angle(transform.forward, Player.transform.position - transform.position);
        healthSlider.value = (float)EnemyData.CurrentHealth / (float)EnemyData.MaxHealth;
        MyAnimatorStateInfo = Ani.GetCurrentAnimatorStateInfo(0);
        if (blockTimer < 0.5f)
        {
            blockTimer += Time.deltaTime;
            SetBlockSpeed();
        }
        else if (isBlock)
        {
            isBlock = false;
            Ani.SetTrigger("isBlock");
            Ani.SetFloat("Speed", 1);
        }
    }

    protected virtual void UpdateState()
    {
        if (EnemyData.CurrentHealth <= 0)
            StartCoroutine(Death());
        else if (GetComponent<HitStop>().IsHitStop)
            currentState = EnemyState.BeakBack;
        else if (Warning)
        {
            if (distance >= turnBackRadius)
                currentState = EnemyState.TurnBack;
            /*else if (angle > visionAngle)
                currentState = EnemyState.Turn;*/
            else if (MyAnimatorStateInfo.tagHash == Animator.StringToHash("Attack"))
                currentState = EnemyState.Attacking;
            else if (distance <= attackRadius && CurrentCoolDown <= 0)
                currentState = EnemyState.Attack;
            else if (distance <= backWalkRadius || isBack)
                currentState = EnemyState.BackWalk;
            else if (distance <= strafeRadius)
                currentState = EnemyState.Strafe;
            else if (distance <= chaseRadius)
                currentState = EnemyState.Chase;
        }
        else if (distance <= chaseRadius && angle < visionAngle)
            currentState = EnemyState.Chase;
        Ani.SetFloat("Direction", Mathf.Lerp(Ani.GetFloat("Direction"), direction, Time.deltaTime * 2));
        Ani.SetFloat("Forward", Mathf.Lerp(Ani.GetFloat("Forward"), forward, Time.deltaTime * 2));
        //float dot = Vector3.Dot(transform.forward, Player.transform.position - transform.position);
        //小于0表示在攻击者后方 不在矩形攻击区域 返回false
        if (canExecution && Distance <= 1 && Input.GetKeyDown(KeyCode.E))
        {
            canExecution = false;
            Execution();
        }
        if (MyAnimatorStateInfo.IsName("Grounded"))
        {
            //rImage.SetActive(false);
            canExecution = false;
            lockMove = false;
        }
        if (currentState != EnemyState.Attacking && Warning)
            CurrentCoolDown -= Time.deltaTime;
    }

    protected virtual void AdditionalAttack()
    {
        if (!IsAttacking)
        {
            IsAttacking = true;
            int randomIndex = UnityEngine.Random.Range(1, meleeAttackCount + 1);
            Ani.SetInteger("MeleeAttackType", randomIndex);
            Ani.SetInteger("AttackMode", 0);
        }
    }
    protected virtual void AdditionalLongDistanceAttack()
    {
        if (!IsAttacking)
        {
            IsAttacking = true;
            int randomIndex = UnityEngine.Random.Range(1, longDistanceAttackCount + 1);
            Ani.SetInteger("LongDistanceAttackType", randomIndex);
            Ani.SetInteger("AttackMode", 0);
        }
    }
    protected virtual void UpdateAttackValue()
    {
        Ani.SetInteger(attack, 0);
        if (MyAnimatorStateInfo.normalizedTime < 0.55f)
        {
            /*movement = isMeleeAttack
                ? transform.forward * meleeSpeed
                : transform.forward * dashSpeed;*/
        }
        else
            movement = Vector3.zero;
        if (MyAnimatorStateInfo.normalizedTime > 0.8f && IsAttacking && !Ani.GetBool("isRepeat"))
        {
            /* Ani.ResetTrigger("isMeleeAttack1");
             Ani.ResetTrigger("isMeleeAttack2");*/
            IsAttacking = false;
            Ani.ResetTrigger(isHited);
            Ani.SetInteger("MeleeAttackType", 0);
            Ani.SetInteger("LongDistanceAttackType", 0);
            //Ani.SetBool("isCombo", false);
            RecoverAttackCoolDown();
        }
    }
    protected virtual void RecoverAttackCoolDown()
    {
        CurrentCoolDown = UnityEngine.Random.Range(minCoolDown, maxCoolDown);
    }
    private void OnGrounded()
    {
        float radius = capsuleCollider.radius;
        int intLayer = LayerMask.NameToLayer("Character");
        Vector3 pointBottom = transform.position + transform.up * (radius - capsuleOffset);
        Vector3 pointTop =
            transform.position + transform.up * (capsuleCollider.height - radius - capsuleOffset);
        Collider[] colliders = Physics.OverlapCapsule(
            pointBottom,
            pointTop,
            radius,
            ~(1 << intLayer)
        );
        isOnGrounded = colliders.Length != 0 ? true : false;
    }

    private void StateSwitch()
    {
        switch (currentState)
        {
            case EnemyState.Wander:
                if (wanderDistance >= wanderRadius)
                    Look(startPos);
                direction = 0;
                forward = 1;
                movement = transform.forward * walkSpeed;
                break;
            case EnemyState.Nav:
                if (myNavMeshAgent.destination == null)
                {
                    myNavMeshAgent.SetDestination(navPointList[1].position);
                    currentNavPoint = 0;
                }
                myNavMeshAgent.speed = navWalkSpeed;
                if (myNavMeshAgent.remainingDistance <= myNavMeshAgent.stoppingDistance)
                {
                    if (currentNavPoint == 0)
                    {
                        myNavMeshAgent.SetDestination(navPointList[1].position);
                        currentNavPoint = 1;
                    }
                    else
                    {
                        //transform.DOLocalRotateQuaternion(navPointList[0].rotation, 0.5f);
                        myNavMeshAgent.SetDestination(navPointList[0].position);
                        currentNavPoint = 0;
                    }
                }
                direction = 0;
                forward = 1;
                break;
            case EnemyState.Stand:
                direction = 0;
                forward = 0;
                movement = Vector3.zero;
                break;
            case EnemyState.Chase:
                AnimationRealTime(false);
                Look(Player.transform.position);
                if (!myNavMeshAgent.enabled)
                    return;
                if (!Warning || myNavMeshAgent.isStopped)
                {
                    AudioManager.Instance.BattleAudio();
                    Warning = true;
                    myNavMeshAgent.SetDestination(Player.transform.position);
                }
                myNavMeshAgent.speed = navRunSpeed;
                myNavMeshAgent.isStopped = false;
                direction = 0;
                forward = 2;
                //movement = transform.forward * runSpeed;
                break;
            case EnemyState.Strafe:
                AnimationRealTime(false);
                Look(Player.transform.position);
                direction = 0.5f;
                forward = 0.5f;
                if (cantStrafe)
                {
                    myNavMeshAgent.isStopped = false;
                    myNavMeshAgent.speed = navWalkSpeed;
                    myNavMeshAgent.SetDestination(Player.transform.position);
                }
                else if (myNavMeshAgent.enabled)
                {
                    myNavMeshAgent.isStopped = true;
                    movement = (transform.forward + transform.right * 0.5f) * strafeSpeed;
                }
                break;
            case EnemyState.Attack:
                AnimationRealTime(false);
                Look(Player.transform.position);
                if (myNavMeshAgent.enabled)
                    myNavMeshAgent.isStopped = true;
                if (distance <= meleeAttackRadius)
                    isMeleeAttack = true;
                else if (isMeleeAttack)
                    isMeleeAttack = false;
                if (isMeleeAttack)
                {
                    Ani.SetInteger(attack, 1);
                    AdditionalAttack();
                }
                else
                {
                    Ani.SetInteger(attack, 2);
                    AdditionalLongDistanceAttack();
                }
                break;
            case EnemyState.Attacking:
                AnimationRealTime(false);
                Look(Player.transform.position);
                if (myNavMeshAgent.enabled)
                    myNavMeshAgent.isStopped = true;
                UpdateAttackValue();
                break;
            case EnemyState.BackWalk:
                AnimationRealTime(false);
                Look(Player.transform.position);
                if (myNavMeshAgent.enabled)
                    myNavMeshAgent.isStopped = true;
                direction = 0;
                forward = -1;
                movement = -transform.forward * walkSpeed;
                if (!isBack)
                    isBack = true;
                else if (distance >= meleeAttackRadius)
                    isBack = false;
                break;
            case EnemyState.Turn:
                Look(Player.transform.position);
                Vector3 dir = Player.transform.position - transform.position;
                Vector3 cross = Vector3.Cross(transform.forward, dir);
                if (myNavMeshAgent.enabled)
                    myNavMeshAgent.isStopped = true;
                if (cross.y >= 0)
                {
                    direction = 2;
                    forward = 0;
                }
                else
                {
                    direction = -2;
                    forward = 0;
                }
                break;
            case EnemyState.TurnBack:
                if (Warning)
                {
                    //GazeSwitch(false);
                    AudioManager.Instance.MainAudio();
                }
                Warning = false;
                if (myNavMeshAgent.enabled)
                    myNavMeshAgent.isStopped = false;
                switch (currentAI)
                {
                    case EnemyAI.Wander:
                        if (wanderDistance < wanderRadius)
                            currentState = EnemyState.Wander;
                        else
                        {
                            Look(startPos);
                            direction = 0;
                            forward = 2;
                            movement = transform.forward * runSpeed;
                        }
                        break;
                    case EnemyAI.Nav:
                        if (myNavMeshAgent.enabled)
                        {
                            myNavMeshAgent.SetDestination(navPointList[0].position);
                            currentNavPoint = 1;
                            direction = 0;
                            forward = 2;
                            if (myNavMeshAgent.remainingDistance <= myNavMeshAgent.stoppingDistance)
                                currentState = EnemyState.Nav;
                        }
                        break;
                    case EnemyAI.Stand:
                        if (myNavMeshAgent.enabled)
                        {
                            myNavMeshAgent.SetDestination(startPos);
                            if (myNavMeshAgent.remainingDistance <= myNavMeshAgent.stoppingDistance)
                                currentState = EnemyState.Stand;
                        }
                        break;
                }
                break;
            case EnemyState.BeakBack:
                AnimationRealTime(true);
                BeakBack();
                //currentState = EnemyState.Chase;
                break;
        }
    }

    protected virtual IEnumerator Death()
    {
        //BeakBack();
        myNavMeshAgent.enabled = false;
        Warning = false;
        Ani.SetBool("isDead", true);
        MyBody.velocity = Vector3.zero;
        shutDown = true;
        AudioManager.Instance.PlayerDied();
        collision.SetActive(false);
        Main.Manager.GameManager.Instance.AddCurrentTotalKill(enemyID);
        for (int i = 0; i < QuestManager.Instance.ActiveQuestList.Count; i++)
        {
            List<(int, int)> targetEnemyList = QuestManager.Instance.ActiveQuestList[i].TargetEnemyList;
            for (int j = 0; j < targetEnemyList.Count; j++)
            {
                if (targetEnemyList[j].Item1 == enemyID)
                    QuestManager.Instance.AddQuestCurrentKill(enemyID);
            }
        }
        healthSlider.value = (float)EnemyData.CurrentHealth / (float)EnemyData.MaxHealth;
        yield return new WaitForSeconds(4);
        int randomIndex = UnityEngine.Random.Range(0, dropItemList.Count);
        Instantiate(dropItemList[randomIndex], transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void BeakBack()
    {
        if (!Warning)
            AudioManager.Instance.BattleAudio();
        Vector3 beakBackDirection = (transform.position - Player.transform.position).normalized;
        lockMove = true;
        Warning = true;
        MyBody.AddForce(beakBackDirection * beakBackForce, ForceMode.Impulse);
    }

    private IEnumerator LosePoise()
    {
        lockMove = true;
        Ani.SetTrigger(isLosePoise);
        yield return null;
        //rImage.SetActive(true);
        canExecution = true;
        EnemyData.CurrentPoise = EnemyData.MaxPoise;
        collision.SetActive(false);
    }

    protected void Look(Vector3 target)
    {
        if (lockMove)
            return;
        movement = Vector3.zero;
        targetRotation = Quaternion.LookRotation(
            new Vector3(target.x - transform.position.x, 0, target.z - transform.position.z)
        );
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );
    }

    /*private void GazeSwitch(bool gazeSwitch)
    {
        if (gazeSwitch)
            lookAtIK.solver.SetIKPositionWeight(1);
        else
            lookAtIK.solver.SetIKPositionWeight(0);
    }*/

    public virtual void ColliderSwitch(int switchCount)
    {
        if (switchCount == 1)
            collision.SetActive(true);
        else
            collision.SetActive(false);
    }

    public void HeavyAttackEffect()
    {
        Vector3 start = collision.transform.GetChild(0).transform.position;
        GameObject rock = Instantiate(rockBreak, start, Quaternion.identity);
        rock.transform.forward = transform.forward;
        AudioManager.Instance.HeavyAttackAudio(0);
        Destroy(rock, 4);
    }
    private void SetBlockSpeed()
    {
        isBlock = true;
        Ani.SetFloat("Speed", blockSpeedCurve.Evaluate(blockTimer));
        collision.SetActive(false);
    }
    private void EventPlayerBlock(params object[] args)
    {
        if ((Transform)args[0] != transform)
            return;
        Vector3 hitPoint = (Vector3)args[1];
        blockTimer = 0;
        Destroy(Instantiate(hitSpark, hitPoint, Quaternion.identity), 2);
        Destroy(Instantiate(hitDistortion, hitPoint, Quaternion.identity), 2);
        AudioManager.Instance.Impact();
        myImpulse.GenerateImpulse();
        IsAttacking = false;
        Ani.ResetTrigger(isHited);
        Ani.SetInteger("MeleeAttackType", 0);
        Ani.SetInteger("LongDistanceAttackType", 0);
        //Ani.SetBool("isCombo", false);
        RecoverAttackCoolDown();
        collision.SetActive(false);
        Main.Manager.GameManager.Instance.PlayerData.Momentum += 50;
        EnemyData.Momentum -= 40;
        EventManager.Instance.DispatchEvent(EventDefinition.eventPlayerInvincible, true, true);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!(other.gameObject.layer == playerAttackLayer || other.gameObject.layer == LayerMask.NameToLayer("Attack")))
            return;
        if (other.gameObject.layer == playerAttackLayer)
            Main.Manager.GameManager.Instance.TakeDamage(Main.Manager.GameManager.Instance.PlayerData, EnemyData);
        if (other.gameObject.layer == LayerMask.NameToLayer("Attack"))
            Main.Manager.GameManager.Instance.TakeDamage(other.GetComponent<AttackID>().AttackData, EnemyData);
        Vector3 hitPoint = new Vector3(
               transform.position.x,
               other.ClosestPointOnBounds(transform.position).y,
               transform.position.z
           );
        HitEffect(hitPoint, other.tag);
        if (canExecution || IsAttacking || shutDown)
            return;
        /*if (EnemyData.CurrentPoise <= 0)
        {
            //Ani.SetFloat("BeakBackMode", 2);
            EnemyData.CurrentPoise = EnemyData.MaxPoise;
            StartCoroutine(LosePoise());
            //MyBody.AddForce(direction * fallDownForce, ForceMode.Impulse);
        }
        else
        {*/
        //Ani.SetFloat("BeakBackMode", 1);
        Ani.SetTrigger(isHited);
        currentState = EnemyState.BeakBack;
        // }
    }

    private void Execution()
    {
        Animator playerAni = Player.GetComponent<Animator>();
        //playerAni.SetInteger("AttackMode", 0);
        lockMove = true;
        transform.position = Player.transform.position + Player.transform.forward;
        transform.LookAt(Player.transform);
        Player.transform.LookAt(transform);
        playerAni.SetTrigger("isRestart");
        playerAni.SetTrigger("isExecution");
        Ani.SetTrigger("isExecuted");
        playableDirector = GameObject.Find("ExecutionTimeline").GetComponent<PlayableDirector>();
        playableDirector.Play();
        //Time.timeScale = 0.5f;
    }
    public void ExecutionAttack()
    {
        /* HitEffect(transform.position + new Vector3(0, 0.75f, 0));
         EnemyCharacterState.TakeDamage(AttackerCharacterState, EnemyCharacterState);*/
    }
    public void ChangeAnimationSpeed(int count)
    {
        if (count == 1)
            Ani.speed *= Mathf.Round(UnityEngine.Random.Range(minAniSpeed, maxAniSpeed) * 10) / 10.0f;
        else
        {
            Ani.speed = 1;
            ColliderSwitch(1);
        }
    }
    public void AdjustAnimationSpeed(float count)
    {
        Ani.speed = count;
    }
    private void AnimationRealTime(bool realTimeBool)
    {
        /*if (realTimeBool)
            Ani.updateMode = AnimatorUpdateMode.UnscaledTime;
        else
            Ani.updateMode = AnimatorUpdateMode.AnimatePhysics;*/
        Ani.updateMode = realTimeBool
            ? AnimatorUpdateMode.UnscaledTime
            : AnimatorUpdateMode.AnimatePhysics;
    }

    private void HitEffect(Vector3 hitPoint, string otherTag)
    {
        //beakBackDirection = (transform.position - Player.transform.position).normalized;
        //Instantiate(hitEffect, transform.position + new Vector3(0, 0.75f, 0), Quaternion.identity);
        CheckNegativeActive();
        if (!isNegative)
        {
            switch (otherTag)
            {
                case "Fire":
                    negativeEffectList[0].SetActive(true);
                    StartCoroutine(Main.Manager.GameManager.Instance.OnFire(negativeEffectList[0], EnemyData));
                    break;
                case "Thunder":
                    negativeEffectList[1].SetActive(true);
                    break;
                case "Ice":
                    negativeEffectList[2].SetActive(true);
                    Ani.speed = 0;
                    shutDown = true;
                    StartCoroutine(Thaw());
                    break;
            }
        }
        CheckNegativeActive();
        Destroy(Instantiate(hitSpark, hitPoint, Quaternion.identity), 2);
        Destroy(Instantiate(hitDistortion, hitPoint, Quaternion.identity), 2);
        //VolumeManager.Instance.DoRadialBlur(0, 0.5f, 0.12f, 0);
        GetComponent<HitStop>().StopTime(0.1f, 0.1f);
        AudioManager.Instance.Impact();
        //AudioManager.Instance.PlayerHurted();
        myImpulse.GenerateImpulse();
        GetComponent<BloodEffect>().SpurtingBlood(hitPoint);
    }
    private IEnumerator Thaw()
    {
        yield return new WaitForSeconds(6f);
        negativeEffectList[2].SetActive(false);
        Ani.speed = 1;
        shutDown = false;
    }
    private void CheckNegativeActive()
    {
        for (int i = 0; i < negativeEffectList.Count; i++)
        {
            if (negativeEffectList[i].activeSelf)
            {
                isNegative = true;
                break;
            }
            isNegative = false;
        }
    }
    private int GetNegativeActive()
    {
        int id = 0;
        for (int i = 0; i < negativeEffectList.Count; i++)
        {
            if (negativeEffectList[i].activeSelf)
                id = i;
        }
        return id;
    }
    public void EndNotify()
    {
        direction = 0;
        forward = 0;
        movement = Vector3.zero;
        shutDown = true;
    }

    public void SceneLoadingNotify(bool loadingBool)
    {
        if (loadingBool)
        {
            direction = 0;
            forward = 0;
            Ani.SetInteger(attack, 0);
            movement = Vector3.zero;
            shutDown = true;
        }
        else
            shutDown = false;
    }
    private void EventAttributeAttack(params object[] args)
    {
        int id = GetNegativeActive();
        UnityEngine.Debug.Log(id);
        if (isNegative && negativeEffectList[id].activeSelf)
        {
            UnityEngine.Debug.Log((int)args[0]);
            if ((int)args[0] == 2)
            {
                StartCoroutine(Lighting());
                attributeAttackList[id].SetActive(true);
            }
        }
    }
    private IEnumerator Lighting()
    {
        yield return new WaitForSeconds(2.95f);
        Main.Manager.GameManager.Instance.TakeDamage(DataManager.Instance.CharacterList[1002], EnemyData);
        HitEffect(transform.position, "");
        int id = GetNegativeActive();
        attributeAttackList[id].SetActive(false);
    }
}