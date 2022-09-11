using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourPlayer : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
    private Rigidbody myBody;

    private void Awake()
    {
        myBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        myBody.velocity = transform.forward * moveSpeed * Time.fixedDeltaTime;
    }
}
