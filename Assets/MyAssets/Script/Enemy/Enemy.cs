using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IObserver
{
    private Animator ani;
    private CharacterController controller;
    private Canvas myCamera;

    [Header("移動參數")]
    [SerializeField]
    private float moveSpeed = 0.1f;

    [SerializeField]
    private float turnSpeed = 10;

    [SerializeField]
    private float gravity = 20;

    [SerializeField]
    private float beakBackForce = 15;

    [SerializeField]
    private float beakBackSpeed = 2;

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
    private bool warning = false;

    [SerializeField]
    private bool lockMove = false;

    [SerializeField]
    private float distance;

    [SerializeField]
    private float wanderDistance;

    [Header("其他")]
    [SerializeField]
    private GameObject collision;

    [SerializeField]
    private Slider healthSlider;

    [SerializeField]
    private GameObject dropItem;
    private Vector3 movement,
        startPos;
    private Quaternion targetRotation;
    private GameObject player;
    private float angle;
    private int direction = Animator.StringToHash("Direction");
    private int forward = Animator.StringToHash("Forward");
    private int attack = Animator.StringToHash("AttackMode");
    private int isHited = Animator.StringToHash("isHited");
    private bool isDead;
    private Cinemachine.CinemachineImpulseSource myImpulse;
    private CharacterState characterState,
        attackerCharacterState;
    private int playerAttackLayer;
    private bool shutDown;
    private Vector3 beakBackDirection;

    [SerializeField]
    private GameObject hitEffect;

    enum EnemyState
    {
        Wander,
        Alert,
        Chase,
        Attack,
        TurnBack,
        BeakBack
    }

    private EnemyState currentState;

    private void Awake()
    {
        movement = Vector3.zero;
        ani = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        startPos = transform.position;
        collision.SetActive(false);
        player = GameManager.Instance.PlayerState.gameObject;
        myCamera = GetComponentInChildren<Canvas>();
        myCamera.worldCamera = Camera.main;
        myImpulse = GetComponent<Cinemachine.CinemachineImpulseSource>();
        characterState = GetComponent<CharacterState>();
        attackerCharacterState = player.GetComponent<CharacterState>();
        playerAttackLayer = LayerMask.NameToLayer("PlayerAttack");
        InitialState();
    }

    private void Start()
    {
        GameManager.Instance.AddObservers(this);
    }

    private void OnDisable()
    {
        //GameManager.Instance.RemoveObservers(this);
    }

    private void Update()
    {
        if (Time.timeScale == 0)
        {
            AnimationRealTime(false);
            return;
        }
        if (controller.isGrounded)
        {
            if (lockMove)
                controller.Move(Vector3.zero);
            ani.SetInteger(attack, 0);
        }
        else
            movement.y -= gravity;
        controller.Move(movement * Time.deltaTime);
        ani.SetBool("isDead", isDead);
        if (isDead || shutDown)
            return;
        StateSwitch();
        UpdateState();
        if (characterState.CurrentHealth <= 0)
            StartCoroutine(Death());
        if (gameObject.GetComponent<HitStop>().IsHitStop)
            currentState = EnemyState.BeakBack;
        else if (warning && distance <= attackRadius)
            currentState = EnemyState.Attack;
        else if (warning && distance <= chaseRadius)
            currentState = EnemyState.Chase;
        else if (distance <= alertRadius && angle < 120 * 0.5f && !warning)
            currentState = EnemyState.Alert;
        if (warning && distance > turnBackRadius)
            currentState = EnemyState.TurnBack;
    }

    private void InitialState()
    {
        characterState.CurrentHealth = characterState.MaxHealth;
        characterState.CurrentDefence = characterState.BaseDefence;
    }

    private void UpdateState()
    {
        distance = Vector3.Distance(transform.position, player.transform.position);
        wanderDistance = Vector3.Distance(transform.position, startPos);
        angle = Vector3.Angle(transform.forward, player.transform.position - transform.position);
        healthSlider.value = (characterState.CurrentHealth / characterState.MaxHealth);
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
                ani.SetFloat(forward, 0);
                movement = Vector3.zero;
                Look(player.transform.position);
                warning = true;
                break;
            case EnemyState.Chase:
                AnimationRealTime(false);
                ani.SetFloat(forward, 2);
                Look(player.transform.position);
                movement = transform.forward * moveSpeed * 2;
                break;
            case EnemyState.Attack:
                AnimationRealTime(false);
                lockMove = true;
                movement = Vector3.zero;
                ani.SetFloat(forward, 0);
                ani.SetInteger(attack, 1);
                break;
            case EnemyState.TurnBack:
                warning = false;
                if (wanderDistance < wanderRadius)
                    currentState = EnemyState.Wander;
                else
                {
                    ani.SetFloat(forward, 2);
                    Look(startPos);
                    movement = transform.forward * moveSpeed * 2;
                }
                break;
            case EnemyState.BeakBack:
                AnimationRealTime(true);
                Look(player.transform.position);
                /* controller.Move(
                    Vector3.Lerp(
                        beakBackDirection * beakBackForce,
                        Vector3.zero,
                        Time.unscaledDeltaTime * beakBackSpeed
                    )
                );*/
                break;
        }
    }

    IEnumerator Death()
    {
        isDead = true;
        AudioManager.Instance.PlayerDied();
        collision.SetActive(false);
        yield return new WaitForSeconds(4);
        Instantiate(dropItem, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void Look(Vector3 target)
    {
        targetRotation = Quaternion.LookRotation(target - transform.position);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );
    }

    public void ColliderSwitch(int switchCount)
    {
        if (switchCount == 1)
        {
            collision.SetActive(true);
            lockMove = true;
        }
        else
        {
            collision.SetActive(false);
            lockMove = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerAttackLayer)
        {
            characterState.TakeDamage(attackerCharacterState, characterState);
            if (isDead)
                return;
            HitEffect();
        }
    }

    private void AnimationRealTime(bool realTimeBool)
    {
        if (realTimeBool)
            ani.updateMode = AnimatorUpdateMode.UnscaledTime;
        else
            ani.updateMode = AnimatorUpdateMode.AnimatePhysics;
    }

    private void HitEffect()
    {
        //beakBackDirection = (transform.position - player.transform.position).normalized;
        //Instantiate(hitEffect, transform.position + new Vector3(0, 0.75f, 0), Quaternion.identity);
        currentState = EnemyState.BeakBack;
        gameObject.GetComponent<HitStop>().StopTime();
        ani.SetTrigger(isHited);
        AudioManager.Instance.PlayerHurted();
        myImpulse.GenerateImpulse();
        Ray ray = new Ray(transform.position, transform.up * 2);
        gameObject.GetComponent<BloodEffect>().SpurtingBlood(ray, transform.position);
    }

    public void EndNotify()
    {
        Debug.Log("Game Over");
        ani.SetBool("gameIsOver", true);
        ani.SetFloat(forward, 0);
        ani.SetInteger(attack, 0);
        shutDown = true;
    }

    public void SceneLoadingNotify()
    {
        shutDown = true;
        ani.SetBool("gameIsOver", true);
        ani.SetFloat(forward, 0);
        ani.SetInteger(attack, 0);
    }
}
