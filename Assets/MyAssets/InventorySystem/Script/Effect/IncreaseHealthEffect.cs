using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseHealthEffect : IEffect
{
    public void ApplyEffect(string target, int value)
    {
        DataManager.Instance.CharacterList[target].TakeDamage(
            DataManager.Instance.CharacterList[target],
            -value
        );
    }
}
