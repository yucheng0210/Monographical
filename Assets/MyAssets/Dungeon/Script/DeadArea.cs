using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadArea : DungeonTrap
{
    [SerializeField]
    private float flyForce;
    [SerializeField]
    private GameObject explosionPrefab;
    public enum DeadMode
    {
        Normal,
        Fly
    }
    public DeadMode currentMode;
    protected override void Initialize()
    {

    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player") && currentMode == DeadMode.Fly)
        {
            Instantiate(explosionPrefab, transform);
            collider.GetComponent<Rigidbody>().AddForce(collider.transform.up * flyForce, ForceMode.Impulse);
        }
    }
}
