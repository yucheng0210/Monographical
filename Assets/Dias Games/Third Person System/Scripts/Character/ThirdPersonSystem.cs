/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;
using DG.Tweening;

namespace DiasGames.ThirdPersonSystem
{
    public enum MoveOnAxis
    {
        XYZ,
        YZ,
        XY
    }

    public class ThirdPersonSystem : MonoBehaviour, IObserver
    {
        #region Components

        // ------------------ Internal vars and components ----------------------- //

        private ThirdPersonAbility m_ActiveAbility = null; // Active ability
        private UnityInputManager m_InputManager;
        private AudioSource m_AudioSource;

        public ThirdPersonAbility ActiveAbility
        {
            get { return m_ActiveAbility; }
        }

        /// <summary>Rigidbody component reference of charater </summary>
        public Rigidbody m_Rigidbody { get; set; }

        /// <summary>Aniamtor component reference of charater </summary>
        public Animator m_Animator { get; set; }

        /// <summary>Capsule collider component reference of charater </summary>
        public CapsuleCollider m_Capsule { get; set; }

        // ----------------------------------------------------------------------- //

        #endregion

        #region Exposed parameters
        /// <summary>
        /// Controls wheter the character is on ground or falling
        /// </summary>
        public bool IsGrounded { get; set; }

        /// <summary>
        /// The normal vector of the ground. This information is useful to find angle of slope
        ///</summary>
        public Vector3 GroundNormal { get; private set; }

        [Tooltip("Layers to threat as a ground")]
        [SerializeField]
        private LayerMask m_GroundMask =
            (1 << 0) | (1 << 14) | (1 << 16) | (1 << 17) | (1 << 18) | (1 << 19) | (1 << 20);

        [Tooltip("Distance from origin to find a ground. Higher values, earlier detects a ground")]
        [SerializeField]
        private float m_GroundCheckDistance = 0.35f;

        [Tooltip("Max slope angle, in degrees, that player can walk")]
        [SerializeField]
        private float m_MaxAngleSlope = 45f;

        [Tooltip("Gravity Acceleration. It will override Physics.Gravity property")]
        [SerializeField]
        float m_GravityAcceleration = 19.6f;

        [Tooltip("Should character keeping zooming camera?")]
        [SerializeField]
        private bool m_AlwaysZoomCamera = false;

        private List<ThirdPersonAbility> m_Abilities = new List<ThirdPersonAbility>();

        public UnityEvent OnAnyAbilityEnters,
            OnAnyAbilityExits,
            OnGrounded;
        public Action<ThirdPersonAbility> OnAbilityChanged;

        #endregion

        #region Private internal parameters

        // Original parameters of capsule
        private float _capsuleOriginHeight;
        private Vector3 _capsuleOriginCenter;
        private float _capsuleOriginRadius;
        private PhysicMaterial capsuleOriginalMaterial;
        private RaycastHit m_GroundHit;

        // private vars
        private bool isZomming = false;

        #endregion

        #region Public parameters and getter

        // ---------------------------------- GETTERS ------------------------------------- //

        public LayerMask GroundMask
        {
            get { return m_GroundMask; }
            set { m_GroundMask = value; }
        }

        public float MaxAngleSlope
        {
            get { return m_MaxAngleSlope; }
        }

        public UnityInputManager InputManager
        {
            get { return m_InputManager; }
        }

        public bool IsZooming
        {
            get { return isZomming; }
        }

        public float CapsuleOriginalHeight
        {
            get { return _capsuleOriginHeight; }
        }

        public List<ThirdPersonAbility> CharacterAbilities
        {
            get { return m_Abilities; }
        }

        // -------------------------------------------------------------------------------- //

        public float GroundCheckDistance { get; set; } // Created to allow change ground distance in runtime and preserve the initial m_GroundCheckDistance
        public bool IsCoroutinePlaying { get; set; } // Avoid play more than one coroutine per time

        /// <summary>
        /// It returns the last ability played by the system
        /// </summary>
        public ThirdPersonAbility LastAbility { get; private set; } = null;

        #endregion

        #region Movement Parameters

        public float m_StationaryTurnSpeed = 180f; // Turn speed on idle state
        public float m_MovingTurnSpeed = 360f; // Turn speed when moving

