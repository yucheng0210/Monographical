using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private GameObject enemy;

    private void Update()
    {
        if (gameObject.transform.childCount <= 0)
            Instantiate(enemy, transform.position, Quaternion.identity, transform);
    }
}
