using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    public int CharacterID { get; set; }
    public string CharacterName { get; set; }
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }
    public int BaseDefence { get; set; }
    public int CurrentDefence { get; set; }
    public int MaxPoise { get; set; }
    public int CurrentPoise { get; set; }
    public int MinAttack { get; set; }
    public int MaxAttack { get; set; }
    public float CriticalMultiplier { get; set; }
    public float CriticalChance { get; set; }
    public int PoiseAttack { get; set; }
}