        [Tooltip("Here you can set which axis character can move. Use YZ to create a 2.5D level")]
        [SerializeField]
        private MoveOnAxis m_MovementAxis = MoveOnAxis.XYZ;

        public float m_TurnAmount { get; private set; } // Turn amount when moving
        public float m_ForwardAmount { get; private set; } // forward amount when moving

        public float m_HorizontalAmount { get; private set; } // Horizontal amount for strafe
        public float m_VerticalAmount { get; private set; } // Vertical amount for strafe

        private AnimatorManager m_AnimatorManager; // Reference to Animator Manager

        public float TurnAmount
        {
            get { return m_TurnAmount; }
        }

        #endregion

        public RaycastHit GroundHitInfo { get; private set; }

        private bool m_IsAICharacter = false;
        public static bool free;
        private float mouse;
        public GameObject collision;
        [SerializeField]
        private MeleeWeaponTrail meleeWeaponTrail;
        [SerializeField]
        private List<GameObject> weaponTrailList;
        [SerializeField]
        private List<GameObject> swordList = new List<GameObject>();
        [SerializeField]
        private List<string> attributesList = new List<string>();
        [SerializeField]
        private int currentSwordID;

        [SerializeField]
        private Slider enduranceSlider;

        [SerializeField]
        private float maxEndurance;

        [SerializeField]
        private float currentEndurance;

        [Header("攻擊動作消耗值")]
        [SerializeField]
        private float blockConsume;
        [SerializeField]
        private GameObject blockCollision;

        public float attackConsume;
        [SerializeField]
        private float momentumConsume;

        [SerializeField]
        private float rollConsume;
        [Header("重攻擊")]
        [SerializeField]
        private float heavyAttackJumpHeight;
        [SerializeField]
        private float heavyAttackJumpOnceDuration;
        [SerializeField]
        private bool isHeavyAttackJump;
        [SerializeField]
        private GameObject rockExplosionEffect;

        [SerializeField]
        private List<GameObject> slashEffectList = new List<GameObject>();
        private CinemachineImpulseSource myImpulse;

        /*[SerializeField]
        private int combo;*/
        public static bool canRoll;
        private int attack = Animator.StringToHash("AttackMode");
        private CharacterState characterState;

        public bool ShutDown { get; set; }
        private AnimatorStateInfo animatorStateInfo;
        public AnimatorStateInfo StateInfo
        {
            get { return animatorStateInfo; }
        }
        private AnimatorTransitionInfo animatorTransitionInfo;


        [SerializeField]
        private bool isRunning;
        private Quaternion currentRotation;

        public void SetControllerAsAI()
        {
            m_IsAICharacter = true;
        }


        private void OnDisable()
        {
            //GameManager.Instance.RemoveObservers(this);
        }

