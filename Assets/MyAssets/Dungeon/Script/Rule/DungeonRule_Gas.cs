using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRule_Gas : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem gas;
    private BoxCollider boxCollider;
    private BoxCollider gasCollider;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        gasCollider = gas.GetComponent<BoxCollider>();
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            boxCollider.enabled = false;
            StartCoroutine(ShrinkCircle());
        }
    }
    private IEnumerator ShrinkCircle()
    {
        yield return new WaitForSecondsRealtime(5);
        gas.gameObject.SetActive(true);
        ParticleSystem.ShapeModule shapeModule = gas.shape;
        while (gas.shape.scale.y <= 20)
        {
            shapeModule.scale += new Vector3(0, 0.25f, 0);
            shapeModule.position -= new Vector3(0, 0.25f, 0);
            gasCollider.size += new Vector3(0, 0.25f, 0);
            gasCollider.center -= new Vector3(0, 0.25f, 0);
            yield return new WaitForSecondsRealtime(1);
        }

    }
}
