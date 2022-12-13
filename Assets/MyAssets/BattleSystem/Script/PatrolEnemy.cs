using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RootMotion.FinalIK;

public class PatrolEnemy : MonoBehaviour, IObserver
{
    private Animator ani;
    private AnimatorStateInfo animatorStateInfo;
    private Rigidbody myBody;
    private CapsuleCollider capsuleCollider;
    private Canvas myCamera;
    private LookAtIK lookAtIK;

    [Header("移動參數")]
    [SerializeField]
    private float moveSpeed = 0.1f;

    [SerializeField]
    private float turnSpeed = 10;

    [SerializeField]
    private float beakBackForce = 15;

    [Header("AI巡邏半徑參數")]
    [SerializeField]
    private float wanderRadius = 15;

    [SerializeField]
    private float alertRadius = 12;

    [SerializeField]
    private float chaseRadius = 5;

    [SerializeField]
    private float attackRadius = 1.2f;

    [SerializeField]
    private float turnBackRadius = 15;

    [Header("狀態")]
    [SerializeField]
    private bool isOnGrounded;

    [SerializeField]
    private bool shutDown;

    [SerializeField]
    private bool warning = false;

    [SerializeField]
    private bool turnBool;

    [SerializeField]
    private bool lockMove;

    [SerializeField]
    private float distance;

    [SerializeField]
    private float wanderDistance;

    [Header("其他")]
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
    private int direction = Animator.StringToHash("Direction");
    private int forward = Animator.StringToHash("Forward");
    private int attack = Animator.StringToHash("AttackMode");
    private int isHited = Animator.StringToHash("isHited");
    private int isLosePoise = Animator.StringToHash("isLosePoise");
    private Cinemachine.CinemachineImpulseSource myImpulse;
    private CharacterState characterState,
        attackerCharacterState;
    private int playerAttackLayer;
    private float accumulateTime = 0;

    enum EnemyState
    {
        Wander,
        Alert,
        Chase,
        Attack,
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
        lookAtIK = GetComponent<LookAtIK>();
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
            accumulateTime = 0;
            if (shutDown)
                return;
            StateSwitch();
            UpdateValue();
            UpdateState();
            if (lockMove)
                movement = Vector3.zero;
        }

