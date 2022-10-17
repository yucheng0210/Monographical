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

    [Header("狀態")]
    [SerializeField]
    private bool isOnGrounded;

    [SerializeField]
    private bool isDead;

    [SerializeField]
    private bool canMove;

    [SerializeField]
    private bool slowTimeBool;

    [Header("其他")]
    [SerializeField]
    private Transform followTargetTrans;

    [SerializeField]
    private DialogSystem[] dialogSystem;
    private Baffle baffle;
    private Vector3 movement;
    private LookAtIK lookAtIK;

    private void Awake()
    {
        myBody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
        lookAtIK = GetComponent<LookAtIK>();
    }

    private void Start()
    {
        //LookBack(true);
        StartCoroutine(Beginning());
    }

    private void FixedUpdate()
    {
        if (isOnGrounded && canMove)
            myBody.velocity = movement * Time.fixedDeltaTime;
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
        if (dialogSystem[1].BlockContinue == false)
            canMove = dialogSystem[1].gameObject.activeSelf ? false : true;
        movement = transform.forward * moveSpeed;
        // Debug.Log(isOnGrounded);
        if (myBody.velocity.y < 0 && !isOnGrounded)
            animator.SetBool("isFall", true);
        else
            animator.SetBool("isFall", false);
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
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        Time.timeScale = 1;
                        animator.SetTrigger("isJump");
                        myBody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                        //Debug.Log(myBody.velocity);
                        accumulatedTime = 0;
                        slowTimeBool = false;
                    }
                    break;
                case Baffle.BaffleType.Left:
                    if (Input.GetKeyDown(KeyCode.A))
                    {
                        Time.timeScale = 1;
                        animator.SetTrigger("isDodgeL");
                        myBody.AddForce(-transform.right * dodgeForce, ForceMode.Impulse);
                        //Debug.Log(myBody.velocity);
                        accumulatedTime = 0;
                        slowTimeBool = false;
                    }
                    break;
                case Baffle.BaffleType.Right:
                    if (Input.GetKeyDown(KeyCode.D))
                    {
                        Time.timeScale = 1;
                        animator.SetTrigger("isDodgeR");
                        myBody.AddForce(transform.right * dodgeForce, ForceMode.Impulse);
                        //Debug.Log(myBody.velocity);
                        accumulatedTime = 0;
                        slowTimeBool = false;
                    }
                    break;
                case Baffle.BaffleType.Down:
                    if (Input.GetKeyDown(KeyCode.S))
                    {
                        Time.timeScale = 1;
                        animator.SetTrigger("isRoll");
                        //Debug.Log(myBody.velocity);
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
                isDead = true;
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
        if (colliders.Length != 0)
            isOnGrounded = true;
        else
            isOnGrounded = false;
    }

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Baffle"))
        {
            baffle = other.GetComponent<Baffle>();
            baffle.RingImage.fillAmount = 1;
            capsuleCollider.isTrigger = true;
            Time.timeScale = baffleTimeScale;
            slowTimeBool = true;
        }
        if (other.CompareTag("Dead"))
            if (isDead)
                animator.SetTrigger("isDead");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Baffle"))
            capsuleCollider.isTrigger = false;
    }
}
