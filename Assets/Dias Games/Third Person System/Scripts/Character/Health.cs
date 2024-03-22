using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;
using UnityEngine.UI;
using Ilumisoft.Minesweeper;

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
        private int arrowAttackLayer;

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
        [SerializeField]
        private int playerID;
        [SerializeField]
        private GameObject onfireEffect;
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
            arrowAttackLayer = LayerMask.NameToLayer("ArrowAttack");
            enemyAttackLayer = LayerMask.NameToLayer("EnemyAttack");
            characterState = GetComponent<CharacterState>();
        }

        private void Start()
        {
            Main.Manager.GameManager.Instance.RegisterPlayer(DataManager.Instance.CharacterList[playerID].Clone(), transform, ani);
            //DataManager.Instance.AddCharacterRegister(characterState);
            EventManager.Instance.AddEventRegister(EventDefinition.eventIsHited, IsHited);
            EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerInvincible, EventInvincible);
        }

        private void Update()
        {
            healthSlider.value = (float)Main.Manager.GameManager.Instance.PlayerData.CurrentHealth
            / (float)Main.Manager.GameManager.Instance.PlayerData.MaxHealth;
            animatorStateInfo = ani.GetCurrentAnimatorStateInfo(0);
            if (animatorStateInfo.IsName("StandUp"))
                ani.ResetTrigger("isHited");
        }
        private IEnumerator OnFire()
        {
            while (onfireEffect.activeSelf && Main.Manager.GameManager.Instance.PlayerData.CurrentHealth > 0)
            {
                Main.Manager.GameManager.Instance.TakeDamage(DataManager.Instance.CharacterList[3006]
                , Main.Manager.GameManager.Instance.PlayerData);
                //Debug.Log("OnFire");
                if (Main.Manager.GameManager.Instance.PlayerData.CurrentHealth <= 0)
                    Die();
                yield return new WaitForSeconds(1);
            }
        }
        public void Extinguishing()
        {
            int randomIndex = UnityEngine.Random.Range(0, 4);
            if (randomIndex == 0)
                onfireEffect.SetActive(false);
        }
        private void OnTriggerEnter(Collider other)
        {
            bool otherLayerBool = other.gameObject.layer == enemyAttackLayer || other.gameObject.layer == arrowAttackLayer
            || other.gameObject.layer == LayerMask.NameToLayer("Trap") || other.gameObject.layer == LayerMask.NameToLayer("Attack");
            Character enemyData = null;
            if (otherLayerBool && Main.Manager.GameManager.Instance.PlayerData.CurrentHealth >= 0
             && !animatorStateInfo.IsName("StandUp"))
            {
                Debug.Log("damage");
                if (isInvincible)
                    return;
                isInvincible = true;
                if (isInvincible)
                {
                    StopCoroutine(ResetInvincible());
                    StartCoroutine(ResetInvincible());
                }
                if (other.gameObject.layer == arrowAttackLayer)
                    enemyData = other.GetComponent<Arrow>().EnemyData;
                else if (other.gameObject.layer == LayerMask.NameToLayer("Trap"))
                    enemyData = other.transform.root.GetComponent<DungeonTrap>().TrapData;
                else if (other.gameObject.layer == enemyAttackLayer)
                    enemyData = other.transform.root.GetComponent<PatrolEnemy>().EnemyData;
                else
                    enemyData = other.GetComponent<AttackID>().AttackData;
                IsHited(other, enemyData);
            }
        }
        private void IsHited(params object[] other)
        {
            Collider newOther = (Collider)other[0];
            Character enemyData = (Character)other[1];
            Main.Manager.GameManager.Instance.TakeDamage(enemyData, Main.Manager.GameManager.Instance.PlayerData);
            Vector3 hitPoint = new Vector3(
                newOther.bounds.center.x,
                newOther.ClosestPointOnBounds(transform.position).y,
                newOther.bounds.center.z
            );
            HitEffect(hitPoint, newOther);
            Vector3 direction = newOther.transform.forward + newOther.transform.up;
            if (Main.Manager.GameManager.Instance.PlayerData.CurrentPoise <= 0)
            {
                ani.SetFloat("BeakBackMode", 2);
                Main.Manager.GameManager.Instance.PlayerData.CurrentPoise = Main.Manager.GameManager.Instance.PlayerData.MaxPoise;
                //rigidbody.AddForce(direction * fallDownForce, ForceMode.Impulse);
            }
            else
                ani.SetFloat("BeakBackMode", 1);
            ani.SetTrigger("isHited");
            if (Main.Manager.GameManager.Instance.PlayerData.CurrentHealth <= 0)
                Die();
            else
            {
                OnReceiveDamage.Invoke();
                OnCharacterDamage?.Invoke();
            }
        }
        private IEnumerator ResetInvincible()
        {
            yield return new WaitForSecondsRealtime(0.5f);
            isInvincible = false;
        }
        private void HitEffect(Vector3 hitPoint, Collider other)
        {
            GetComponent<HitStop>().StopTime(0.1f, 0.2f);
            myImpulse.GenerateImpulse();
            if (other.CompareTag("Light"))
                Destroy(Instantiate(lightHitSpark, hitPoint, Quaternion.identity), 2);
            else if (other.CompareTag("Fire") && !onfireEffect.activeSelf)
            {
                onfireEffect.SetActive(true);
                StartCoroutine(OnFire());
            }
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
            Main.Manager.GameManager.Instance.EndNotifyObservers();

            // Play sound
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayerDied();
            EventManager.Instance.DispatchEvent(EventDefinition.eventGameOver);
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
                //Die();
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
            if (isInvincible)
            {
                StopCoroutine(ResetInvincible());
                StartCoroutine(ResetInvincible());
            }
        }

    }
}
