using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;
using UnityEngine.UI;

namespace DiasGames.ThirdPersonSystem
{
    public class Health : Modifier
    {
        [SerializeField]
        private float m_MaxHealth = 100f;

        [SerializeField]
        private bool m_RestartSceneAfterDie = true;

        [SerializeField]
        private float m_WaitToRestart = 3f;

        [Header("Events")]
        [SerializeField]
        private UnityEvent OnReceiveDamage;

        [SerializeField]
        private UnityEvent OnDie;

        // Action Events
        public Action OnCharacterDie,
            OnCharacterDamage,
            OnHealthChanged;

        [SerializeField]
        private float m_CurrentHealth;
        private Rigidbody[] ragdollRigidbodies;
        private List<Collider> allColliders = new List<Collider>();
        private Cinemachine.CinemachineImpulseSource myImpulse;
        private int enemyAttackLayer;

        /*public float HealthValue
        {
            get { return m_CurrentHealth; }
            set
            {
                m_CurrentHealth = value;
                if (m_CurrentHealth > m_MaxHealth)
                    m_CurrentHealth = m_MaxHealth;
                if (m_CurrentHealth < 0)
                    m_CurrentHealth = 0;
            }
        }
        public float MaximumHealth
        {
            get { return m_MaxHealth; }
        }*/

        private float m_LastDamagedTime = 0;
        public Slider healthSlider;

        [SerializeField]
        private float recover;

        [SerializeField]
        private GameObject hitSpark;
        [SerializeField]
        private GameObject lightHitSpark;
        [SerializeField]
        private GameObject hitDistortion;
        public float attack;
        private Animator ani;
        private CharacterState characterState,
            attackerCharacterState;

        [SerializeField]
        private bool isInvincible;

        [SerializeField]
        private float fallDownForce;
        private AnimatorStateInfo animatorStateInfo;
        //private Rigidbody rigidbody;

        private void Awake()
        {
            //rigidbody = GetComponent<Rigidbody>();
            ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
            allColliders.AddRange(GetComponentsInChildren<Collider>());
            ani = GetComponent<Animator>();
            myImpulse = GetComponent<Cinemachine.CinemachineImpulseSource>();

            m_CurrentHealth = m_MaxHealth;

            GlobalEvents.AddEvent("Damage", Damage);
            GlobalEvents.AddEvent("Restart", RespawnCharacter);
            GlobalEvents.AddEvent("RestoreHealth", RestoreHealth);

            DisableRagdoll();
            enemyAttackLayer = LayerMask.NameToLayer("EnemyAttack");
            characterState = GetComponent<CharacterState>();
            InitialState();
        }

        private void Start()
        {
            GameManager.Instance.RegisterPlayer(characterState);
            DataManager.Instance.AddCharacterRegister(characterState);
            EventManager.Instance.AddEventRegister(EventDefinition.eventIsHited, IsHited);
            EventManager.Instance.AddEventRegister(
                EventDefinition.eventPlayerInvincible,
                EventInvincible
            );
        }

        private void Update()
        {
            healthSlider.value = characterState.CurrentHealth / characterState.MaxHealth;
            animatorStateInfo = ani.GetCurrentAnimatorStateInfo(0);
            if (animatorStateInfo.IsName("StandUp"))
                ani.ResetTrigger("isHited");
        }

        private void InitialState()
        {
            characterState.CurrentHealth = characterState.MaxHealth;
            characterState.CurrentDefence = characterState.BaseDefence;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (
                other.gameObject.layer == enemyAttackLayer
                && characterState.CurrentHealth >= 0
                && !animatorStateInfo.IsName("StandUp")
            )
                IsHited(other);
        }

