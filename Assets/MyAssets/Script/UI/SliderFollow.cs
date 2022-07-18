using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderFollow : MonoBehaviour
{
    [SerializeField]
    private GameObject role;

    [SerializeField]
    private float h;

    private void Update()
    {
        transform.position = role.transform.position + new Vector3(0, h, 0);
    }
}
