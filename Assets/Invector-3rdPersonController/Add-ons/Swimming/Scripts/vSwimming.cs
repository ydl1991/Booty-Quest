using UnityEngine;
using UnityEngine.Events;

namespace Invector.vCharacterController.vActions
{
    /// <summary>
    /// vSwimming Add-on
    /// On this Add-on we're locking the tpInput along with the tpMotor, tpAnimator & tpController methods to handle the Swimming behaviour.
    /// We can still access those scripts and methods, and call just what we need to use for example the FreeMovement, CameraInput, StaminaRecovery and UpdateHUD methods    
    /// This way the add-on become modular and plug&play easy to modify without changing the core of the controller. 
    /// </summary>

    [vClassHeader("Swimming Action")]
    public class vSwimming : vActionListener
    {
        #region Swimming Variables

        [Tooltip("Name of the tag assign into the Water object")]
        public string waterTag = "Water";

        [Header("Speed & Extra Options")]
        [Tooltip("Uncheck if you don't want to go under water")]
        public bool swimUpAndDown = true;
        [Tooltip("Speed to swim forward")]
        public float swimForwardSpeed = 4f;
        [Tooltip("Speed to rotate the character")]
        public float swimRotationSpeed = 4f;
        [Tooltip("Speed to swim up")]
        public float swimUpSpeed = 2f;
        [Tooltip("Speed to swim down")]
        public float swimDownSpeed = 2f;
        [Tooltip("Increase the radius of the capsule collider to avoid enter walls")]
        public float colliderRadius = .5f;
        [Tooltip("Height offset to match the character Y position")]
        public float heightOffset = 0.3f;

        [Header("Health/Stamina Consuption")]
        [Tooltip("Leave with 0 if you don't want to use stamina consuption")]
        public float stamina = 15f;
        [Tooltip("How much health will drain after all the stamina were consumed")]
        public int healthConsumption = 1;

        [Header("Particle Effects")]
        public GameObject impactEffect;
        [Tooltip("Check the Rigibody.Y of the character to trigger the ImpactEffect Particle")]
        public float velocityToImpact = -4f;
        public GameObject waterRingEffect;
        [Tooltip("Frequency to instantiate the WaterRing effect while standing still")]
        public float waterRingFrequencyIdle = .8f;
        [Tooltip("Frequency to instantiate the WaterRing effect while swimming")]
        public float waterRingFrequencySwim = .15f;
        [Tooltip("Instantiate a prefab when exit the water")]
        public GameObject waterDrops;
        [Tooltip("Y Offset based at the capsule collider")]
        public float waterDropsYOffset = 1.6f;

        [Tooltip("Debug Mode will show the current behaviour at the console window")]
        public bool debugMode;

        [Header("Inputs")]
        [Tooltip("Input to make the character go up")]
        public GenericInput swimUpInput = new GenericInput("Space", "X", "X");
        [Tooltip("Input to make the character go down")]
        public GenericInput swimDownInput = new GenericInput("LeftShift", "Y", "Y");

        private vThirdPersonInput tpInput;
        private float originalColliderRadius;
        private float timer;
        private float waterHeightLevel;
        private float originalMoveSpeed;
        private float originalRotationSpeed;
        private float waterRingSpawnFrequency;
        private bool inTheWater;

        // bools to trigger a method once on a update
        private bool triggerSwimState;
        private bool triggerUnderWater;
        private bool triggerAboveWater;

        #endregion

        public UnityEvent OnEnterWater;
        public UnityEvent OnExitWater;
        public UnityEvent OnAboveWater;
        public UnityEvent OnUnderWater;

        protected override void Start()
        {
            base.Start();
            tpInput = GetComponentInParent<vThirdPersonInput>();
            if (tpInput)
            {
                tpInput.onUpdate -= UpdateSwimmingBehavior;
                tpInput.onUpdate += UpdateSwimmingBehavior;
            }
        }

        protected virtual void UpdateSwimmingBehavior()
        {
            if (!inTheWater || tpInput.cc.customAction) return;

            UnderWaterBehaviour();
            SwimmingBehaviour();
        }

