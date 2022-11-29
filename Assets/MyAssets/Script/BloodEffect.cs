using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEffect : MonoBehaviour
{
    [SerializeField]
    private GameObject[] BloodFX;

    [SerializeField]
    private GameObject BloodAttach;
    private Light dirLight;
    private int effectIdx;
    private Vector3 direction;
    private Ray ray;

    private void Awake()
    {
        dirLight = GameObject.Find("Directional Light").GetComponent<Light>();
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, transform.up * 2, Color.red);
        ray = new Ray(transform.position, transform.up);
        SpurtingBlood();
    }

    private Transform GetNearestObject(Transform hit, Vector3 hitPos)
    {
        var closestPos = 100f;
        Transform closestBone = null;
        var childs = hit.GetComponentsInChildren<Transform>();

        foreach (var child in childs)
        {
            var dist = Vector3.Distance(child.position, hitPos);
            if (dist < closestPos)
            {
                closestPos = dist;
                closestBone = child;
            }
        }

        var distRoot = Vector3.Distance(hit.position, hitPos);
        if (distRoot < closestPos)
        {
            closestPos = distRoot;
            closestBone = hit;
        }
        return closestBone;
    }

    public void SpurtingBlood()
    {
        RaycastHit hit;
        int intLayer = LayerMask.NameToLayer("Character");
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log(hit.transform.position);
            float angle = Mathf.Atan2(hit.normal.x, hit.normal.z) * Mathf.Rad2Deg + 180;
            if (effectIdx == BloodFX.Length)
                effectIdx = 0;
            var instance = Instantiate(
                BloodFX[effectIdx],
                hit.point,
                Quaternion.Euler(0, angle + 90, 0)
            );
            effectIdx++;
            var settings = instance.GetComponent<BFX_BloodSettings>();
            settings.LightIntensityMultiplier = dirLight.intensity;
            var nearestBone = GetNearestObject(hit.transform.root, hit.point);
            //settings.GroundHeight = transform.y;
            //Instantiate(BloodAttach, transform, Quaternion.identity);
            if (nearestBone != null)
            {
                var attachBloodInstance = Instantiate(BloodAttach);
                var bloodT = attachBloodInstance.transform;
                bloodT.position = hit.point;
                bloodT.localRotation = Quaternion.identity;
                bloodT.localScale = Vector3.one * Random.Range(0.75f, 1.2f);
                bloodT.LookAt(hit.point + hit.normal, direction);
                bloodT.Rotate(90, 0, 0);
                bloodT.transform.parent = nearestBone;
                //Destroy(attachBloodInstance, 20);
            }
        }
    }
}
