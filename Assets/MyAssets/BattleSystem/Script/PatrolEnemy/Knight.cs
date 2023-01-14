using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//using RootMotion.FinalIK;

public class Knight : MonoBehaviour, IObserver
{
    private Animator ani;
    private AnimatorStateInfo animatorStateInfo;
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
    private float jumpForce = 15;

    [SerializeField]
    private float jumpDashSpeed = 300;

    [SerializeField]
    private float meleeDashSpeed = 80;

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
    private float meleeAttackRadius = 2.5f;

    [SerializeField]
    private float backWalkRadius = 1;

    [SerializeField]
    private float turnBackRadius = 12;

    [Header("冷卻")]
    [SerializeField]
    private float maxCoolDown = 5.0f;

    [SerializeField]
    private float minCoolDown = 3f;

    [SerializeField]
    private float currentCoolDown = 0.0f;

    [Header("狀態")]
    [SerializeField]
    private bool isOnGrounded;

    [SerializeField]
    private bool shutDown;

    [SerializeField]
    private bool warning = false;

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

    [Header("其他")]
    [SerializeField]
    private GameObject rockBreak;

    [SerializeField]
    private GameObject hitSpark;

    [SerializeField]
    private GameObject collision;

    [SerializeField]
    private Slider healthSlider;

    [SerializeField]
    private GameObject dropItem;

    [SerializeField]
    private float capsuleOffset = 0.3f;

    [SerializeField]
    private GameObject rImage;
    private Vector3 movement,
        startPos;
    private Quaternion targetRotation;
    private GameObject player;
    private float angle;
    private DiasGames.ThirdPersonSystem.UnityInputManager unityInputManager;
    private int attack = Animator.StringToHash("AttackMode");
    private int isHited = Animator.StringToHash("isHited");
    private int isLosePoise = Animator.StringToHash("isLosePoise");
    private Cinemachine.CinemachineImpulseSource myImpulse;
    private CharacterState characterState,
        attackerCharacterState;
    private int playerAttackLayer;
    private float direction;
    private float forward;

    enum EnemyState
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

    private void Awake()
    {
        movement = Vector3.zero;
        ani = GetComponent<Animator>();
        myBody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        startPos = transform.position;
        collision.SetActive(false);
        player = GameManager.Instance.PlayerState.gameObject;
        myCamera = GetComponentInChildren<Canvas>();
        myCamera.worldCamera = Camera.main;
        myImpulse = GetComponent<Cinemachine.CinemachineImpulseSource>();
        characterState = GetComponent<CharacterState>();
        attackerCharacterState = player.GetComponent<CharacterState>();
        playerAttackLayer = LayerMask.NameToLayer("PlayerAttack");
        //lookAtIK = GetComponent<LookAtIK>();
        unityInputManager = player.GetComponent<DiasGames.ThirdPersonSystem.UnityInputManager>();
        InitialState();
    }

    private void Start()
    {
        GameManager.Instance.AddObservers(this);
        AudioManager.Instance.MainAudio();
    }