        private void SwimmingBehaviour()
        {
            // trigger swim behaviour only if the water level matches the player height + offset
            if (tpInput.cc._capsuleCollider.bounds.center.y + heightOffset < waterHeightLevel)
            {
                if (tpInput.cc.currentHealth > 0)
                {
                    if (!triggerSwimState) EnterSwimState();                // call once the swim behaviour
                    SwimUpOrDownInput();                                    // input to swin up or down
                    tpInput.SetStrafeLocomotion(false);                     // limit the player to not go on strafe mode
                    tpInput.MoveInput();                                    // update the input
                    tpInput.cc.SetAnimatorMoveSpeed(tpInput.cc.freeSpeed);  // update the animator input magnitude
                }
                else
                    ExitSwimState();                                        // use the trigger around the edges to exit by playing an animation                                     
            }
            else
                ExitSwimState();
        }

        private void UnderWaterBehaviour()
        {
            if (water)
            {
                waterHeightLevel = water.transform.position.y;
            }

            WaterRingEffect();

            if (isUnderWater)
            {
                StaminaConsumption();

                if (!triggerUnderWater)
                {
                    tpInput.cc._capsuleCollider.radius = colliderRadius;
                    triggerUnderWater = true;
                    triggerAboveWater = false;
                    OnUnderWater.Invoke();
                }
            }
            else
            {
                if (!triggerAboveWater && triggerSwimState)
                {
                    triggerUnderWater = false;
                    triggerAboveWater = true;
                    OnAboveWater.Invoke();
                }
            }
        }

        private void StaminaConsumption()
        {
            if (tpInput.cc.currentStamina <= 0)
            {
                tpInput.cc.ChangeHealth(-healthConsumption);
            }
            else
            {
                tpInput.cc.ReduceStamina(stamina, true);        // call the ReduceStamina method from the player
                tpInput.cc.currentStaminaRecoveryDelay = 0.25f;    // delay to start recovery stamina           
            }
        }

        public GameObject water;
        public override void OnActionEnter(Collider other)
        {
            if (other.gameObject.CompareTag(waterTag))
            {
                if (debugMode) Debug.Log("Player enter the Water");
                inTheWater = true;
                water = other.gameObject;
                waterHeightLevel = other.transform.position.y;
                originalMoveSpeed = tpInput.cc.moveSpeed;
                originalRotationSpeed = tpInput.cc.freeSpeed.rotationSpeed;

                if (tpInput.cc.verticalVelocity <= velocityToImpact)
                {
                    var newPos = new Vector3(transform.position.x, other.transform.position.y, transform.position.z);
                    Instantiate(impactEffect, newPos, tpInput.transform.rotation).transform.SetParent(vObjectContainer.root, true); ;
                }
            }
        }
       
        public override void OnActionExit(Collider other)
        {
            if (other.gameObject.CompareTag(waterTag))
            {
                if (debugMode) Debug.Log("Player left the Water");
                if (other.gameObject == water) water = null;
                inTheWater = false;
                ExitSwimState();
                if (waterDrops)
                {
                    var newPos = new Vector3(transform.position.x, transform.position.y + waterDropsYOffset, transform.position.z);
                    GameObject myWaterDrops = Instantiate(waterDrops, newPos, tpInput.transform.rotation) as GameObject;
                    myWaterDrops.transform.parent = transform;
                }
            }
        }

        private void EnterSwimState()
        {
            if (debugMode) Debug.Log("Player is Swimming");

            triggerSwimState = true;
            OnEnterWater.Invoke();
            tpInput.SetLockAllInput(true);
            tpInput.cc.disableCheckGround = true;
            tpInput.cc.lockSetMoveSpeed = true;
            tpInput.cc.moveSpeed = swimForwardSpeed;
            tpInput.cc.freeSpeed.rotationSpeed = swimRotationSpeed;
            ResetPlayerValues();
            tpInput.cc.animator.CrossFadeInFixedTime("Swimming", 0.25f);
            tpInput.cc._rigidbody.useGravity = false;
            tpInput.cc._rigidbody.drag = 10f;
            tpInput.cc._capsuleCollider.isTrigger = false;
        }

