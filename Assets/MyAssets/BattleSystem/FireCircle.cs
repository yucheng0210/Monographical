using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCircle : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<RFX4_EffectEvent>().ActivateEffect();
    }
}
