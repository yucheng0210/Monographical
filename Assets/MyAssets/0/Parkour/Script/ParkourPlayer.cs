using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RootMotion.FinalIK;

public class ParkourPlayer : MonoBehaviour
{
    private Rigidbody myBody;
    private Animator animator;
    private CapsuleCollider capsuleCollider;

    [Header("參數")]
    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float baffleTimeScale = 0.1f;

    [SerializeField]
    private float slowTime;

    [SerializeField]
    private float turnSpeed;

    [SerializeField]
    private float accumulatedTime;

    [SerializeField]
    private float capsuleOffset;

    [SerializeField]
    private float jumpForce;

    [SerializeField]
    private float dodgeForce;

    [SerializeField]
    private Vector3 beakBackForce;

    [Header("狀態")]
    [SerializeField]
    private bool isOnGrounded;

    [SerializeField]
    private bool isDead;

    [SerializeField]
    private bool isClimb;

    [SerializeField]
    private bool canMove;

    [SerializeField]
    private bool upTheAir;

    [SerializeField]
    private bool slowTimeBool;

    [SerializeField]
    private bool outOfTime;

    [SerializeField]
    private float playerDirection;

    [SerializeField]
    private bool runTest;

    [Header("其他")]
    [SerializeField]
    private Transform followTargetTrans;

    /* [SerializeField]
    private DialogSystem[] dialogSystem;*/
    private Baffle baffle;
    private SliderFollow sliderFollow;
    private Vector3 movement;
    private LookAtIK lookAtIK;
    private Cinemachine.CinemachineImpulseSource runImpulse;
    private AnimatorStateInfo animatorStateInfo;
    private float x;

    private void Awake()
    {
        myBody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
        lookAtIK = GetComponent<LookAtIK>();
        runImpulse = GetComponent<Cinemachine.CinemachineImpulseSource>();
    }

    private void Start()
    {
        //LookBack(true);
        //StartCoroutine(Beginning());
        EventManager.Instance.AddEventRegister(EventDefinition.eventMainLine, HandleMainLine);
    }

    private void FixedUpdate()
    {
        animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (isOnGrounded && !isDead)
        {
            runImpulse.GenerateImpulse();
            if (canMove)
                myBody.velocity = movement * Time.fixedDeltaTime;
        }
    }

    private void Update()
    {
        OnGrounded();
        SwitchStateValue();
        SwitchBaffleType();
        // StartCoroutine(LookBack(true));
    }

    private void SwitchStateValue()
    {
        movement = transform.forward * moveSpeed;
        if (myBody.velocity.y < -2f)
            animator.SetBool("isFall", true);
        else
            animator.SetBool("isFall", false);
        upTheAir = animatorStateInfo.tagHash == Animator.StringToHash("UPTheAir") ? true : false;
    }

