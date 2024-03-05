using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.AI;
using DG.Tweening;
public abstract class PatrolEnemy : MonoBehaviour, IObserver
{
    public Animator Ani { get; set; }
    public AnimatorStateInfo MyAnimatorStateInfo { get; set; }
    private Rigidbody myBody;
    private CapsuleCollider capsuleCollider;
    private Canvas myCamera;
    private NavMeshAgent navMeshAgent;
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
    private int playerAttackLayer;
    private float direction;
    private float forward;
    protected int meleeAttackCount;
    public GameObject Player { get; private set; }
    public Collider MyCollider { get; private set; }
    public GameObject RockBreak
    {
        get { return rockBreak; }
    }
    public float Distance
    {
        get { return distance; }
    }

    private enum EnemyState
    {
        Wander,
        Nav,
        Stand,
        Chase,
        Strafe,
        Attack,
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
            myBody.velocity = movement * Time.fixedDeltaTime;
    }
    private IEnumerator InitialRegister()
    {
        yield return null;
        Main.Manager.GameManager.Instance.EnemyList.Add(EnemyData);
        AudioManager.Instance.MainAudio();
        Player = Main.Manager.GameManager.Instance.PlayerTrans.gameObject;
        shutDown = false;
        //navMeshAgent.SetDestination(navPointList[1].position);
    }
    private IEnumerator InitialState()
    {
        yield return null;
        movement = Vector3.zero;
        Ani = GetComponent<Animator>();
        myBody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        MyCollider = GetComponent<Collider>();
        startPos = transform.position;
        myCamera = GetComponentInChildren<Canvas>();
        myCamera.worldCamera = Camera.main;
        myImpulse = GetComponent<CinemachineImpulseSource>();
        CurrentCoolDown = UnityEngine.Random.Range(minCoolDown, maxCoolDown);
        EnemyData = DataManager.Instance.CharacterList[enemyID].Clone();
        EnemyData.CurrentHealth = EnemyData.MaxHealth;
        playerAttackLayer = LayerMask.NameToLayer("PlayerAttack");
        navMeshAgent = GetComponent<NavMeshAgent>();
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
        if (distance <= meleeAttackRadius)
            isMeleeAttack = true;
        else if (isMeleeAttack)
            isMeleeAttack = false;
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
        if (MyAnimatorStateInfo.tagHash == Animator.StringToHash("Attack"))
            UpdateAttackValue();
        else if (CurrentCoolDown >= 0 && Warning)
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
        if (MyAnimatorStateInfo.normalizedTime > 0.9f && IsAttacking)
        {
            IsAttacking = false;
            Ani.ResetTrigger(isHited);
            /* Ani.ResetTrigger("isMeleeAttack1");
             Ani.ResetTrigger("isMeleeAttack2");*/
            Ani.SetInteger("MeleeAttackType", 0);
            CurrentCoolDown = UnityEngine.Random.Range(minCoolDown, maxCoolDown);
        }
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
                if (navMeshAgent.destination == null)
                {
                    navMeshAgent.SetDestination(navPointList[1].position);
                    currentNavPoint = 0;
                }
                navMeshAgent.speed = navWalkSpeed;
                if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                {
                    if (currentNavPoint == 0)
                    {
                        navMeshAgent.SetDestination(navPointList[1].position);
                        currentNavPoint = 1;
                    }
                    else
                    {
                        //transform.DOLocalRotateQuaternion(navPointList[0].rotation, 0.5f);
                        navMeshAgent.SetDestination(navPointList[0].position);
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
                if (!Warning || navMeshAgent.isStopped)
                {
                    AudioManager.Instance.BattleAudio();
                    Warning = true;
                    navMeshAgent.SetDestination(Player.transform.position);
                }
                navMeshAgent.speed = navRunSpeed;
                navMeshAgent.isStopped = false;
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
                    navMeshAgent.isStopped = false;
                    navMeshAgent.speed = navWalkSpeed;
                    navMeshAgent.SetDestination(Player.transform.position);
                }
                else
                {
                    navMeshAgent.isStopped = true;
                    movement = (transform.forward + transform.right * 0.5f) * strafeSpeed;
                }
                break;
            case EnemyState.Attack:
                AnimationRealTime(false);
                Look(Player.transform.position);
                navMeshAgent.isStopped = true;
                if (isMeleeAttack)
                {
                    Ani.SetInteger(attack, 1);
                    AdditionalAttack();
                }
                else
                    Ani.SetInteger(attack, 2);
                break;
            case EnemyState.BackWalk:
                AnimationRealTime(false);
                Look(Player.transform.position);
                navMeshAgent.isStopped = true;
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
                navMeshAgent.isStopped = true;
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
                navMeshAgent.isStopped = false;
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
                        navMeshAgent.SetDestination(navPointList[0].position);
                        currentNavPoint = 1;
                        direction = 0;
                        forward = 2;
                        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                            currentState = EnemyState.Nav;
                        break;
                    case EnemyAI.Stand:
                        navMeshAgent.SetDestination(startPos);
                        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                            currentState = EnemyState.Stand;
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

    private IEnumerator Death()
    {
        //BeakBack();
        Warning = false;
        Ani.SetBool("isDead", true);
        myBody.velocity = Vector3.zero;
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
        myBody.AddForce(beakBackDirection * beakBackForce, ForceMode.Impulse);
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

    private void Look(Vector3 target)
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerAttackLayer)
        {
            Main.Manager.GameManager.Instance.TakeDamage(Main.Manager.GameManager.Instance.PlayerData, EnemyData);
            Vector3 hitPoint = new Vector3(
                transform.position.x,
                other.ClosestPointOnBounds(transform.position).y,
                transform.position.z
            );
            HitEffect(hitPoint);
            if (shutDown || canExecution || IsAttacking)
                return;
            if (EnemyData.CurrentPoise <= 0)
            {
                //Ani.SetFloat("BeakBackMode", 2);
                EnemyData.CurrentPoise = EnemyData.MaxPoise;
                StartCoroutine(LosePoise());
                //myBody.AddForce(direction * fallDownForce, ForceMode.Impulse);
            }
            else
            {
                //Ani.SetFloat("BeakBackMode", 1);
                Ani.SetTrigger(isHited);
                currentState = EnemyState.BeakBack;
            }
        }
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
        HitEffect(transform.position + new Vector3(0, 0.75f, 0));
        EnemyCharacterState.TakeDamage(AttackerCharacterState, EnemyCharacterState);
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

    private void HitEffect(Vector3 hitPoint)
    {
        //beakBackDirection = (transform.position - Player.transform.position).normalized;
        //Instantiate(hitEffect, transform.position + new Vector3(0, 0.75f, 0), Quaternion.identity);
        Destroy(Instantiate(hitSpark, hitPoint, Quaternion.identity), 2);
        Destroy(Instantiate(hitDistortion, hitPoint, Quaternion.identity), 2);
        //VolumeManager.Instance.DoRadialBlur(0, 0.5f, 0.12f, 0);
        GetComponent<HitStop>().StopTime(0.1f, 0.2f);
        AudioManager.Instance.Impact();
        AudioManager.Instance.PlayerHurted();
        myImpulse.GenerateImpulse();
        GetComponent<BloodEffect>().SpurtingBlood(hitPoint);
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
}