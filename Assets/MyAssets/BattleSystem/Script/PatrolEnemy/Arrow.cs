using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public bool isHit { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        isHit = true;
        int characterLayer = LayerMask.NameToLayer("Character");
        if (other.gameObject.layer == characterLayer)
            transform.parent.SetParent(other.transform);
        Destroy(gameObject);
    }
}
