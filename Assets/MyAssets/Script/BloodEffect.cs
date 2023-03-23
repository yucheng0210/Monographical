using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEffect : MonoBehaviour
{
    [SerializeField]
    private GameObject[] bloodFX;

    [SerializeField]
    private GameObject bloodAttach;

    [SerializeField]
    private List<Transform> bodyTransList = new List<Transform>();
    private Light dirLight;
    private int effectIdx;

    private void Awake()
    {
        dirLight = GameObject.Find("Directional Light").GetComponent<Light>();
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

    public void SpurtingBlood(Vector3 hitPoint)
    {
        float angle = Mathf.Atan2(hitPoint.x, hitPoint.z) * Mathf.Rad2Deg + 180;
        if (effectIdx == bloodFX.Length)
            effectIdx = 0;
        var instance = Instantiate(
            bloodFX[effectIdx],
            hitPoint,
            Quaternion.Euler(0, angle + 90, 0)
        );
        effectIdx++;
        var settings = instance.GetComponent<BFX_BloodSettings>();
        settings.LightIntensityMultiplier = dirLight.intensity;
        settings.GroundHeight = transform.position.y;
        Instantiate(bloodAttach, transform.position, Quaternion.identity);
        float lastDistance = 99;
        Transform bodyDecalTrans = null;
        for (int i = 0; i < bodyTransList.Count; i++)
        {
            if (Vector3.Distance(bodyTransList[i].position, hitPoint) < lastDistance)
            {
                bodyDecalTrans = bodyTransList[i];
                lastDistance = Vector3.Distance(bodyTransList[i].position, hitPoint);
            }
        }
        Instantiate(bloodAttach, bodyDecalTrans);
    }
}
