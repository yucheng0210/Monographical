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
    [SerializeField]
    private Transform lookTrans;
    private bool isOpen;
    private void OnTriggerStay(Collider collider)
    {
        if (collider.CompareTag("Player") && Input.GetKeyDown(KeyCode.E) && !isOpen)
            StartCoroutine(Open());
    }
    private IEnumerator Open()
    {
        isOpen = true;
        Main.Manager.GameManager.Instance.PlayerTrans.LookAt(lookTrans);
        Main.Manager.GameManager.Instance.PlayerTrans.position = lookTrans.position;
        yield return null;
        Main.Manager.GameManager.Instance.PlayerAni.SetTrigger("isOpenDoor");
        leftDoorTrans.DOLocalRotate(new Vector3(0, openAngle, 0), openTime);
        rightDoorTrans.DOLocalRotate(new Vector3(0, -openAngle, 0), openTime);
    }
}
