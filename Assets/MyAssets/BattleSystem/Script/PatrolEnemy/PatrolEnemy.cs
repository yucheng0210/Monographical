using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public abstract class PatrolEnemy : MonoBehaviour, IObserver
{
    public Animator Ani { get; set; }
    public AnimatorStateInfo MyAnimatorStateInfo { get; set; }
    private Rigidbody myBody;
    private CapsuleCollider capsuleCollider;
    private Canvas myCamera;

    //private LookAtIK lookAtIK;

    [Header("移動參數")]
    [SerializeField]
    private float walkSpeed = 50f;

    [SerializeField]
    private float runSpeed = 200;

    [SerializeField]
    private float strafeSpeed = 30;

    [SerializeField]
    private float dashSpeed = 300;

    [SerializeField]
    private float meleeSpeed = 80;

    [SerializeField]
    private float turnSpeed = 3;

    [SerializeField]
    private float beakBackForce = 2;

    [Header("AI巡邏半徑參數")]
    [SerializeField]
    private float wanderRadius = 15;

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
    private bool shutDown;

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

    [Header("戰鬥特效")]
    [SerializeField]
    private GameObject rockBreak;

    [SerializeField]
    private GameObject hitSpark;
    [SerializeField]
    private GameObject hitDistortion;
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
    private CinemachineImpulseSource myImpulse;
    public bool IsAttacking { get; set; }
    public Character EnemyData { get; private set; }
    public CharacterState EnemyCharacterState { get; set; }
    public CharacterState AttackerCharacterState { get; set; }
    private int playerAttackLayer;
    private float direction;
    private float forward;
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
        Chase,
        Strafe,
        Attack,
        BackWalk,
        Turn,
        TurnBack,
        BeakBack,
    }

    [SerializeField]
    private EnemyState currentState;

    protected virtual void Awake()
    {
        InitialState();
    }

    protected virtual void Start()
    {
        InitialRegister();
    }

    private void Update()
    {
        // TODO: 戰鬥模式更改，會緩緩地靠近對手，轉向動畫
        if (Time.timeScale == 0)
        {
            AnimationRealTime(false);
            return;
        }
        OnGrounded();
        if (isOnGrounded)
        {
            if (shutDown)
                return;
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
    private void InitialRegister()
    {
        GameManager.Instance.AddObservers(this);
        GameManager.Instance.EnemyList.Add(EnemyData);
        AudioManager.Instance.MainAudio();
        Player = GameManager.Instance.PlayerTrans.gameObject;

    }
    private void InitialState()
    {
        movement = Vector3.zero;
        Ani = GetComponent<Animator>();
        myBody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        MyCollider = GetComponent<Collider>();
        startPos = transform.position;
        myCamera = GetComponentInChildren<Canvas>();
        myCamera.worldCamera = Camera.main;
        myImpulse = GetComponent<Cinemachine.CinemachineImpulseSource>();
        CurrentCoolDown = UnityEngine.Random.Range(minCoolDown, maxCoolDown);
        EnemyData = DataManager.Instance.CharacterList[enemyID].Clone();
        EnemyData.CurrentHealth = EnemyData.MaxHealth;
        /*EnemyCharacterState = GetComponent<CharacterState>();
        AttackerCharacterState = Player.GetComponent<CharacterState>();
        playerAttackLayer = LayerMask.NameToLayer("PlayerAttack");
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
        healthSlider.value = (EnemyData.CurrentHealth / EnemyData.MaxHealth);
        MyAnimatorStateInfo = Ani.GetCurrentAnimatorStateInfo(0);
    }

    protected virtual void UpdateState()
    {
        if (EnemyData.CurrentHealth <= 0)
            StartCoroutine(Death());
        else if (gameObject.GetComponent<HitStop>().IsHitStop)
            currentState = EnemyState.BeakBack;
        else if (Warning)
        {
            if (distance >= turnBackRadius)
                currentState = EnemyState.TurnBack;
            else if (angle > 60)
                currentState = EnemyState.Turn;
            else if (distance <= attackRadius && CurrentCoolDown <= 0)
                currentState = EnemyState.Attack;
            else if (distance <= backWalkRadius || isBack)
                currentState = EnemyState.BackWalk;
            else if (distance <= strafeRadius)
                currentState = EnemyState.Strafe;
            else if (distance <= chaseRadius)
                currentState = EnemyState.Chase;
        }
        else if (distance <= chaseRadius && angle < 60)
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

    protected virtual void AdditionalAttack() { }

    protected virtual void UpdateAttackValue()
    {
        Ani.SetInteger(attack, 0);
        if (MyAnimatorStateInfo.normalizedTime < 0.55f)
        {
            movement = isMeleeAttack
                ? transform.forward * meleeSpeed
                : transform.forward * dashSpeed;
        }
        else
            movement = Vector3.zero;
        if (MyAnimatorStateInfo.normalizedTime > 0.9f && IsAttacking)
        {
            IsAttacking = false;
            Ani.ResetTrigger(isHited);
            Ani.ResetTrigger("isMeleeAttack1");
            Ani.ResetTrigger("isMeleeAttack2");
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
            case EnemyState.Chase:
                AnimationRealTime(false);
                Look(Player.transform.position);
                if (!Warning)
                {
                    AudioManager.Instance.BattleAudio();
                    Warning = true;
                }
                direction = 0;
                forward = 2;
                movement = transform.forward * runSpeed;
                break;
            case EnemyState.Strafe:
                AnimationRealTime(false);
                Look(Player.transform.position);
                direction = 0.5f;
                forward = 0.5f;
                movement = (transform.forward + transform.right * 0.5f) * strafeSpeed;
                break;
            case EnemyState.Attack:
                AnimationRealTime(false);
                Look(Player.transform.position);
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
        EnemyCharacterState.CurrentPoise = EnemyCharacterState.MaxPoise;
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

    public void ColliderSwitch(int switchCount)
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
            EnemyCharacterState.TakeDamage(AttackerCharacterState, EnemyCharacterState);
            Vector3 hitPoint = new Vector3(
                transform.position.x,
                other.ClosestPointOnBounds(transform.position).y,
                transform.position.z
            );
            HitEffect(hitPoint);
            if (shutDown || canExecution || IsAttacking)
                return;
            if (EnemyCharacterState.CurrentPoise <= 0)
            {
                //Ani.SetFloat("BeakBackMode", 2);
                EnemyCharacterState.CurrentPoise = EnemyCharacterState.MaxPoise;
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
        GetComponent<HitStop>().StopTime();
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
