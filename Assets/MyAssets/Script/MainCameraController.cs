using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCameraController : MonoBehaviour
{
    private Camera playercamera;
    public float offset;
    public float moveSpeed;
    float mouseX;

    void Start()
    {
        playercamera = GetComponentInChildren<Camera>();
    }

    void LateUpdate()
    {
        mouseX = Input.GetAxis("Mouse X") * moveSpeed;
        playercamera.transform.LookAt(transform.position);
        //float mouseY=Input.GetAxis("Mouse Y")*moveSpeed;
        //playercamera.transform.localRotation=camera.transform.localRotation*Quaternion.Euler(-mouseY,0,0);
        transform.localRotation = transform.localRotation * Quaternion.Euler(0, mouseX, 0);
    }
}