        private void ExitSwimState()
        {
            if (!triggerSwimState) return;
            if (debugMode) Debug.Log("Player Stop Swimming");

            triggerSwimState = false;
            OnExitWater.Invoke();
            tpInput.SetLockAllInput(false);
            tpInput.cc.disableCheckGround = false;
            tpInput.cc.lockSetMoveSpeed = false;
            tpInput.cc.moveSpeed = originalMoveSpeed;
            tpInput.cc.freeSpeed.rotationSpeed = originalRotationSpeed;
            tpInput.cc.animator.SetInteger(vAnimatorParameters.ActionState, 0);
            tpInput.cc._rigidbody.useGravity = true;
            tpInput.cc._rigidbody.drag = 0f;
        }

        private void SwimUpOrDownInput()
        {
            if (tpInput.cc.customAction) return;
            var upConditions = (((tpInput.cc._capsuleCollider.bounds.center.y + heightOffset) - waterHeightLevel) < -.2f);

            if (!swimUpAndDown)
            {
                var newPos = new Vector3(transform.position.x, waterHeightLevel, transform.position.z);
                if (upConditions) tpInput.cc.transform.position = Vector3.Lerp(transform.position, newPos, 0.5f * Time.deltaTime);
                return;
            }

            // extra rigibody up velocity                 
            if (swimUpInput.GetButton())
            {
                if (upConditions)
                {
                    var vel = tpInput.cc._rigidbody.velocity;
                    vel.y = swimUpSpeed;
                    tpInput.cc._rigidbody.velocity = vel;
                    tpInput.cc.animator.SetInteger(vAnimatorParameters.ActionState, 4);
                }

            }
            else if (swimDownInput.GetButton())
            {
                var vel = tpInput.cc._rigidbody.velocity;
                vel.y = -swimDownSpeed;
                tpInput.cc._rigidbody.velocity = vel;
                tpInput.cc.animator.SetInteger(vAnimatorParameters.ActionState, 3);
            }
            else
            {
                if (isUnderWater)
                    tpInput.cc.animator.SetInteger(vAnimatorParameters.ActionState, 2);
                else
                    tpInput.cc.animator.SetInteger(vAnimatorParameters.ActionState, 1);
            }
        }

        private void WaterRingEffect()
        {
            // switch between waterRingFrequency for idle and swimming
            if (tpInput.cc.input != Vector3.zero) waterRingSpawnFrequency = waterRingFrequencySwim;
            else waterRingSpawnFrequency = waterRingFrequencyIdle;

            // counter to instantiate the waterRingEffects using the current frequency
            timer += Time.deltaTime;
            if (timer >= waterRingSpawnFrequency)
            {
                var newPos = new Vector3(transform.position.x, waterHeightLevel, transform.position.z);
                Instantiate(waterRingEffect, newPos, tpInput.transform.rotation).transform.SetParent(vObjectContainer.root, true);
                timer = 0f;
            }
        }

        private void ResetPlayerValues()
        {
            tpInput.cc.isJumping = false;
            tpInput.cc.isSprinting = false;
            tpInput.cc.isCrouching = false;
            tpInput.cc.animator.SetFloat(vAnimatorParameters.InputHorizontal, 0);
            tpInput.cc.animator.SetFloat(vAnimatorParameters.InputVertical, 0);
            tpInput.cc.animator.SetInteger(vAnimatorParameters.ActionState, 1);
            tpInput.cc.isGrounded = true;                                         // ground the character so that we can run the root motion without any issues
            tpInput.cc.animator.SetBool(vAnimatorParameters.IsGrounded, true);    // also ground the character on the animator so that he won't float after finishes the climb animation
            tpInput.cc.verticalVelocity = 0f;
        }

        bool isUnderWater
        {
            get
            {
                if (tpInput.cc._capsuleCollider.bounds.max.y >= waterHeightLevel + 0.25f)
                    return false;
                else
                    return true;
            }
        }

        //------------------------------------------------------------------------------
        // Yuki: Update swim speed when loot
        public void UpdateSwimSpeed(float newSpeed)
        {
            swimForwardSpeed = newSpeed;
            tpInput.cc.moveSpeed = swimForwardSpeed;
        }
        //------------------------------------------------------------------------------
    }
}