    private void OnDisable()
    {
        GameManager.Instance.RemoveObservers(this);
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

    private void FixedUpdate()
    {
        if (shutDown)
            return;
        if (isOnGrounded)
            myBody.velocity = movement * Time.fixedDeltaTime;
    }

    private void InitialState()
    {
        characterState.CurrentHealth = characterState.MaxHealth;
        characterState.CurrentDefence = characterState.BaseDefence;
        characterState.CurrentPoise = characterState.MaxPoise;
        //lookAtIK.solver.target = player.transform.GetChild(0).transform;
        currentCoolDown = UnityEngine.Random.Range(minCoolDown, maxCoolDown);
    }

    private void UpdateValue()
    {
        distance = Vector3.Distance(transform.position, player.transform.position);
        wanderDistance = Vector3.Distance(transform.position, startPos);
        angle = Vector3.Angle(transform.forward, player.transform.position - transform.position);
        healthSlider.value = (characterState.CurrentHealth / characterState.MaxHealth);
        animatorStateInfo = ani.GetCurrentAnimatorStateInfo(0);
    }

    private void UpdateState()
    {
        if (characterState.CurrentHealth <= 0)
            StartCoroutine(Death());
        else if (gameObject.GetComponent<HitStop>().IsHitStop)
            currentState = EnemyState.BeakBack;
        else if (warning)
        {
            if (distance >= turnBackRadius)
                currentState = EnemyState.TurnBack;
            else if (angle > 60)
                currentState = EnemyState.Turn;
            else if (distance <= attackRadius && currentCoolDown <= 0)
                currentState = EnemyState.Attack;
            else if ((distance <= backWalkRadius) || isBack)
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
        ani.SetFloat(
            "Direction",
            Mathf.Lerp(ani.GetFloat("Direction"), direction, Time.deltaTime * 2)
        );
        ani.SetFloat("Forward", Mathf.Lerp(ani.GetFloat("Forward"), forward, Time.deltaTime * 2));
        if (animatorStateInfo.IsName("Grounded"))
        {
            rImage.SetActive(false);
            lockMove = false;
            unityInputManager.enabled = true;
        }
        if (animatorStateInfo.tagHash == Animator.StringToHash("Attack"))
        {
            if (animatorStateInfo.normalizedTime < 0.55f)
            {
                movement = isMeleeAttack
                    ? transform.forward * meleeDashSpeed
                    : transform.forward * jumpDashSpeed;
            }
            if (animatorStateInfo.normalizedTime > 0.9f)
            {
                ani.SetInteger(attack, 0);
                currentCoolDown = UnityEngine.Random.Range(minCoolDown, maxCoolDown);
            }
        }
        else if (currentCoolDown >= 0)
            currentCoolDown -= Time.deltaTime;
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
                Look(player.transform.position);
                if (!warning)
                {
                    AudioManager.Instance.BattleAudio();
                    warning = true;
                }
                direction = 0;
                forward = 2;
                movement = transform.forward * runSpeed;
                break;
            case EnemyState.Strafe:
                AnimationRealTime(false);
                Look(player.transform.position);
                direction = 0.5f;
                forward = 0.5f;
                movement = (transform.forward * 0.5f + transform.right) * strafeSpeed;
                break;
            case EnemyState.Attack:
                AnimationRealTime(false);
                //Look(player.transform.position);
                if (isMeleeAttack)
                    ani.SetInteger(attack, 1);
                else
                    ani.SetInteger(attack, 2);
                break;
            case EnemyState.BackWalk:
                AnimationRealTime(false);
                direction = 0;
                forward = -1;
                movement = -transform.forward * walkSpeed;
                if (!isBack)
                    isBack = true;
                else if (distance >= meleeAttackRadius)
                    isBack = false;
                break;
            case EnemyState.Turn:
                Look(player.transform.position);
                Vector3 dir = player.transform.position - transform.position;
                Vector3 cross = Vector3.Cross(transform.forward, dir);
                if (cross.y >= 0)
                {
                    direction = 1;
                    forward = 0;
                }
                else
                {
                    direction = -1;
                    forward = 0;
                }
                break;
            case EnemyState.TurnBack:
                if (warning)
                {
                    //GazeSwitch(false);
                    AudioManager.Instance.MainAudio();
                }
                warning = false;
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
        ani.SetBool("isDead", true);
        myBody.velocity = Vector3.zero;
        shutDown = true;
        AudioManager.Instance.PlayerDied();
        collision.SetActive(false);
        yield return new WaitForSeconds(4);
        Instantiate(dropItem, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void BeakBack()
    {
        if (!warning)
            AudioManager.Instance.BattleAudio();
        Vector3 beakBackDirection = (transform.position - player.transform.position).normalized;
        lockMove = true;
        warning = true;
        myBody.AddForce(beakBackDirection * beakBackForce, ForceMode.Impulse);
    }

    private void LosePoise()
    {
        lockMove = true;
        ani.SetTrigger(isLosePoise);
        rImage.SetActive(true);
        characterState.CurrentPoise = characterState.MaxPoise;
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
        AudioManager.Instance.HeavyAttackAudio(0);
        Destroy(rock, 4);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerAttackLayer)
        {
            characterState.TakeDamage(attackerCharacterState, characterState);
            if (shutDown)
                return;
            /*if (characterState.CurrentPoise > 0)
            {*/
            ani.SetTrigger(isHited);
            currentState = EnemyState.BeakBack;
            Vector3 hitPoint = new Vector3(
                transform.position.x,
                other.ClosestPoint(transform.position).y,
                transform.position.z
            );
            HitEffect(hitPoint);
            //}
            /*else
                LosePoise();*/
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && rImage.activeSelf && Input.GetKeyDown(KeyCode.E))
            Execution();
    }

    private void Execution()
    {
        Animator playerAni = player.GetComponent<Animator>();
        player.gameObject.transform.LookAt(gameObject.transform);
        unityInputManager.enabled = false;
        playerAni.SetTrigger("isExecution");
        ani.SetTrigger("isExecuted");
    }

    private void AnimationRealTime(bool realTimeBool)
    {
        /*if (realTimeBool)
            ani.updateMode = AnimatorUpdateMode.UnscaledTime;
        else
            ani.updateMode = AnimatorUpdateMode.AnimatePhysics;*/
        ani.updateMode = realTimeBool
            ? AnimatorUpdateMode.UnscaledTime
            : AnimatorUpdateMode.AnimatePhysics;
    }

    private void HitEffect(Vector3 hitPoint)
    {
        //beakBackDirection = (transform.position - player.transform.position).normalized;
        //Instantiate(hitEffect, transform.position + new Vector3(0, 0.75f, 0), Quaternion.identity);
        Destroy(Instantiate(hitSpark, hitPoint, Quaternion.identity), 2);
        //        VolumeManager.Instance.DoRadialBlur(0, 0.5f, 0.12f, 0);
        gameObject.GetComponent<HitStop>().StopTime();
        AudioManager.Instance.Impact();
        AudioManager.Instance.PlayerHurted();
        myImpulse.GenerateImpulse();
        gameObject.GetComponent<BloodEffect>().SpurtingBlood(hitPoint);
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
            ani.SetInteger(attack, 0);
            movement = Vector3.zero;
            shutDown = true;
        }
        else
            shutDown = false;
    }
}