        private void IsHited(params object[] other)
        {
            if (isInvincible)
                return;

            Collider newOther = (Collider)other[0];
            attackerCharacterState = newOther.gameObject.GetComponentInParent<CharacterState>();
            characterState.TakeDamage(attackerCharacterState, characterState);
            Vector3 hitPoint = new Vector3(
                newOther.bounds.center.x,
                newOther.ClosestPointOnBounds(transform.position).y,
                newOther.bounds.center.z
            );
            HitEffect(hitPoint, newOther);
            Vector3 direction = newOther.transform.forward + newOther.transform.up;
            if (characterState.CurrentPoise <= 0)
            {
                ani.SetFloat("BeakBackMode", 2);
                characterState.CurrentPoise = characterState.MaxPoise;
                //rigidbody.AddForce(direction * fallDownForce, ForceMode.Impulse);
            }
            else
                ani.SetFloat("BeakBackMode", 1);
            ani.SetTrigger("isHited");
            if (characterState.CurrentHealth <= 0)
                Die();
            else
            {
                OnReceiveDamage.Invoke();
                OnCharacterDamage?.Invoke();
            }
        }

        private void HitEffect(Vector3 hitPoint, Collider other)
        {
            GetComponent<HitStop>().StopTime();
            myImpulse.GenerateImpulse();
            if (other.CompareTag("Light"))
                Destroy(Instantiate(lightHitSpark, hitPoint, Quaternion.identity), 2);
            else
                Destroy(Instantiate(hitSpark, hitPoint, Quaternion.identity), 2);
            Destroy(Instantiate(hitDistortion, hitPoint, Quaternion.identity), 2);
            AudioManager.Instance.Impact();
            AudioManager.Instance.PlayerHurted();
            GetComponent<BloodEffect>().SpurtingBlood(hitPoint);
            //AudioManager.Instance.PlayerHurted();
        }

        private void RestoreHealth(GameObject obj, object value)
        {
            m_CurrentHealth += (float)value;
            m_CurrentHealth = Mathf.Clamp(m_CurrentHealth, 0, m_MaxHealth);
            OnHealthChanged?.Invoke();
        }

        public void Die()
        {
            if (m_System != null)
            {
                m_System.enabled = false;
                m_System.m_Animator.enabled = false;
                m_System.m_Capsule.sharedMaterial = null;
            }

            EnableRagdoll();
            OnDie.Invoke();
            OnCharacterDie?.Invoke();
            GameManager.Instance.EndNotifyObservers();

            // Play sound
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayerDied();

            if (m_RestartSceneAfterDie)
                StartCoroutine(RestartCharacter());
        }

        private IEnumerator RestartCharacter()
        {
            yield return new WaitForSeconds(m_WaitToRestart);

            //GlobalEvents.ExecuteEvent("Restart", null, null);
        }

        private void RespawnCharacter(GameObject obj, object value)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void Damage(GameObject obj, object amount)
        {
            if (m_LastDamagedTime > Time.fixedTime && m_RestartSceneAfterDie)
                return;

            if (obj != gameObject)
            {
                if (allColliders.Find(x => x.gameObject == obj) == null)
                    return;
            }

            m_CurrentHealth -= (float)amount;
            if (m_CurrentHealth <= 0)
            {
                m_CurrentHealth = 0;
                Die();
            }
            else
            {
                OnReceiveDamage.Invoke();
                OnCharacterDamage?.Invoke();
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayerHurted();
            }

            m_LastDamagedTime = Time.fixedTime + 0.1f;
            OnHealthChanged?.Invoke();
        }

        private void EnableRagdoll()
        {
            if (ragdollRigidbodies.Length <= 0)
                return;

            Vector3 vel = ragdollRigidbodies[0].velocity;
            ragdollRigidbodies[0].isKinematic = true;

            for (int i = 1; i < ragdollRigidbodies.Length; i++)
            {
                ragdollRigidbodies[i].isKinematic = false;
                ragdollRigidbodies[i].useGravity = true;
                ragdollRigidbodies[i].velocity = vel;
            }
        }

        private void DisableRagdoll()
        {
            if (ragdollRigidbodies.Length <= 0)
                return;

            ragdollRigidbodies[0].isKinematic = false;

            for (int i = 1; i < ragdollRigidbodies.Length; i++)
                ragdollRigidbodies[i].isKinematic = true;
        }

        private void EventInvincible(params object[] args)
        {
            isInvincible = (bool)args[0];
        }
    }
}
