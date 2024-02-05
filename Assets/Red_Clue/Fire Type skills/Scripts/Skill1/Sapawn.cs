using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sapawn : MonoBehaviour
{
    public GameObject spawnPoint;
    public GameObject vfx;

    private GameObject spawnEffect;

    void Start()
    {
        spawnEffect = vfx;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
           Sawn();
        }
    }
    void Sawn()
    {
        if (spawnPoint != null)
        {
            vfx = Instantiate(spawnEffect, spawnPoint.transform.position, Quaternion.identity);
        }

        else
        {
            Debug.Log("Spawn Point");
        }
    }
}
