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
    private bool canMove;

    [SerializeField]
    private bool upTheAir;

    [SerializeField]
    private bool slowTimeBool;

    [SerializeField]
    private bool outOfTime;

    [Header("其他")]
    [SerializeField]
    private Transform followTargetTrans;

    /* [SerializeField]
    private DialogSystem[] dialogSystem;*/
    private Baffle baffle;
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
        x = Input.GetAxis("Horizontal");
        movement = (transform.forward + new Vector3(x, 0, 0)) * moveSpeed;
        if (myBody.velocity.y < -0.5f)
            animator.SetBool("isFall", true);
        else
            animator.SetBool("isFall", false);
        upTheAir = animatorStateInfo.tagHash == Animator.StringToHash("UPTheAir") ? true : false;
    }

    private void SwitchBaffleType()
    {
        if (slowTimeBool)
        {
            accumulatedTime += Time.unscaledDeltaTime;
            baffle.RingImage.fillAmount = (slowTime - accumulatedTime) / slowTime;
            switch (baffle.baffleType)
            {
                case Baffle.BaffleType.Up:
                    if (Input.GetButtonDown("X") || Input.GetKeyDown(KeyCode.Space))
                    {
                        Time.timeScale = 1;
                        myBody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                        if (!upTheAir)
                            animator.SetTrigger("isJump");
                        else
                        {
                            animator.SetTrigger("isDoubleJump");
                            runImpulse.GenerateImpulse(new Vector3(15, 5, 0));
                        }
                        accumulatedTime = 0;
                        slowTimeBool = false;
                    }
                    break;
                case Baffle.BaffleType.Left:
                    if (Input.GetButtonDown("Y") || Input.GetKeyDown(KeyCode.Q))
                    {
                        Time.timeScale = 1;
                        animator.SetTrigger("isDodgeL");
                        myBody.AddForce(-transform.right * dodgeForce, ForceMode.Impulse);
                        accumulatedTime = 0;
                        slowTimeBool = false;
                    }
                    break;
                case Baffle.BaffleType.Right:
                    if (Input.GetButtonDown("A") || Input.GetKeyDown(KeyCode.E))
                    {
                        Time.timeScale = 1;
                        animator.SetTrigger("isDodgeR");
                        myBody.AddForce(transform.right * dodgeForce, ForceMode.Impulse);
                        accumulatedTime = 0;
                        slowTimeBool = false;
                    }
                    break;
                case Baffle.BaffleType.Down:
                    if (Input.GetButtonDown("B") || Input.GetKeyDown(KeyCode.S))
                    {
                        Time.timeScale = 1;
                        animator.SetTrigger("isRoll");
                        accumulatedTime = 0;
                        slowTimeBool = false;
                    }
                    break;
                case Baffle.BaffleType.TurnRight:
                    if (Input.GetButtonDown("A") || Input.GetKeyDown(KeyCode.E))
                    {
                        Time.timeScale = 1;
                        StartCoroutine(Turn(90));
                        accumulatedTime = 0;
                        slowTimeBool = false;
                    }
                    break;
                case Baffle.BaffleType.TurnLeft:
                    if (Input.GetButtonDown("A") || Input.GetKeyDown(KeyCode.Q))
                    {
                        Time.timeScale = 1;
                        StartCoroutine(Turn(-90));
                        accumulatedTime = 0;
                        slowTimeBool = false;
                    }
                    break;
                case Baffle.BaffleType.Climb:
                if (Input.GetButtonDown("A") || Input.GetKeyDown(KeyCode.Space))
                    {
                        Time.timeScale = 1;
                        animator.SetTrigger("isRoll");
                        accumulatedTime = 0;
                        slowTimeBool = false;
                    }
                break;
            }
            if (accumulatedTime >= slowTime)
            {
                capsuleCollider.isTrigger = false;
                Time.timeScale = 1;
                accumulatedTime = 0;
                slowTimeBool = false;
                outOfTime = true;
            }
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
        isOnGrounded = colliders.Length != 0 ? true : false;
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
        Quaternion lookPos = Quaternion.Euler(0, transform.rotation.y + direction, 0);
        while (!Mathf.Approximately(transform.rotation.y, -lookPos.y))
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                lookPos,
                Time.deltaTime * turnSpeed * 2
            );
            yield return null;
        }
        transform.rotation = lookPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Baffle"))
        {
            baffle = other.GetComponent<Baffle>();
            slowTime = baffle.SlowTime;
            baffle.RingImage.fillAmount = 1;
            capsuleCollider.isTrigger = true;
            Time.timeScale = baffleTimeScale;
            slowTimeBool = true;
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
        isDead = true;
        animator.SetTrigger("isDead");
        Time.timeScale = 0.5f;
        runImpulse.GenerateImpulse(new Vector3(15, 5, 0));
        GameManager.Instance.EndNotifyObservers();
        myBody.AddForce(beakBackForce, ForceMode.Impulse);
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
            case 1:
                StartCoroutine(LookBack());
                break;
            case 5:
                canMove = true;
                break;
        }
    }
}
