using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public bool IsHit { get; private set; }
    public Character EnemyData { get; set; }
    private void Start()
    {
        Destroy(transform.parent.gameObject, 5f);
    }
    private void OnTriggerStay(Collider other)
    {
        IsHit = true;
        transform.parent.SetParent(other.transform);
        Destroy(transform.parent.gameObject, 0.1f);
    }
}
