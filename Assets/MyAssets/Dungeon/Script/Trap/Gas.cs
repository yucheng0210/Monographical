using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gas : DungeonTrap
{
    private bool isHit;
    protected override void Initialize()
    {
        //StartCoroutine(IsHited());
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player") && !isHit)
            StartCoroutine(IsHited());
    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
            isHit = false;
    }
    private IEnumerator IsHited()
    {
        isHit = true;
        while (isHit && Main.Manager.GameManager.Instance.PlayerData.CurrentHealth > 0)
        {
            Main.Manager.GameManager.Instance.TakeDamage(DataManager.Instance.CharacterList[TrapData.CharacterID]
              , Main.Manager.GameManager.Instance.PlayerData);
            yield return new WaitForSeconds(1);
        }
    }
}
