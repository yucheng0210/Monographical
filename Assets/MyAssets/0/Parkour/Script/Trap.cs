using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trap : MonoBehaviour, IObserver
{
    private GameObject trap;
    private Rigidbody trapBody;

    [SerializeField]
    private float force;

    private void Start()
    {
        GameManager.Instance.AddObservers(this);
        trap = transform.parent.gameObject.transform.GetChild(1).gameObject;
        trapBody = trap.gameObject.GetComponent<Rigidbody>();
    }

    private void OnDisable()
    {
        GameManager.Instance.RemoveObservers(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            trapBody.velocity = transform.forward * force;
    }

    private void OnTriggerExit(Collider other)
    {
        /* if (other.CompareTag("Player"))
            clueCanvas.SetActive(false);*/
    }

    public void EndNotify()
    {
        // clueCanvas.SetActive(false);
    }

    public void SceneLoadingNotify(bool loadingBool)
    {
        //        throw new System.NotImplementedException();
    }
}
