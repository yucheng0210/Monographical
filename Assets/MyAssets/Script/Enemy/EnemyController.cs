using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private GameObject enemy;

    [SerializeField]
    private float enemyCount = 1;

    private void Update()
    {
        if (gameObject.transform.childCount <= enemyCount-1)
            Instantiate(enemy, transform.position, Quaternion.identity, transform);
    }
}
