using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseAttackEffect : IEffect
{
    public void ApplyEffect(string target, int value)
    {
        DataManager.Instance.CharacterList[target].MinAttack += value;
        DataManager.Instance.CharacterList[target].MaxAttack += value;
    }
}
