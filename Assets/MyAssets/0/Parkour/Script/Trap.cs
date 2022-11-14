using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trap : MonoBehaviour, IObserver
{
    private List<GameObject> traps;

    [SerializeField]
    private float moveSpeed;

    private void Start()
    {
        traps = new List<GameObject>(transform.childCount);
        GameManager.Instance.AddObservers(this);
        for (int i = 0; i < transform.childCount; i++)
        {
            traps.Add(transform.GetChild(i).gameObject);
        }
    }

    private void OnDisable()
    {
        GameManager.Instance.RemoveObservers(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Rigidbody trapBody = traps[i].GetComponent<Rigidbody>();
                ;
                trapBody.velocity = transform.forward * moveSpeed;
            }
        }
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
