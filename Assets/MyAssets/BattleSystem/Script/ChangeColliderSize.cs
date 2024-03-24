using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class ChangeColliderSize : MonoBehaviour
{
    [SerializeField]
    private Vector3 destinationSize = new Vector3(5, 1, 13);
    [SerializeField]
    private float duration = 0.45f;
    private float sizeZ = 0;
    private void Update()
    {
        DOTween.To(() => sizeZ, x => sizeZ = x, destinationSize.z, duration);
        GetComponent<BoxCollider>().size = new Vector3(destinationSize.x, destinationSize.y, sizeZ);
        GetComponent<BoxCollider>().center = new Vector3(0, 0, sizeZ / 2);
    }

}
