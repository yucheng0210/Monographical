using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "CharacterState/NormalData")]
public class CharacterData_SO : ScriptableObject
{
    [Header("狀態")]
    public int maxHealth;
    public int currentHealth;
    public int baseDefence;
    public int currentDefence;
}
