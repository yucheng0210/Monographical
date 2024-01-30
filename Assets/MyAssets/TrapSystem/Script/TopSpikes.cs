using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopSpikes : MonoBehaviour
{
    [SerializeField]
    private Rigidbody myBody;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
            myBody.useGravity = true;
    }
}
