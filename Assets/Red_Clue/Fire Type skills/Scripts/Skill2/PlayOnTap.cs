using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOnTap : MonoBehaviour
{
    public ParticleSystem particlesystem;

    
    
    void Start()
    {
        
    }

    
    void Update()
    {
        Initiate();
    }

    void Initiate()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            
            _ = Instantiate(particlesystem, transform.position, Quaternion.identity);

            
        }

        
    }
}
