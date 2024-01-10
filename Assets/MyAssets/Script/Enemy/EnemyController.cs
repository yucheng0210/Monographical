using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private PatrolEnemy enemy;

    [SerializeField]
    private float enemyCount = 1;

    private void Start()
    {
        CreateEnemy();
    }
    private void CreateEnemy()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Instantiate(enemy, transform.position, Quaternion.identity);
        }
    }
}