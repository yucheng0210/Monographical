using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEffect : MonoBehaviour
{
    public GameObject[] BloodFX;
    public Vector3 direction;
    public GameObject BloodAttach;
    int effectIdx;

    public void SpurtingBlood(Ray ray, Vector3 transform)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) || true)
        {
            float angle = Mathf.Atan2(hit.normal.x, hit.normal.z) * Mathf.Rad2Deg + 180;
            if (effectIdx == BloodFX.Length)
                effectIdx = 0;
            var instance = Instantiate(
                BloodFX[effectIdx],
                transform + new Vector3(0, 0.75f, 0),
                Quaternion.Euler(0, angle + 90, 0)
            );
            effectIdx++;
            var settings = instance.GetComponent<BFX_BloodSettings>();
            settings.GroundHeight = transform.y;
            Instantiate(BloodAttach, transform, Quaternion.identity);
        }
    }
}
