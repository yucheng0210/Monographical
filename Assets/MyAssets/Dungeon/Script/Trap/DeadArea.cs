using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadArea : DungeonTrap
{
    [SerializeField]
    private float flyForce;
    [SerializeField]
    private float mineForce;
    [SerializeField]
    private GameObject explosionPrefab;
    public enum DeadMode
    {
        Normal,
        Fly,
        Mine
    }
    public DeadMode currentMode;
    protected override void Initialize()
    {

    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            switch (currentMode)
            {
                case DeadMode.Fly:
                    Instantiate(explosionPrefab, transform);
                    collider.GetComponent<Rigidbody>().AddForce(collider.transform.up * flyForce, ForceMode.Impulse);
                    break;
                case DeadMode.Mine:
                    GameObject explosion = Instantiate(explosionPrefab, collider.transform.position, Quaternion.identity);
                    explosion.transform.localScale *= 5;
                    collider.GetComponent<Rigidbody>().AddExplosionForce(mineForce, collider.transform.position, 10, 3);
                    Debug.Log("mine");
                    break;
            }
        }
    }
}
