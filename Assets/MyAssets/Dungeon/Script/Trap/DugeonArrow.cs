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
    [SerializeField]
    private bool isAutoShoot;
    [SerializeField]
    private float autoShootStartTime;
    [SerializeField]
    private float autoShootCoolDown;
    protected override void Initialize()
    {
        if (isAutoShoot)
            InvokeRepeating(nameof(Shoot), autoShootStartTime, autoShootCoolDown);
    }
    private void Shoot()
    {
        GameObject arrow = Instantiate(arrowPrefab, launcherTrans);
        arrow.GetComponent<Rigidbody>().AddForce(arrowForce * launcherTrans.forward, ForceMode.Impulse);
        Destroy(arrow, 2);
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player") && !isAutoShoot)
            Shoot();
    }
}
