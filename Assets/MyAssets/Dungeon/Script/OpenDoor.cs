using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class OpenDoor : MonoBehaviour
{
    [SerializeField]
    private Transform leftDoorTrans;
    [SerializeField]
    private Transform rightDoorTrans;
    [SerializeField]
    private float openTime;
    [SerializeField]
    private float openAngle;
    private bool isOpen;
    private void OnTriggerStay(Collider collider)
    {
        if (collider.CompareTag("Player") && Input.GetKeyDown(KeyCode.E) && !isOpen)
        {
            isOpen = true;
            GameManager.Instance.PlayerAni.SetTrigger("isOpenDoor");
            GameManager.Instance.PlayerTrans.LookAt(transform);
            leftDoorTrans.DOLocalRotate(new Vector3(0, openAngle, 0), openTime);
            rightDoorTrans.DOLocalRotate(new Vector3(0, -openAngle, 0), openTime);
        }
    }
}
