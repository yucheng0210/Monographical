using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogChange : MonoBehaviour
{
    [SerializeField]
    private DialogSystem dialogSystem;
    [SerializeField]
    private string changeName;
    private bool isChange = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isChange)
        {
            isChange = true;
            dialogSystem.gameObject.SetActive(true);
            dialogSystem.dialogName = changeName;
        }
    }
}
