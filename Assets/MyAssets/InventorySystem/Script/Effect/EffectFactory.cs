using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectFactory : IEffectFactory
{
    public IEffect CreateEffect(string effectType)
    {
        switch (effectType)
        {
            case "IncreaseHealth":
                return new IncreaseHealthEffect();
            case "IncreaseAttack":
                return new IncreaseAttackEffect();
            default:
                return null;
        }
    }
}
