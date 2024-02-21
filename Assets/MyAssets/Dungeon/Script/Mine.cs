using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : DungeonTrap
{
    [SerializeField]
    private GameObject explosionPrefab;
    protected override void Initialize()
    {
        //throw new System.NotImplementedException();
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
            Instantiate(explosionPrefab, collider.transform);
    }
}
