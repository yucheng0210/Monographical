using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseHealthEffect : IEffect
{
    public void ApplyEffect(int value, string target)
    {
       /* DataManager.Instance.CharacterList[target].TakeDamage(
            DataManager.Instance.CharacterList[target],
            -value
        );*/
    }
}
