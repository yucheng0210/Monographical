using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectFactory : Singleton<EffectFactory>
{
    public IEffect CreateEffect(string effectType)
    {
        switch (effectType)
        {
            case "IncreaseHealthEffect":
                return new IncreaseHealthEffect();
            case "IncreaseAttackEffect":
                return new IncreaseAttackEffect();
            default:
                return null;
        }
    }
}
