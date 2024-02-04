using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderFollow : MonoBehaviour
{
    [SerializeField]
    private GameObject role;

    [SerializeField]
    private float height;

    [SerializeField]
    private float xOffset;
    [SerializeField]
    private float zOffset;
    private float currentHeight;
    public float Height
    {
        get { return height; }
        set { height = value; }
    }

    private void Awake()
    {
        //currentHeight = height;
        transform.position = role.transform.position + new Vector3(xOffset, height, zOffset);
    }

    private void Update()
    {
        //currentHeight = Mathf.Lerp(currentHeight, height, Time.deltaTime * 5);
        transform.position = role.transform.position + new Vector3(xOffset, height, zOffset);
        transform.rotation = Camera.main.transform.rotation;
    }
}
