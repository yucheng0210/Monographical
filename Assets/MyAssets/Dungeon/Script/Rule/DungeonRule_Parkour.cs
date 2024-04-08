using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRule_Parkour : MonoBehaviour
{
    [SerializeField]
    private GameObject explosionSource;
    private bool isStart;
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player") && !isStart)
        {
            isStart = true;
            StartCoroutine(StartParkour());
        }
    }
    private IEnumerator StartParkour()
    {
        yield return new WaitForSecondsRealtime(5);
        explosionSource.SetActive(true);
    }
}
