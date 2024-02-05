using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    public float Speed;
    public GameObject MuzzlePrefab;
    public GameObject BlazePrefab;
    
    void Start()
    {
        if (MuzzlePrefab != null)
        {
            GameObject muzzle = Instantiate(MuzzlePrefab, transform.position, Quaternion.identity);
            muzzle.transform.forward = gameObject.transform.forward;
            ParticleSystem muz = muzzle.GetComponent<ParticleSystem>();

            if (muz != null)
            {
                Destroy(muzzle, muz.main.duration);
            }
            else
            {
                ParticleSystem psChild = muzzle.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(muzzle, psChild.main.duration);
            }

            
        }
    }

    
    void Update()
    {
        if(Speed != 0)
        {
            StartCoroutine(Waitformuzzle());
        }
        else
        {
            Debug.Log("No Speed");
        }
    }

    IEnumerator Waitformuzzle()
    {
        yield return new WaitForSeconds(2.01f);
        transform.position -= transform.right * (Speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        Speed = 0;

        _ = collision.contacts[0];

        if (BlazePrefab != null)
        {
            var hitVFX = Instantiate(BlazePrefab);
            var psHit = hitVFX.GetComponent<ParticleSystem>();
            if (psHit != null)
                Destroy(hitVFX, psHit.main.duration);

            else
            {
                var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitVFX, psChild.main.duration);
            }
        }

        Destroy(gameObject);
    }
}
