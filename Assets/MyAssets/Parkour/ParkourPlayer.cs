using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private bool slowTimeBool;
    private Vector3 movement;

    private void Awake()
    {
        myBody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (isOnGrounded)
            myBody.velocity = movement * Time.fixedDeltaTime;
    }

    private void Update()
    {
        OnGrounded();
        movement = transform.forward * moveSpeed;
       // Debug.Log(isOnGrounded);  
        if (!isOnGrounded)
            animator.SetBool("isFall", true);
        else
            animator.SetBool("isFall", false);
        if (slowTimeBool)
        {
            accumulatedTime += Time.unscaledDeltaTime;
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
            }
            if (accumulatedTime >= slowTime)
            {
                Time.timeScale = 1;
                accumulatedTime = 0;
                slowTimeBool = false;
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

            if (baffle.baffleType == Baffle.BaffleType.Up)
            {
                Time.timeScale = baffleTimeScale;
                slowTimeBool = true;
            }
        }
    }
}
