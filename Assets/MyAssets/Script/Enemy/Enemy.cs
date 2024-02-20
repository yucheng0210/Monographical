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
    private Cinemachine.CinemachineImpulseSource myImpulse;
    private CharacterState characterState,
        attackerCharacterState;
    private int playerAttackLayer;
    private bool shutDown;
    private float accumulateTime = 0;

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
        player = Main.Manager.GameManager.Instance.PlayerTrans.gameObject;
        myCamera = GetComponentInChildren<Canvas>();
        myCamera.worldCamera = Camera.main;
        myImpulse = GetComponent<Cinemachine.CinemachineImpulseSource>();
        characterState = GetComponent<CharacterState>();
        attackerCharacterState = player.GetComponent<CharacterState>();
        playerAttackLayer = LayerMask.NameToLayer("PlayerAttack");
        InitialState();
    }


    private void OnDisable()
    {
        Main.Manager.GameManager.Instance.RemoveObservers(this);
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
            ani.SetInteger(attack, 0);
            accumulateTime = 0;
            if (shutDown)
                return;
            if (lockMove)
                controller.Move(Vector3.zero);
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
        else
        {
            accumulateTime += Time.deltaTime;
            movement.y -= gravity * 0.5f * Mathf.Pow(accumulateTime, 2);
        }
        controller.Move(movement * Time.deltaTime);
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
                break;
        }
    }

    IEnumerator Death()
    {
        //BeakBack();
        shutDown = true;
        AudioManager.Instance.PlayerDied();
        collision.SetActive(false);
        ani.enabled = false;
        yield return new WaitForSeconds(4);
        Instantiate(dropItem, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void BeakBack()
    {
        movement = Vector3.zero;
        Vector3 beakBackDirection = (transform.position - player.transform.position).normalized;
        movement.y = Mathf.Sin(Mathf.Deg2Rad * 30) * accumulateTime;
        movement.x = beakBackDirection.x;
        movement.z = beakBackDirection.z;
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
            if (shutDown)
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
        //gameObject.GetComponent<HitStop>().StopTime();
        ani.SetTrigger(isHited);
        AudioManager.Instance.PlayerHurted();
        myImpulse.GenerateImpulse();
        Ray ray = new Ray(transform.position, transform.up * 2);
        //gameObject.GetComponent<BloodEffect>().SpurtingBlood(ray, transform.position);
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
