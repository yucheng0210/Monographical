using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public bool IsHit { get; private set; }

    private void OnTriggerStay(Collider other)
    {
        int characterLayer = LayerMask.NameToLayer("Character");
        int groundLayer = LayerMask.NameToLayer("Ground");
        IsHit = true;
        transform.parent.SetParent(other.transform);
        Destroy(transform.parent.gameObject, 0.1f);
    }
}
