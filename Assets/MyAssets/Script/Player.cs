using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private Animator ani;
    public float moveSpeed;
    public float jumpForce;
    public float gravity;
    private CharacterController controller;
    private Vector3 movement = Vector3.zero;
    float h;
    float v;
    float mouse;
    public float dropSpeed;
    public static bool playerLockMove = false;
    private int direction = Animator.StringToHash("Direction");
    private int forward = Animator.StringToHash("Forward");
    private int attack = Animator.StringToHash("AttackMode");
    private int state = Animator.StringToHash("State");

    enum PlayerState
    {
        Walk,
        Run,
        Jump,
        Roll,
        Attack,
        Death
    }

    private PlayerState currentState;

    void Awake()
    {
        ani = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Time.timeScale != 0)
        {
            if (controller.isGrounded)
            {
                h = Input.GetAxis("Horizontal");
                v = Input.GetAxis("Vertical");
                mouse = Input.GetAxis("Mouse ScrollWheel");
                if (Input.GetKey(KeyCode.LeftShift) && ani.GetFloat(forward) >= 1)
                    currentState = PlayerState.Run;
                else
                    currentState = PlayerState.Walk;
                if (Input.GetKey(KeyCode.Space))
                    currentState = PlayerState.Jump;
                else if (ani.GetInteger(state) == 1)
                    ani.SetInteger(state, 0);
                if (mouse != 0 || controller.velocity.y < dropSpeed)
                    currentState = PlayerState.Roll;
                else if (ani.GetInteger(state) == 2)
                    ani.SetInteger(state, 0);
                if (Input.GetMouseButton(0))
                    currentState = PlayerState.Attack;
                else if (ani.GetInteger(attack) == 1)
                    ani.SetInteger(attack, 0);
                if (transform.position.y < -20)
                    currentState = PlayerState.Death;
                switch (currentState)
                {
                    case PlayerState.Walk:
                        ani.SetFloat(direction, h);
                        ani.SetFloat(forward, v);
                        movement = new Vector3(0.5f * h, 0, v);
                        movement = transform.TransformDirection(movement) * moveSpeed;
                        break;
                    case PlayerState.Run:
                        ani.SetFloat(direction, h);
                        ani.SetFloat(forward, 2 * v);
                        movement = new Vector3(0.5f * h, 0, 2 * v);
                        movement = transform.TransformDirection(movement) * moveSpeed;
                        break;
                    case PlayerState.Jump:
                        ani.SetInteger(state, 1);
                        movement.y = jumpForce;
                        break;
                    case PlayerState.Roll:
                        ani.SetInteger(state, 2);
                        playerLockMove = true;
                        break;
                    case PlayerState.Attack:
                        ani.SetInteger(attack, 1);
                        playerLockMove = true;
                        break;
                    case PlayerState.Death:
                        ani.SetInteger(state, 3);
                        break;
                }
            }
            else
                movement.y -= gravity * Time.deltaTime;
            if (!playerLockMove)
                controller.Move(movement * Time.deltaTime);
        }
    }
}
