using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterState : MonoBehaviour
{
    [SerializeField]
    private CharacterData_SO templateData;

    [SerializeField]
    private CharacterData_SO characterData;

    [SerializeField]
    private AttackData_SO attackData;

    [SerializeField]
    private string characterName;
    public int AdditionalAttack { get; set; }
    public string CharacterName
    {
        get { return characterName; }
    }

    //[HideInInspector]
    private bool isCritical;
    private float poiseRecoverTime;
    private bool poiseRecoverBool;

    private void Awake()
    {
        if (templateData != null)
            characterData = Instantiate(templateData);
        characterData.currentPoise = characterData.maxPoise;
    }

    #region Read from NormalData
    public float MaxHealth
    {
        get
        {
            if (characterData != null)
                return characterData.maxHealth;
            else
                return 0;
        }
        set
        {
            characterData.maxHealth = (int)value;
            if (characterData.maxHealth < 0)
                characterData.maxHealth = 0;
        }
    }
    public float CurrentHealth
    {
        get
        {
            if (characterData != null)
                return characterData.currentHealth;
            else
                return 0;
        }
        set
        {
            characterData.currentHealth = (int)value;
            if (characterData.currentHealth < 0)
                characterData.currentHealth = 0;
            if (characterData.currentHealth > characterData.maxHealth)
                characterData.currentHealth = characterData.maxHealth;
        }
    }
    public int BaseDefence
    {
        get
        {
            if (characterData != null)
                return characterData.baseDefence;
            else
                return 0;
        }
        set
        {
            characterData.baseDefence = (int)value;
            if (characterData.baseDefence < 0)
                characterData.baseDefence = 0;
        }
    }
    public float CurrentDefence
    {
        get
        {
            if (characterData != null)
                return characterData.currentDefence;
            else
                return 0;
        }
        set
        {
            characterData.currentDefence = (int)value;
            if (characterData.currentDefence < 0)
                characterData.currentDefence = 0;
            if (characterData.currentDefence > characterData.baseDefence)
                characterData.currentDefence = characterData.baseDefence;
        }
    }
    public float MaxPoise
    {
        get { return characterData.maxPoise; }
        set
        {
            characterData.maxPoise = value;
            if (characterData.maxPoise < 0)
                characterData.maxPoise = 0;
        }
    }
    public float CurrentPoise
    {
        get { return characterData.currentPoise; }
        set
        {
            characterData.currentPoise = value;
            if (characterData.currentPoise < 0)
                characterData.currentPoise = 0;
            if (characterData.currentPoise > characterData.maxPoise)
                characterData.currentPoise = characterData.maxPoise;
        }
    }
    #endregion
    #region Read from AttackData

    public float CriticalChance
    {
        get
        {
            if (attackData != null)
                return attackData.criticalChance;
            else
                return 0;
        }
        set
        {
            attackData.criticalChance = value;
            if (attackData.criticalChance > 1)
                attackData.criticalChance = 1;
            if (attackData.criticalChance < 0)
                attackData.criticalChance = 0;
        }
    }
    public int MinAttack
    {
        get
        {
            if (attackData != null)
                return attackData.minAttack;
            else
                return 0;
        }
        set
        {
            attackData.minAttack = (int)value;
            if (attackData.minAttack > attackData.maxAttack)
                attackData.minAttack = attackData.maxAttack;
            if (attackData.minAttack < 0)
                attackData.minAttack = 0;
        }
    }
    public int MaxAttack
    {
        get
        {
            if (attackData != null)
                return attackData.maxAttack;
            else
                return 0;
        }
        set
        {
            attackData.maxAttack = (int)value;
            if (attackData.maxAttack < 0)
                attackData.maxAttack = 0;
        }
    }
    public float CriticalMultiplier
    {
        get
        {
            if (attackData != null)
                return attackData.criticalMultiplier;
            else
                return 0;
        }
        set
        {
            attackData.criticalMultiplier = value;
            if (attackData.criticalMultiplier < 1)
                attackData.criticalMultiplier = 1;
        }
    }
    public float PoiseAttack
    {
        get { return attackData.poiseAttack; }
        set
        {
            attackData.poiseAttack = value;
            if (attackData.poiseAttack < 0)
                attackData.poiseAttack = 0;
        }
    }
    #endregion
    #region Read from Transform
    public Vector3 Pos
    {
        get { return characterData.currentPos; }
        set { characterData.currentPos = value; }
    }
    public Quaternion Rotation
    {
        get { return characterData.currentRotation; }
        set { characterData.currentRotation = value; }
    }
    #endregion
    public CharacterData_SO CharacterData
    {
        get { return characterData; }
    }

    private void Update()
    {
        if (poiseRecoverBool)
            poiseRecoverTime += Time.deltaTime;
        if (poiseRecoverTime >= 8)
        {
            CurrentPoise = MaxPoise;
            poiseRecoverBool = false;
        }
    }

    public void TakeDamage(CharacterState attacker, CharacterState defender)
    {
        poiseRecoverTime = 0;
        poiseRecoverBool = true;
        int damage = (int)Mathf.Max(attacker.CurrentAttack() - defender.CurrentDefence, 0);
        damage += AdditionalAttack;
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        CurrentPoise = Mathf.Max(defender.CurrentPoise - attacker.PoiseAttack, 0);
    }

    public void TakeDamage(CharacterState character, int damage)
    {
        poiseRecoverTime = 0;
        poiseRecoverBool = true;
        damage += AdditionalAttack;
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
    }

    private int CurrentAttack()
    {
        float damage = Random.Range(MinAttack, MaxAttack);
        isCritical = Random.value < CriticalChance;
        if (isCritical)
            damage *= CriticalMultiplier;
        return (int)damage;
    }
}
