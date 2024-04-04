using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Block"))
        {
            Vector3 hitPoint = new Vector3(
            transform.position.x,
            collider.ClosestPointOnBounds(transform.position).y,
            transform.position.z);
            Debug.Log("block");
            EventManager.Instance.DispatchEvent(EventDefinition.eventPlayerBlock, transform.root, hitPoint);
        }

    }
}
