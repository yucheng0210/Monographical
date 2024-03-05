using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;

public class Boss : PatrolEnemy
{
    [SerializeField]
    private GameObject anotherCollision;
    [Header("怒吼")]
    [SerializeField]
    private UnityEngine.Rendering.Volume mainVolumeProfile;
    [SerializeField]
    private float roarIntensity;
    [SerializeField]
    private float roarOnceDuration;
    protected override void AdditionalAttack()
    {
        meleeAttackCount = 3;
        base.AdditionalAttack();
    }
    private IEnumerator Roar()
    {
        if (mainVolumeProfile.profile.TryGet(out RadialBlur radialBlur))
            radialBlur.intensity.Override(roarIntensity);
        myImpulse.GenerateImpulse();
        yield return new WaitForSeconds(roarOnceDuration);
        radialBlur.intensity.Override(0);
    }
    public override void ColliderSwitch(int switchCount)
    {
        base.ColliderSwitch(switchCount);
        if (switchCount == 1)
            anotherCollision.SetActive(true);
        else
            anotherCollision.SetActive(false);
    }
}