        private void Awake()
        {
            //AudioManager.Instance.MainAudio();
            // Get components
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Animator = GetComponent<Animator>();
            m_Capsule = GetComponent<CapsuleCollider>();
            m_InputManager = GetComponent<UnityInputManager>();
            m_AnimatorManager = GetComponent<AnimatorManager>();
            m_AudioSource = GetComponent<AudioSource>();
            myImpulse = GetComponent<CinemachineImpulseSource>();
            // Get initial dimensions of capsule
            _capsuleOriginCenter = m_Capsule.center;
            _capsuleOriginHeight = m_Capsule.height;
            _capsuleOriginRadius = m_Capsule.radius;
            capsuleOriginalMaterial = m_Capsule.sharedMaterial;

            //Set initial groundDistance
            GroundCheckDistance = m_GroundCheckDistance;

            IsCoroutinePlaying = false;

            Physics.gravity = new Vector3(
                Physics.gravity.x,
                -m_GravityAcceleration,
                Physics.gravity.z
            );

            m_Abilities.Clear();
            m_Abilities.AddRange(GetComponents<ThirdPersonAbility>());
            foreach (ThirdPersonAbility ability in m_Abilities)
                ability.Initialize(this, m_AnimatorManager, m_InputManager);
            InitialState();
            if (GameObject.FindGameObjectsWithTag("Player").Length > 1)
                Destroy(this.gameObject);
        }
        private void Start()
        {
            EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerCantMove, EventPlayerCantMove);
            EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerBlock, EventPlayerBlock);
        }
        private void FixedUpdate()
        {
            CheckGround(); // check ground bellow character
            Physics.gravity = new Vector3(
                Physics.gravity.x,
                -m_GravityAcceleration,
                Physics.gravity.z
            );

            if (!IsGrounded)
                HandleAir();
            else
                HandleGrounded();

            // ----------------------- ABILITY FIXED UPDATE --------------------------- //
            if (m_ActiveAbility != null)
                m_ActiveAbility.FixedUpdateAbility();
            // ----------------------------------------------------------------- //
            if (currentEndurance < maxEndurance && !animatorStateInfo.IsTag("Attack") && !animatorStateInfo.IsName("Roll"))
                currentEndurance++;
        }

        private void Update()
        {
            if (Time.timeScale == 0)
                return;
            if (ShutDown)
            {
                LockRotation(currentRotation);
                return;
            }
            if (!m_IsAICharacter)
            {
                // Check Camera Zoom
                if (m_InputManager.zoomButton.IsPressed || m_AlwaysZoomCamera)
                    TryZoom();
                else
                    isZomming = false;
            }
            if (!Main.Manager.GameManager.Instance.IsTalking)
                AttackState();
            if (enduranceSlider != null)
                enduranceSlider.value = currentEndurance / maxEndurance;
            // ----------------------- ABILITY UPDATE --------------------------- //

            if (m_ActiveAbility != null)
                m_ActiveAbility.UpdateAbility();

            // ----------------------------------------------------------------- //

            canRoll = currentEndurance >= rollConsume ? true : false;
            currentRotation = transform.rotation;
        }

        private void InitialState()
        {
            //collision.SetActive(false);
            currentEndurance = maxEndurance;
            //combo = 0;
        }
        private void AttackState()
        {
            animatorStateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
            animatorTransitionInfo = m_Animator.GetAnimatorTransitionInfo(0);
            //Main.Manager.GameManager.Instance.PlayerData.Momentum = 100;
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                if (currentSwordID <= 0)
                    currentSwordID = swordList.Count - 1;
                else
                    currentSwordID--;
                SwitchSword();
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                if (currentSwordID >= swordList.Count - 1)
                    currentSwordID = 0;
                else
                    currentSwordID++;
                SwitchSword();
            }

            if (free && currentEndurance >= attackConsume
                && !m_Animator.GetBool("isAttack")
                && !m_Animator.GetBool("isHeavyAttack")
                && m_InputManager.enabled
            )
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        if (Main.Manager.GameManager.Instance.PlayerData.Momentum >= momentumConsume)
                        {
                            // m_Animator.SetTrigger("isHeavyAttack");
                            ReduceMomentum(momentumConsume);
                            ReduceEndurance(attackConsume);
                            EventManager.Instance.DispatchEvent(EventDefinition.eventAttributeAttack);
                        }
                    }
                    else
                    {
                        m_Animator.SetTrigger("isAttack");
                        ReduceEndurance(attackConsume);
                    }
                }
            }
            if (Input.GetMouseButtonDown(2) && currentEndurance >= blockConsume)
            {
                m_Animator.SetTrigger("isBlock");
                ReduceEndurance(blockConsume);
            }
        }
        private void SwitchSword()
        {
            for (int i = 0; i < swordList.Count; i++)
            {
                swordList[i].SetActive(false);
            }
            swordList[currentSwordID].SetActive(true);
            collision.tag = attributesList[currentSwordID];
        }
        public void HeavyAttackJump()
        {
            IsInvincible(1);
            transform.DOMoveY(transform.position.y + heavyAttackJumpHeight, heavyAttackJumpOnceDuration);
            isHeavyAttackJump = true;
            m_Rigidbody.useGravity = false;
            m_Animator.speed = 0.5f;
        }
        public void HeavyAttackChop()
        {
            m_Animator.speed = 1f;
            transform.DOMoveY(transform.position.y - heavyAttackJumpHeight, heavyAttackJumpOnceDuration / 4);
            StartCoroutine(WaitForChopDown());
        }
        private IEnumerator WaitForChopDown()
        {
            yield return new WaitForSecondsRealtime(heavyAttackJumpOnceDuration / 4);
            myImpulse.GenerateImpulse();
            Instantiate(rockExplosionEffect, transform.position + transform.up * 0.1f, Quaternion.identity);
            m_Rigidbody.useGravity = true;
            isHeavyAttackJump = false;
            IsInvincible(0);
        }
        public void SlashAudio(int count)
        {
            AudioManager.Instance.SlashAudio(count);
        }
        public void ReduceEndurance(float consume)
        {
            currentEndurance -= consume;
        }
        private void ReduceMomentum(float value)
        {
            Main.Manager.GameManager.Instance.PlayerData.Momentum -= value;
        }
        public void ColliderSwitch(int switchCount)
        {
            if (switchCount > 0)
            {
                collision.SetActive(true);
                weaponTrailList[currentSwordID].SetActive(true);
                // meleeWeaponTrail.Emit = true;
                /*Instantiate(
                    slashEffectList[switchCount - 1],
                    collision.transform.position,
                    Quaternion.Euler(60, 0, 0)
                );*/
            }
            else
            {
                // meleeWeaponTrail.Emit = false;
                collision.SetActive(false);
                blockCollision.SetActive(false);
                weaponTrailList[currentSwordID].SetActive(false);
            }
        }
        public void BlockColliderSwitch(int switchCount)
        {
            if (switchCount > 0)
                blockCollision.SetActive(true);
            else
                blockCollision.SetActive(false);
        }
        private void TryZoom()
        {
            if (ActiveAbility == null)
            {
                isZomming = true;
            }
            else
            {
                if (ActiveAbility.AllowCameraZoom)
                {
                    isZomming = true;
                }
                else
                {
                    isZomming = false;
                }
            }
        }

        #region Movement methods

        public Vector3 FreeMoveDirection
        {
            get
            {
                Vector3 m_FreeMoveDirection = InputManager.RelativeInput;

                // convert the world relative moveInput vector into a local-relative
                // turn amount and forward amount required to head in the desired
                // direction.
                if (m_FreeMoveDirection.magnitude > 1f)
                    m_FreeMoveDirection.Normalize();

                m_FreeMoveDirection = transform.InverseTransformDirection(m_FreeMoveDirection);
                return m_FreeMoveDirection;
            }
        }

        /// <summary>
        /// Calculate movement parameters
        /// </summary>
        public void CalculateMoveVars()
        {
            if (!IsGrounded)
                return;

            Vector3 m_StrafeDirection = InputManager.RelativeInput;

            if (m_StrafeDirection.magnitude > 1f)
                m_StrafeDirection.Normalize();

            m_VerticalAmount = Vector3.Dot(m_StrafeDirection, transform.forward);
            m_HorizontalAmount = Vector3.Dot(m_StrafeDirection, transform.right);

            m_TurnAmount = Mathf.Atan2(FreeMoveDirection.x, FreeMoveDirection.z);
            //Run(修改程式)
            if (InputManager.runButton.WasPressed && FreeMoveDirection.magnitude > 0.1f)
                isRunning = true;
            else if (FreeMoveDirection.magnitude < 0.1f)
                isRunning = false;

            if (
                FreeOnMove(InputManager.RelativeInput) /*|| !IsGrounded*/
            )
            {
                m_ForwardAmount = FreeMoveDirection.z;
                if (isRunning)
                    m_ForwardAmount = Mathf.Lerp(m_ForwardAmount * 2, 3f, Time.fixedDeltaTime);
            }
            else
            {
                isRunning = false;
                m_ForwardAmount = 0;
            }
        }

        /// <summary>
        /// Send movement parameters to animator
        /// </summary>
        public void UpdateMovementAnimator(float dampTime = 0.1f)
        {
            // Set animator parameters of movement
            m_AnimatorManager.SetForwardParameter(m_ForwardAmount / 2, dampTime);
            m_AnimatorManager.SetTurnParameter(m_TurnAmount, dampTime);
            m_AnimatorManager.SetVerticallParameter(m_VerticalAmount, dampTime);
            m_AnimatorManager.SetHorizontalParameter(m_HorizontalAmount, dampTime);
        }

        /// <summary>
        /// Rotates the character to direction of movement
        /// </summary>
        public void RotateToDirection(float stationarySpeed, float movingTurnSpeed)
        {
            // help the character turn faster (this is in addition to root rotation in the animation)
            float turnSpeed = Mathf.Lerp(stationarySpeed, movingTurnSpeed, m_ForwardAmount);
            transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
        }

        /// <summary>
        /// Rotates the character to direction of movement
        /// </summary>
        public void RotateToDirection()
        {
            RotateToDirection(m_StationaryTurnSpeed, m_MovingTurnSpeed);
        }

        /// <summary>
        /// Rotate character to get same forward direction from camera
        /// </summary>
        public void RotateByCamera()
        {
            Vector3 velocity = Vector3.zero;
            Vector3 CamForward = Vector3
                .Scale(Camera.main.transform.forward, new Vector3(1, 0, 1))
                .normalized;
            transform.forward = Vector3.SmoothDamp(
                transform.forward,
                CamForward,
                ref velocity,
                0.02f
            );
        }

        // Check if character can walk on desired direction
        private bool FreeOnMove(Vector3 direction)
        {
            Vector3 p1 = transform.position + Vector3.up * (m_Capsule.radius * 2);
            Vector3 p2 = transform.position + Vector3.up * (m_Capsule.height - m_Capsule.radius);

            RaycastHit[] hits = Physics.CapsuleCastAll(
                p1,
                p2,
                m_Capsule.radius,
                direction,
                m_Rigidbody.velocity.sqrMagnitude * Time.fixedDeltaTime + 0.25f,
                GroundMask,
                QueryTriggerInteraction.Ignore
            );
            foreach (RaycastHit hit in hits)
            {
                if (
                    hit.normal.y <= Mathf.Cos(MaxAngleSlope * Mathf.Deg2Rad)
                    && hit.collider.tag != "Player"
                )
                    return false;
            }

            return true;
        }

        #endregion

        /// <summary>
        /// Method called by any ability to try enter ability
        /// </summary>
        /// <param name="ability"></param>
        public void OnTryEnterAbility(ThirdPersonAbility ability)
        {
            if (m_ActiveAbility == null)
                EnterAbility(ability);
            else
            {
                // Check if new ability has priority above current ability
                foreach (ThirdPersonAbility stopAbility in ability.IgnoreAbilities)
                {
                    if (stopAbility == m_ActiveAbility)
                        EnterAbility(ability);
                }
            }
        }

        /// <summary>
        /// Method that enter an ability. Can be also called to force any ability to enter
        /// </summary>
        /// <param name="ability"></param>
        public void EnterAbility(ThirdPersonAbility ability, bool forceAbility = false)
        {
            ExitActiveAbility();

            m_ActiveAbility = ability;

            m_ActiveAbility.OnEnterAbility();
            m_Animator.applyRootMotion = m_ActiveAbility.UseRootMotion;

            OnAnyAbilityEnters.Invoke();
            UpdatePositionOnMovableObject(null);
            OnAbilityChanged?.Invoke(ability);
        }

        /// <summary>
        /// Method that exits an ability.
        /// </summary>
        /// <param name="ability"></param>
        public void ExitAbility(ThirdPersonAbility ability)
        {
            if (m_ActiveAbility == ability)
            {
                LastAbility = m_ActiveAbility;
                m_ActiveAbility = null;

                if (ability.Active)
                    ability.OnExitAbility();

                m_Capsule.sharedMaterial = capsuleOriginalMaterial;
                OnAnyAbilityExits.Invoke();
            }
        }

        /// <summary>
        /// Force current active ability to exit
        /// </summary>
        /// <param name="ability"></param>
        public void ExitActiveAbility()
        {
            if (m_ActiveAbility != null)
            {
                LastAbility = m_ActiveAbility;
                if (m_ActiveAbility.Active)
                    m_ActiveAbility.OnExitAbility();

                m_ActiveAbility = null;

                m_Capsule.sharedMaterial = capsuleOriginalMaterial;
                OnAnyAbilityExits.Invoke();
            }
        }

        // Called to apply root motion
        private void OnAnimatorMove()
        {
            // Vars that control root motion
            bool useRootMotion = false;
            bool verticalMotion = false;
            bool rotationMotion = false;
            Vector3 multiplier = Vector3.one;

            // Check if some ability is activated
            if (m_ActiveAbility != null)
            {
                useRootMotion = m_ActiveAbility.UseRootMotion;
                verticalMotion = m_ActiveAbility.UseVerticalRootMotion;
                rotationMotion = m_ActiveAbility.UseRotationRootMotion;
                multiplier = m_ActiveAbility.RootMotionMultiplier;
            }

            if (Mathf.Approximately(Time.deltaTime, 0f) || !useRootMotion)
            {
                return;
            } // Conditions to avoid animation root motion

            Vector3 delta = m_Animator.deltaPosition;

            if (m_MovementAxis == MoveOnAxis.YZ)
            {
                delta.x = 0;
            }
            else if (m_MovementAxis == MoveOnAxis.XY)
            {
                delta.z = 0;
            }

            delta = transform.InverseTransformVector(delta);
            delta = Vector3.Scale(delta, multiplier);
            delta = transform.TransformVector(delta);

            Vector3 vel = (delta) / Time.deltaTime; // Get animator movement

            if (!verticalMotion)
                vel.y = m_Rigidbody.velocity.y; // Preserve vertical velocity

            m_Rigidbody.velocity = vel; // Set character velocity

            Vector3 deltaRot = m_Animator.deltaRotation.eulerAngles;

            if (rotationMotion)
                transform.rotation *= Quaternion.Euler(deltaRot);
        }

        /// <summary>
        /// Execute an animation event called by animation
        /// </summary>
        /// <param name="eventName"></param>
        public void ExecuteAnimationEvent(string eventName)
        {
            GlobalEvents.ExecuteEvent(eventName, gameObject, null);
        }

        /// <summary>
        /// Checks ground bellow character
        /// </summary>
        private void CheckGround()
        {
            if (GroundCheckDistance > 0.05f)
            {
                Vector3 Origin = transform.position + Vector3.up;

                if (
                    Physics.SphereCast(
                        Origin,
                        m_Capsule.radius,
                        Vector3.down,
                        out m_GroundHit,
                        2f + m_GroundCheckDistance,
                        m_GroundMask,
                        QueryTriggerInteraction.Ignore
                    )
                )
                {
                    float distance = transform.position.y - m_GroundHit.point.y;

                    if (distance > -m_GroundCheckDistance && distance <= m_GroundCheckDistance)
                    {
                        if (m_GroundHit.normal.y > Mathf.Cos(m_MaxAngleSlope * Mathf.Deg2Rad)) // Calculate the angle of the ground. If it's higher than maxSlope, don't be grounded
                        {
                            if (!IsGrounded)
                            {
                                if (m_Rigidbody.velocity.y < -6)
                                    OnGrounded.Invoke();

                                m_LastGroundPos = m_GroundHit.transform.position;
                                m_LastAngle = m_GroundHit.transform.rotation.eulerAngles.y;
                            }

                            IsGrounded = true;
                            GroundNormal = m_GroundHit.normal;
                            GroundHitInfo = m_GroundHit;
                            return;
                        }
                        else
                        {
                            Origin += m_Rigidbody.velocity.normalized * (m_Capsule.radius + 0.05f);
                            if (
                                Physics.Raycast(
                                    Origin,
                                    Vector3.down,
                                    out m_GroundHit,
                                    2f + m_GroundCheckDistance,
                                    m_GroundMask,
                                    QueryTriggerInteraction.Ignore
                                )
                            )
                            {
                                distance = transform.position.y - m_GroundHit.point.y;

                                if (
                                    distance > -m_GroundCheckDistance
                                    && distance <= m_GroundCheckDistance
                                )
                                {
                                    if (
                                        m_GroundHit.normal.y
                                        > Mathf.Cos(m_MaxAngleSlope * Mathf.Deg2Rad)
                                    ) // Calculate the angle of the ground. If it's higher than maxSlope, don't be grounded
                                    {
                                        if (!IsGrounded)
                                        {
                                            if (m_Rigidbody.velocity.y < -6)
                                                OnGrounded.Invoke();

                                            m_LastGroundPos = m_GroundHit.transform.position;
                                            m_LastAngle = m_GroundHit
                                                .transform
                                                .rotation
                                                .eulerAngles
                                                .y;
                                        }

                                        IsGrounded = true;
                                        GroundNormal = m_GroundHit.normal;
                                        GroundHitInfo = m_GroundHit;
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (isHeavyAttackJump)
                IsGrounded = true;
            else
                IsGrounded = false;
            GroundNormal = Vector3.up;
        }

        /// <summary>
        /// Method to make gravity more realistic on Jump
        /// </summary>
        void HandleAir()
        {
            if (!m_Rigidbody.useGravity)
            {
                GroundCheckDistance = m_GroundCheckDistance;
                return;
            } // Don't apply force if gravity is not being applied.

            GroundCheckDistance = m_Rigidbody.velocity.y < 2 ? m_GroundCheckDistance : 0.01f; // change ground distance to allow Jump
        }

        /// <summary>
        /// Method that restrict rigidbody velocity to avoid character goes up during a move
        /// </summary>
        void HandleGrounded()
        {
            UpdatePositionOnMovableObject(m_GroundHit.transform);

            if (!m_Rigidbody.useGravity)
            {
                return;
            } // Uses only with gravity applied and idle and walking states

            Vector3 vel = m_Rigidbody.velocity;
            vel.y = Mathf.Clamp(vel.y, -50, 0); // Avoid character goes up

            m_Rigidbody.velocity = vel;
        }

        private Vector3 m_LastGroundPos = Vector3.zero;
        private float m_LastAngle = 0;
        private Transform m_CurrentTarget = null;
        public Vector3 DeltaPos { get; private set; }
        public float DeltaYAngle { get; private set; }

        public void UpdatePositionOnMovableObject(Transform target)
        {
            if (target == null)
            {
                m_CurrentTarget = null;
                return;
            }

            if (m_CurrentTarget != target)
            {
                m_CurrentTarget = target;

                DeltaPos = Vector3.zero;
                DeltaYAngle = 0;
            }
            else
            {
                DeltaPos = target.transform.position - m_LastGroundPos;
                DeltaYAngle = target.transform.rotation.eulerAngles.y - m_LastAngle;

                Vector3 direction = transform.position - target.transform.position;
                direction.y = 0;

                float FinalAngle =
                    Vector3.SignedAngle(Vector3.forward, direction.normalized, Vector3.up)
                    + DeltaYAngle;

                float xMult = Vector3.Dot(Vector3.forward, direction.normalized) > 0 ? 1 : -1;
                float zMult = Vector3.Dot(Vector3.right, direction.normalized) > 0 ? -1 : 1;

                float cosine = Mathf.Abs(Mathf.Cos(FinalAngle * Mathf.Deg2Rad));
                Vector3 deltaRotPos =
                    new Vector3(
                        cosine * xMult,
                        0,
                        Mathf.Abs(Mathf.Sin(FinalAngle * Mathf.Deg2Rad)) * zMult
                    ) * Mathf.Abs(direction.magnitude);

                DeltaPos += deltaRotPos * (DeltaYAngle * Mathf.Deg2Rad);
            }

            if (DeltaPos.magnitude > 3f)
                DeltaPos = Vector3.zero;

            transform.position += DeltaPos;
            transform.Rotate(0, DeltaYAngle, 0);

            m_LastGroundPos = target.transform.position;
            m_LastAngle = target.transform.rotation.eulerAngles.y;
        }

        /// <summary>
        /// Scale capsule collider
        /// </summary>
        /// <param name="height">How much to scale (Uses initial dimension as reference) </param>
        public void ChangeCapsuleSize(float height)
        {
            Vector3 start = GroundPoint();
            RaycastHit hit;
            if (
                Physics.SphereCast(
                    start,
                    m_Capsule.radius,
                    Vector3.up,
                    out hit,
                    height,
                    GroundMask,
                    QueryTriggerInteraction.Ignore
                )
            )
                height = hit.distance + 0.05f;

            m_Capsule.center = height * 0.5f * Vector3.up;
            m_Capsule.radius =
                height < _capsuleOriginRadius * 2 ? height * 0.5f : _capsuleOriginRadius;
            m_Capsule.height = height;
        }

        /// <summary>
        /// Return the ground position
        /// </summary>
        /// <returns></returns>
        public Vector3 GroundPoint()
        {
            Vector3 start = transform.position + Vector3.up * CapsuleOriginalHeight;
            RaycastHit hit;
            if (
                Physics.SphereCast(
                    start,
                    0.1f,
                    Vector3.down,
                    out hit,
                    CapsuleOriginalHeight * 1.5f,
                    m_GroundMask,
                    QueryTriggerInteraction.Ignore
                )
            )
                return hit.point;

            return Vector3.zero;
        }

        public RaycastHit GetScreenHitPoint(
            Transform camera,
            float maxDistance,
            LayerMask targetMask,
            float spread
        )
        {
            Vector3 origin = camera.position;
            origin += camera.right * UnityEngine.Random.Range(-spread, spread);
            origin += camera.up * UnityEngine.Random.Range(-spread, spread);

            RaycastHit hit;
            Physics.Raycast(
                origin,
                camera.forward,
                out hit,
                maxDistance,
                targetMask,
                QueryTriggerInteraction.Collide
            );
            //Physics.SphereCast(aimRay, 0.1f, out hit, maxDistance, targetMask, QueryTriggerInteraction.Collide);

            return hit;
        }

        public RaycastHit GetScreenHitPoint(
            Transform camera,
            float maxDistance,
            LayerMask targetMask
        )
        {
            return GetScreenHitPoint(camera, maxDistance, targetMask, 0);
        }

        public void FreeTrue()
        {
            free = true;
        }

        public void FreeFalse()
        {
            free = false;
        }
        public void SetShutDown(int arg)
        {
            if (arg == 0)
                ShutDown = false;
            else
                ShutDown = true;
        }
        public void SetTimeScale(float scale)
        {
            Time.timeScale = scale;
        }
        private void LockRotation(Quaternion lockRotation)
        {
            transform.rotation = lockRotation;
        }
        public void IsInvincible(int invincible)
        {
            if (invincible == 1)
                EventManager.Instance.DispatchEvent(EventDefinition.eventPlayerInvincible, true);
            else
                EventManager.Instance.DispatchEvent(EventDefinition.eventPlayerInvincible, false);
        }
        private void EventPlayerCantMove(params object[] args)
        {
            m_Animator.SetBool("Standby", true);
            PlayerCantMove((int)args[0]);
        }
        public void PlayerCantMove(int cantMove)
        {
            if (cantMove == 1)
            {
                Main.Manager.GameManager.Instance.PlayerCantMove = true;
                Camera.main.GetComponent<CinemachineBrain>().enabled = false;
                m_Rigidbody.constraints |= RigidbodyConstraints.FreezePositionX;
                m_Rigidbody.constraints |= RigidbodyConstraints.FreezePositionY;
                m_Rigidbody.constraints |= RigidbodyConstraints.FreezePositionZ;
                m_InputManager.enabled = false;
                //EventManager.Instance.DispatchEvent(EventDefinition.eventPlayerCantMove, true);
            }
            else
            {
                Main.Manager.GameManager.Instance.PlayerCantMove = false;
                m_Animator.SetBool("Standby", false);
                Camera.main.GetComponent<CinemachineBrain>().enabled = true;
                m_Rigidbody.constraints &= ~RigidbodyConstraints.FreezePositionX;
                m_Rigidbody.constraints &= ~RigidbodyConstraints.FreezePositionY;
                m_Rigidbody.constraints &= ~RigidbodyConstraints.FreezePositionZ;
                m_InputManager.enabled = true;
                //EventManager.Instance.DispatchEvent(EventDefinition.eventPlayerCantMove, false);
            }
        }
        public void ResetIsHited()
        {
            m_Animator.ResetTrigger("isHited");
        }

        public void EndNotify()
        {
            Debug.Log("我死了");
        }

        // TODO: 傳送中不能移動攻擊
        public void SceneLoadingNotify(bool loadingBool)
        {
            if (loadingBool)
            {
                m_Animator.SetBool("Standby", true);
                m_Animator.SetFloat("Forward", 0);
                m_Animator.SetFloat("Vertical", 0);
                m_Animator.SetFloat("Horizontal", 0);
                m_Animator.SetInteger("AttackMode", 0);
                ShutDown = true;
            }
            else
            {
                ShutDown = false;
                m_Animator.SetBool("Standby", false);
            }
        }
        private void EventPlayerBlock(params object[] args)
        {
            blockCollision.SetActive(false);
        }
    }
}
