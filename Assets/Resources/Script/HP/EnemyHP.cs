using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : CharacterHP
{
    public DiasGames.ThirdPersonSystem.Health health;

    public override void Awake()
    {
        MaxHealth = 120;
        base.Awake();
        health = GameObject
            .FindWithTag("Player")
            .GetComponent<DiasGames.ThirdPersonSystem.Health>();
    }

    public override void Update()
    {
        base.Update();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack"))
            TakeDamage(gameObject, health.attack);
    }
}
