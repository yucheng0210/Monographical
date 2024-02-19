using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class DugeonArrow : DungeonTrap
{
    [SerializeField]
    private GameObject arrowPrefab;
    [SerializeField]
    private Transform launcherTrans;
    [SerializeField]
    private float arrowForce;
    protected override void Initialize()
    {

    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            GameObject arrow = Instantiate(arrowPrefab, launcherTrans);
            arrow.GetComponent<Rigidbody>().AddForce(arrowForce * launcherTrans.forward, ForceMode.Impulse);
            Destroy(arrow, 5);
        }

    }
}
