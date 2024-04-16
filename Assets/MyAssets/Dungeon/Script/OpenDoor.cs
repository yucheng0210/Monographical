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
    [SerializeField]
    private GameObject clueMenu;
    private bool isOpen;
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
            clueMenu.SetActive(true);
    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
            clueMenu.SetActive(false);
    }
    private void OnTriggerStay(Collider collider)
    {
        if (collider.CompareTag("Player") && Input.GetKeyDown(KeyCode.E) && !isOpen)
            StartCoroutine(Open());
    }
    protected virtual IEnumerator Open()
    {
        AudioManager.Instance.OpenDoor();
        isOpen = true;
        Main.Manager.GameManager.Instance.PlayerTrans.position = lookTrans.position;
        Main.Manager.GameManager.Instance.PlayerTrans.rotation = lookTrans.rotation;
        Main.Manager.GameManager.Instance.PlayerTrans.LookAt(lookTrans);
        Camera.main.transform.LookAt(transform.forward);
        yield return null;
        Main.Manager.GameManager.Instance.PlayerAni.SetTrigger("isOpenDoor");
        leftDoorTrans.DOLocalRotate(new Vector3(0, openAngle, 0), openTime);
        rightDoorTrans.DOLocalRotate(new Vector3(0, -openAngle, 0), openTime);
        yield return new WaitForSecondsRealtime(0.1f);
        Camera.main.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
}
