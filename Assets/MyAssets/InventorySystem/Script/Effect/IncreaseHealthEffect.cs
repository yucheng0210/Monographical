using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseHealthEffect : IEffect
{
    private int value;

    public IncreaseHealthEffect(int value)
    {
        this.value = value;
    }

    public void ApplyEffect()
    {
        DataManager.Instance.CharacterList["Player"].TakeDamage(
            DataManager.Instance.CharacterList["Player"],
            -value
        );
    }
}
