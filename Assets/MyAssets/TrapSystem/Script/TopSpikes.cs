using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopSpikes : DungeonTrap
{
    [SerializeField]
    private Rigidbody myBody;

    protected override void Initialize()
    {
        myBody.useGravity = false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            myBody.useGravity = true;
            StartCoroutine(LoseAttack());
        }
    }
    private IEnumerator LoseAttack()
    {
        yield return new WaitForSeconds(1);
        myBody.gameObject.layer = 0;

    }
}
