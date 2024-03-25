using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseHealthEffect : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        Main.Manager.GameManager.Instance.TakeDamage(-value, Main.Manager.GameManager.Instance.PlayerData);
    }
}
