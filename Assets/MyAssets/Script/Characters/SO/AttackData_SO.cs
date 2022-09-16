using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "CharacterState/AttackData")]
public class AttackData_SO : ScriptableObject
{
    public int minAttack;
    public int maxAttack;
    public float criticalMultiplier;
    public float criticalChance;
    public float poiseAttack;
}
