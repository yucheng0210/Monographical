using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRule_CantRun : MonoBehaviour
{
    [SerializeField]
    private GameObject playerSword;
    [SerializeField]
    private GameObject playerShield;
    private BoxCollider myCollider;

    private void Awake()
    {
        myCollider = GetComponent<BoxCollider>();
    }
    private void OnTriggerStay(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (Main.Manager.GameManager.Instance.PlayerAni.GetFloat("Forward") > 0.5f)
            {
                AudioManager.Instance.DanceAudio();
                myCollider.enabled = false;
                playerSword.SetActive(false);
                playerShield.SetActive(false);
                Main.Manager.GameManager.Instance.PlayerAni.SetBool("isDancing", true);
                EventManager.Instance.DispatchEvent(EventDefinition.eventIsHited, myCollider);
                EventManager.Instance.DispatchEvent(EventDefinition.eventPlayerInvincible, true, false);
            }
        }
    }
}
