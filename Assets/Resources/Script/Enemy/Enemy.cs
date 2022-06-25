using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
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
    private float stopTime = 0.2f;

    [SerializeField]
    private float beakBackDis = 15;

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

    enum EnemyState
    {
        Wander,
        Alert,
        Chase,
        Attack,
        TurnBack
    }

    private EnemyState currentState;

    private void Awake()
    {
        movement = Vector3.zero;
        ani = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        startPos = transform.position;
        collision.SetActive(false);
        player = GameObject.FindWithTag("Player");
        myCamera = GetComponentInChildren<Canvas>();
        myCamera.worldCamera = Camera.main;
        myImpulse = GetComponent<Cinemachine.CinemachineImpulseSource>();
        characterState = GetComponent<CharacterState>();
        attackerCharacterState = player.GetComponent<CharacterState>();
        playerAttackLayer = LayerMask.NameToLayer("PlayerAttack");
        InitialState();
    }

    private void Update()
    {
        ani.SetBool("isDead", isDead);
        if (Time.timeScale == 0 || isDead)
            return;
        UpdateState();
        if (characterState.CurrentHealth <= 0)
            StartCoroutine(Death());
        if (warning && distance <= attackRadius)
            currentState = EnemyState.Attack;
        else if (warning && distance <= chaseRadius)
            currentState = EnemyState.Chase;
        else if (distance <= alertRadius && angle < 120 * 0.5f && !warning)
            currentState = EnemyState.Alert;
        if (warning && distance > turnBackRadius)
            currentState = EnemyState.TurnBack;
        if (controller.isGrounded)
            StateSwitch();
        else
            movement.y -= gravity * Time.deltaTime;
        if (lockMove)
            controller.Move(Vector3.zero);
        else
        {
            controller.Move(movement * Time.deltaTime);
            ani.SetInteger(attack, 0);
        }
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
                ani.SetFloat(forward, 2);
                Look(player.transform.position);
                movement = transform.forward * moveSpeed * 2;
                break;
            case EnemyState.Attack:
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
        }
    }

    IEnumerator Death()
    {
        isDead = true;
        AudioManager.Instance.PlayerDied();
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
            AudioManager.Instance.PlayerHurted();
            HitEffect();
            ani.SetTrigger(isHited);
            currentState = EnemyState.Chase;
        }
    }

    private void HitEffect()
    {
        myImpulse.GenerateImpulse();
        gameObject.GetComponent<HitStop>().Stop(stopTime);
        Ray ray = new Ray(transform.position, transform.up * 2);
        gameObject.GetComponent<BloodEffect>().SpurtingBlood(ray, transform.position);
        controller.Move(
            (transform.position - player.transform.position) * beakBackDis * Time.deltaTime
        );
    }
}
