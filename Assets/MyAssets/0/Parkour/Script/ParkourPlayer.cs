using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParkourPlayer : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
    private Rigidbody myBody;
    private Animator animator;
    private Baffle baffle;
    private CapsuleCollider capsuleCollider;
    private bool isOnGrounded;

    [SerializeField]
    private float baffleTimeScale = 0.1f;

    [SerializeField]
    private float slowTime;

    [SerializeField]
    private float accumulatedTime;

    [SerializeField]
    private float capsuleOffset;

    [SerializeField]
    private float jumpForce;

    [SerializeField]
    private float dodgeForce;

    [SerializeField]
    private GameObject dialog;
    private bool slowTimeBool;
    private Vector3 movement;
    private bool isDead;
    private bool canMove;

    private void Awake()
    {
        myBody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
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
    }

    private void SwitchStateValue()
    {
        canMove = dialog.activeSelf ? false : true;
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