    private void SwitchBaffleType()
    {
        if (!slowTimeBool)
            return;
        accumulatedTime += Time.unscaledDeltaTime;
        for (int i = 0; i < baffle.RingImage.Count; i++)
        {
            baffle.RingImage[i].fillAmount = (slowTime - accumulatedTime) / slowTime;
        }
        switch (baffle.baffleType)
        {
            case Baffle.BaffleType.Up:
                if (Input.GetButtonDown("X") || Input.GetKeyDown(KeyCode.Space) || runTest)
                {
                    SuccessfullyDodge();
                    if (!upTheAir)
                    {
                        myBody.velocity = new Vector3(0, 0, myBody.velocity.z);
                        animator.SetTrigger("isJump");
                        myBody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                    }
                    else
                    {
                        myBody.velocity = new Vector3(myBody.velocity.x, 0, myBody.velocity.z);
                        animator.SetTrigger("isDoubleJump");
                        myBody.AddForce(transform.up * jumpForce * 0.5f, ForceMode.Impulse);
                        runImpulse.GenerateImpulse(new Vector3(25, 15, 0));
                    }
                }
                break;
            case Baffle.BaffleType.Left:
                if (Input.GetButtonDown("Y") || Input.GetKeyDown(KeyCode.Q) || runTest)
                {
                    SuccessfullyDodge();
                    animator.SetTrigger("isDodgeL");
                    myBody.AddForce(-transform.right * dodgeForce, ForceMode.Impulse);
                }
                break;
            case Baffle.BaffleType.Right:
                if (Input.GetButtonDown("A") || Input.GetKeyDown(KeyCode.E) || runTest)
                {
                    SuccessfullyDodge();
                    animator.SetTrigger("isDodgeR");
                    myBody.AddForce(transform.right * dodgeForce, ForceMode.Impulse);
                }
                break;
            case Baffle.BaffleType.Down:
                if (Input.GetButtonDown("B") || Input.GetKeyDown(KeyCode.S) || runTest)
                {
                    SuccessfullyDodge();
                    animator.SetTrigger("isRoll");
                }
                break;
            case Baffle.BaffleType.TurnRight:
                if (Input.GetButtonDown("A") || Input.GetKeyDown(KeyCode.E) || runTest)
                {
                    SuccessfullyDodge();
                    StartCoroutine(Turn(90));
                }
                break;
            case Baffle.BaffleType.TurnLeft:
                if (Input.GetButtonDown("A") || Input.GetKeyDown(KeyCode.Q) || runTest)
                {
                    SuccessfullyDodge();
                    StartCoroutine(Turn(-90));
                }
                break;
            case Baffle.BaffleType.Climb:
                Time.timeScale = 1;
                canMove = false;
                slowTimeBool = false;
                myBody.velocity = Vector3.zero;
                capsuleCollider.isTrigger = false;
                StartCoroutine(ClimbingLadder());
                break;
            case Baffle.BaffleType.Select:
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    SuccessfullyDodge();
                    StartCoroutine(Turn(-90));
                }
                if (Input.GetKeyDown(KeyCode.E) || runTest)
                {
                    SuccessfullyDodge();
                    StartCoroutine(Turn(90));
                }
                break;
        }
        OutOfTime();
    }

    private void SuccessfullyDodge()
    {
        Time.timeScale = 1;
        accumulatedTime = 0;
        slowTimeBool = false;
        baffle.ClueCanvas.SetActive(false);
    }

    private void OutOfTime()
    {
        if (accumulatedTime >= slowTime)
        {
            capsuleCollider.isTrigger = false;
            Time.timeScale = 1;
            accumulatedTime = 0;
            slowTimeBool = false;
            outOfTime = true;
            baffle.ClueCanvas.SetActive(false);
        }
    }

    private void OnGrounded()
    {
        float radius = capsuleCollider.radius;
        int intLayer = LayerMask.NameToLayer("Ground");
        Vector3 pointBottom = transform.position + transform.up * (radius - capsuleOffset);
        Vector3 pointTop =
            transform.position + transform.up * (capsuleCollider.height - radius - capsuleOffset);
        Collider[] colliders = Physics.OverlapCapsule(
            pointBottom,
            pointTop,
            radius,
            (1 << intLayer)
        );
        isOnGrounded = colliders.Length != 0;
    }

    private IEnumerator ClimbingLadder()
    {
        float animationCount = 0;
        float animationProgress = 0;
        transform.position = new Vector3(
            baffle.transform.position.x,
            transform.position.y,
            transform.position.z
        );
        isClimb = true;
        animator.SetBool("isClimb", isClimb);
        while (animator.GetBool("isClimb"))
        {
            accumulatedTime += Time.unscaledDeltaTime;
            for (int i = 0; i < baffle.RingImage.Count; i++)
            {
                baffle.RingImage[i].fillAmount = (slowTime - accumulatedTime) / slowTime;
            }
            animationProgress = animatorStateInfo.normalizedTime - animationCount;
            if (animationProgress >= 1 && animatorStateInfo.IsName("Climb"))
            {
                animator.speed = 0;
                if (Input.GetButtonDown("A") || Input.GetKeyDown(KeyCode.Space) || runTest)
                {
                    animator.speed = 1;
                    animationCount++;
                    sliderFollow.Height += 0.8f;
                }
            }
            if (animator.speed == 1)
                myBody.velocity = transform.up;
            else
            {
                myBody.useGravity = false;
                myBody.velocity = Vector3.zero;
            }
            animator.SetBool("isClimb", isClimb);
            OutOfTime();
            yield return null;
        }
        baffle.ClueCanvas.SetActive(false);
        while (animatorStateInfo.IsTag("Climb"))
        {
            animator.speed = 1;
            myBody.velocity = (transform.up + transform.forward) * 2f;
            yield return null;
        }
        canMove = true;
        myBody.useGravity = true;
    }

    #region "沒使用事件管理器的開頭"
    /*
    private IEnumerator Beginning()
    {
        dialogSystem[1].BlockContinue = true;
        while (dialogSystem[0].gameObject.activeSelf)
            yield return null;
        Quaternion lookPos = Quaternion.Euler(0, -180, 0);
        lookAtIK.solver.SetIKPositionWeight(1);
        while (!Mathf.Approximately(followTargetTrans.rotation.y, -lookPos.y))
        {
            followTargetTrans.rotation = Quaternion.Slerp(
                followTargetTrans.rotation,
                lookPos,
                Time.deltaTime * turnSpeed
            );
            yield return null;
        }
        yield return new WaitForSecondsRealtime(1);
        lookPos = Quaternion.Euler(0, 0, 0);
        lookAtIK.solver.SetIKPositionWeight(0);
        while ((followTargetTrans.rotation.y - lookPos.y) > 0.01f)
        {
            followTargetTrans.rotation = Quaternion.Slerp(
                followTargetTrans.rotation,
                lookPos,
                Time.deltaTime * turnSpeed
            );
            yield return null;
        }
        followTargetTrans.rotation = lookPos;
        dialogSystem[1].gameObject.SetActive(true);
        dialogSystem[1].BlockContinue = false;
    }
    */
    #endregion
    private IEnumerator LookBack()
    {
        Quaternion lookPos = Quaternion.Euler(0, -180, 0);
        lookAtIK.solver.SetIKPositionWeight(1);
        while (!Mathf.Approximately(followTargetTrans.rotation.y, -lookPos.y))
        {
            followTargetTrans.rotation = Quaternion.Slerp(
                followTargetTrans.rotation,
                lookPos,
                Time.deltaTime * turnSpeed
            );
            yield return null;
        }
        lookPos = Quaternion.Euler(0, 0, 0);
        lookAtIK.solver.SetIKPositionWeight(0);
        while ((followTargetTrans.rotation.y - lookPos.y) > 0.1f)
        {
            followTargetTrans.rotation = Quaternion.Slerp(
                followTargetTrans.rotation,
                lookPos,
                Time.deltaTime * turnSpeed
            );
            yield return null;
        }
        followTargetTrans.rotation = lookPos;
        EventManager.Instance.DispatchEvent(EventDefinition.eventNextMainLine, this);
    }

    private IEnumerator Turn(float direction)
    {
        playerDirection += direction;
        if (playerDirection > 0)
            playerDirection = 0;
        if (playerDirection < -180)
            playerDirection = -90;
        Quaternion lookPos = Quaternion.Euler(0, playerDirection, 0);
        while (Mathf.Abs(transform.rotation.y - lookPos.y) > 0.01f)
        {
            transform.Rotate(0, direction * Time.deltaTime * turnSpeed, 0);
            yield return null;
        }
        transform.rotation = lookPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead)
            return;
        if (other.CompareTag("Baffle"))
        {
            baffle = other.GetComponent<Baffle>();
            slowTime = baffle.SlowTime;
            for (int i = 0; i < baffle.RingImage.Count; i++)
            {
                baffle.RingImage[i].fillAmount = 1;
            }
            capsuleCollider.isTrigger = true;
            Time.timeScale = baffleTimeScale;
            slowTimeBool = true;
            sliderFollow = baffle.ClueCanvas.GetComponentInChildren<SliderFollow>();
        }
        if (other.CompareTag("BaffleEnd"))
        {
            isClimb = false;
            accumulatedTime = 0;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Dead") && !isDead && outOfTime)
            StartCoroutine(Death());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Baffle"))
            capsuleCollider.isTrigger = false;
    }

    private IEnumerator Death()
    {
        EventManager.Instance.DispatchEvent(EventDefinition.eventGameOver);
        isDead = true;
        animator.SetTrigger("isDead");
        Time.timeScale = 0.5f;
        runImpulse.GenerateImpulse(new Vector3(25, 15, 0));
        GameManager.Instance.EndNotifyObservers();
        myBody.velocity = Vector3.zero;
        beakBackForce = transform.TransformDirection(beakBackForce);
        myBody.AddForce(beakBackForce, ForceMode.Impulse);
        gameObject
            .GetComponent<BloodEffect>()
            .SpurtingBlood(transform.position + new Vector3(0, 1.75f, 0));
        yield return new WaitForSecondsRealtime(2);
        Time.timeScale = 1;
        yield return new WaitForSecondsRealtime(5);
        SaveLoadManager.Instance.AutoSave();
        StartCoroutine(SceneController.Instance.Transition("StartMenu"));
    }

    private void HandleMainLine(params object[] args)
    {
        switch ((int)args[0])
        {
            case 0:

                break;
            case 1:
                StartCoroutine(LookBack());
                break;
            case 5:
                canMove = true;
                break;
        }
    }
}
