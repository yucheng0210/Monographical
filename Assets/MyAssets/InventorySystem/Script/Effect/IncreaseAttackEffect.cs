using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseAttackEffect : IEffect
{
    private int value;

    public IncreaseAttackEffect(int value)
    {
        this.value = value;
    }

    public void ApplyEffect()
    {
        DataManager.Instance.CharacterList["Player"].AdditionalAttack += value;
    }
}