        /*  else
        {
            accumulateTime += Time.deltaTime;
            movement.y -= gravity * 0.5f * Mathf.Pow(accumulateTime, 2);
        }*/

    }

    private void FixedUpdate()
    {
        if (isOnGrounded)
            myBody.velocity = movement * Time.fixedDeltaTime;
    }

    private void InitialState()
    {
        characterState.CurrentHealth = characterState.MaxHealth;
        characterState.CurrentDefence = characterState.BaseDefence;
        characterState.CurrentPoise = characterState.MaxPoise;
        lookAtIK.solver.target = player.transform.GetChild(0).transform;
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
        if (gameObject.GetComponent<HitStop>().IsHitStop)
            currentState = EnemyState.BeakBack;
        else if (warning && distance <= attackRadius && angle < 60)
            currentState = EnemyState.Attack;
        else if (warning && distance <= chaseRadius)
            currentState = EnemyState.Chase;
        else if (distance <= alertRadius && angle < 120 * 0.5f && !warning)
            currentState = EnemyState.Alert;
        if (warning && distance > turnBackRadius)
            currentState = EnemyState.TurnBack;
        /*if (
            animatorStateInfo.tagHash == Animator.StringToHash("Attack")
            || animatorStateInfo.tagHash == Animator.StringToHash("isHited")
        )
        {
            ani.SetInteger(attack, 0);
            lockMove = true;
        }
        else
            lockMove = false;*/
        if (animatorStateInfo.IsName("Grounded"))
        {
            rImage.SetActive(false);
            lockMove = false;
            unityInputManager.enabled = true;
        }
        if (animatorStateInfo.tagHash == Animator.StringToHash("Attack"))
        {
            ani.SetInteger(attack, 0);
            lockMove = true;
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
                ani.SetFloat(forward, 1);
                movement = transform.forward * moveSpeed;
                break;
            case EnemyState.Alert:
                if (!warning)
                {
                    GazeSwitch(true);
                    AudioManager.Instance.BattleAudio();
                }
                ani.SetFloat(forward, 0);
                movement = Vector3.zero;
                //Look(player.transform.position);
                warning = true;
                break;
            case EnemyState.Chase:
                AnimationRealTime(false);
                GazeSwitch(false);
                Look(player.transform.position);
                if (turnBool)
                    return;
                ani.SetFloat(forward, 2);
                movement = transform.forward * moveSpeed * 3;
                break;
            case EnemyState.Attack:
                AnimationRealTime(false);
                GazeSwitch(false);
                lockMove = true;
                ani.SetFloat(forward, 0);
                ani.SetInteger(attack, 1);
                break;
            case EnemyState.TurnBack:
                if (warning)
                {
                    GazeSwitch(false);
                    AudioManager.Instance.MainAudio();
                }
                warning = false;
                if (wanderDistance < wanderRadius)
                    currentState = EnemyState.Wander;
                else
                {
                    Look(startPos);
                    if (turnBool)
                        return;
                    ani.SetFloat(forward, 2);
                    movement = transform.forward * moveSpeed * 3;
                }
                break;
            case EnemyState.BeakBack:
                AnimationRealTime(true);
                BeakBack();
                //currentState = EnemyState.Chase;
                break;
        }
    }

    IEnumerator Death()
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
        /*if (characterState.CurrentPoise <= 0)
            LosePoise();
        else*/
        myBody.AddForce(beakBackDirection * beakBackForce, ForceMode.Impulse);
        //movement = Vector3.zero;
    }

    private void LosePoise()
    {
        lockMove = true;
        ani.SetTrigger(isLosePoise);
        rImage.SetActive(true);
        characterState.CurrentPoise = characterState.MaxPoise;
    }

    private void Look(Vector3 target)
    {
        if (lockMove)
            return;
        movement = Vector3.zero;
        float targetAngle = Vector3.Angle(transform.forward, target - transform.position);
        targetRotation = Quaternion.LookRotation(
            new Vector3(target.x - transform.position.x, 0, target.z - transform.position.z)
        );
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );
        if (targetAngle > 60)
        {
            ani.SetFloat(forward, -1);
            turnBool = true;
        }
        else
            turnBool = false;
    }

    private void GazeSwitch(bool gazeSwitch)
    {
        if (gazeSwitch)
            lookAtIK.solver.SetIKPositionWeight(1);
        else
            lookAtIK.solver.SetIKPositionWeight(0);
    }

    public void ColliderSwitch(int switchCount)
    {
        if (switchCount == 1)
            collision.SetActive(true);
        else
            collision.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerAttackLayer)
        {
            characterState.TakeDamage(attackerCharacterState, characterState);
            if (shutDown)
                return;
            /* if (characterState.CurrentPoise > 0)
            {*/
            ani.SetTrigger(isHited);
            currentState = EnemyState.BeakBack;
            Vector3 hitPoint = new Vector3(
                transform.position.x,
                other.ClosestPoint(transform.position).y,
                transform.position.z
            );
            HitEffect(hitPoint);
            /* }
            else
                LosePoise();*/
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && rImage.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            Animator playerAni = player.GetComponent<Animator>();
            player.gameObject.transform.LookAt(gameObject.transform);
            unityInputManager.enabled = false;
            playerAni.SetTrigger("isExecution");
            ani.SetTrigger("isExecuted");
        }
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
        ani.SetFloat(forward, 0);
        ani.SetInteger(attack, 0);
        movement = Vector3.zero;
        shutDown = true;
    }

    public void SceneLoadingNotify(bool loadingBool)
    {
        if (loadingBool)
        {
            ani.SetFloat(forward, 0);
            ani.SetInteger(attack, 0);
            movement = Vector3.zero;
            shutDown = true;
        }
        else
            shutDown = false;
    }
}
