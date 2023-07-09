using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectFactory : IEffectFactory
{
    public IEffect CreateEffect(string effectType, int value)
    {
        switch (effectType)
        {
            case "IncreaseHealthEffect":
                return new IncreaseHealthEffect(value);
            case "IncreaseAttackEffect":
                return new IncreaseAttackEffect(value);
            default:
                return null;
        }
    }
}
