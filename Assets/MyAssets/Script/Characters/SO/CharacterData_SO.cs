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
    public float maxPoise;
    public float currentPoise;

    [Header("位置")]
    public Vector3 currentPos;
    public Quaternion currentRotation;
}
