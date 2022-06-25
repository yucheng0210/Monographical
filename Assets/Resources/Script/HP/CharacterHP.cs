using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHP : MonoBehaviour
{
    [SerializeField]
    private float maxHealth;

    [SerializeField]
    private float currentHealth;

    [SerializeField]
    private Slider healthSlider;

    [SerializeField]
    private AudioClip hurt,
        death;

    [SerializeField]
    private GameObject dropItem;

    [SerializeField]
    private float waitTime;

    [SerializeField]
    private float attack;
    public Animator Ani { get; set; }
    public AudioSource CharacterSource { get; set; }
    public Slider HealthSlider
    {
        get { return healthSlider; }
    }
    public float MaxHealth
    {
        get { return maxHealth; }
        set
        {
            if (value < 0)
                value = 1;
            maxHealth = value;
        }
    }
    public float CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            currentHealth = value;
            if (currentHealth < 0)
                currentHealth = 0;
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
        }
    }
    public float Attack
    {
        get { return attack; }
        set
        {
            attack = value;
            if (attack < 0)
                attack = 1;
        }
    }

    public virtual void Awake()
    {
        Ani = GetComponent<Animator>();
        CharacterSource = GetComponent<AudioSource>();
        CurrentHealth = MaxHealth;
    }

    public virtual void Update()
    {
        HealthSlider.value = (CurrentHealth / MaxHealth);
    }

    virtual public void TakeDamage(GameObject target, float damage)
    {
        if (!Ani.GetBool("isDead"))
        {
            CharacterSource.PlayOneShot(hurt, 0.1f);
            CurrentHealth -= damage;
        }
        if (CurrentHealth <= 0)
            StartCoroutine(Death());
    }

    public virtual IEnumerator Death()
    {
        CharacterSource.PlayOneShot(death, 0.1f);
        Ani.SetBool("isDead", true);
        yield return new WaitForSeconds(waitTime);
        Instantiate(dropItem, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
