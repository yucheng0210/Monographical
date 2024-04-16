using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionImpact : MonoBehaviour
{
    [SerializeField]
    private float radius = 5.0F;
    [SerializeField]
    private float power = 10.0F;

    private void Start()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(power, explosionPos, radius, 3.0F);
        }
    }
}
