using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Enums;
using _Idle.Scripts.Balance;
using _Idle.Scripts.Model.Player;
using _Idle.Scripts.Model.Unit;
using _Idle.Scripts.View.Item;
using Plugins.GeneralTools.Scripts.View;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace _Idle.Scripts.View.Unit
{
    public class UnitView : ViewWithModel<UnitModel>
    {
        [SerializeField] private NavMeshAgent _navMeshAgent;
        [SerializeField] protected Animator _animator;
        [SerializeField] private Transform _unitPoint;
        [SerializeField] private UnitType _unitType;
        [SerializeField] private Transform _pickPoint;
        [SerializeField] private Rigidbody _unitRigidbody;
        [SerializeField] private Rigidbody _physicRigidbody;
        [SerializeField] private RadialProgress _progress;
        [SerializeField] private GameObject _ragdollPlayer;
        [SerializeField] private ConfigurableJoint _physicsJoint;
        [SerializeField] private ConfigurableJoint[] _ragdollParts;
        [SerializeField] private ConfigurableJoint[] _handParts;
        [SerializeField] private SpineCollider _spineCollider;
        [SerializeField] private List<CollisionListener> _inputHitListeners;
        [SerializeField] private List<CollisionMarker> _collisionMarkers;

        [SerializeField] private List<Rigidbody> _allBodies;
        [SerializeField] private List<AnimationController> _animationControllers;
        [SerializeField] private SkinnedMeshRenderer _mesh;
        [SerializeField] private Mesh[] _skins;
        
        public Transform GrabbedPoint1;
        public Transform GrabbedPoint2;

        public List<CollisionListener> InputHitListeners => _inputHitListeners;
        public CollisionListener AttackRadiusListener;
        public SphereCollider AttackRadiusCollider;
        public AnimationEventsSender AnimationEventsSender;

        public SpineCollider SpineCollider => _spineCollider;
        public Transform UnitPoint => _unitPoint;
        public Transform PhysicsTransform => _physicsJoint.transform;
        public NavMeshAgent NavMeshAgent => _navMeshAgent;

        public SkinnedMeshRenderer[] UnitMesh = new SkinnedMeshRenderer[2];


        private JointDrive NeckJoint = new JointDrive
        {
            positionSpring = 100,
            positionDamper = 1,
            maximumForce = Mathf.Infinity
        };
        
        private JointDrive HeadJoint = new JointDrive
        {
            positionSpring = 1000,
            positionDamper = 10,
            maximumForce = 1000
        };
        
        private List<JointDrivePart> _drives = new List<JointDrivePart>
        {
            new JointDrivePart
            {
                DriveType = JointDrivePartType.Off,
                JointDrive = new JointDrive
                {
                    positionSpring = 150,
                    positionDamper = 500,
                    maximumForce = Mathf.Infinity
                }
            }, 
            new JointDrivePart
            {
                DriveType = JointDrivePartType.Low,
                JointDrive = new JointDrive
                {
                    positionSpring = 500,
                    positionDamper = 600,
                    maximumForce = Mathf.Infinity
                }
            }, 
            new JointDrivePart
            {
                DriveType = JointDrivePartType.Medium,
                JointDrive = new JointDrive
                {
                    positionSpring = 2000,
                    positionDamper = 600,
                    maximumForce = Mathf.Infinity
                }
            }, 
            new JointDrivePart
            {
                DriveType = JointDrivePartType.High,
                JointDrive = new JointDrive
                {
                    positionSpring = 12000,
                    positionDamper = 1000,
                    maximumForce = Mathf.Infinity
                }
            }, 
            new JointDrivePart
            {
                DriveType = JointDrivePartType.OnController,
                JointDrive = new JointDrive
                {
                    positionSpring = 15000,
                    positionDamper = 2000,
                    maximumForce = Mathf.Infinity
                }
            }, 
            new JointDrivePart
            {
                DriveType = JointDrivePartType.OnRagdoll,
                JointDrive = new JointDrive
                {
                    positionSpring = 20000,
                    positionDamper = 600,
                    maximumForce = Mathf.Infinity
                }
            },
            new JointDrivePart
            {
                DriveType = JointDrivePartType.CustomAttack,
                JointDrive = new JointDrive
                {
                    positionSpring = 10000,
                    positionDamper = 100,
                    maximumForce = 1000
                }
            }
        };
        
        public GameObject RightHand;
        public GameObject LeftHand;
        public Transform WeaponAnimationTarget;
        public GameObject RangeWeaponBag;
        public Transform RaycastPoint;
        public ParticleSystem TargetMark;
        public Transform RangeWeaponGrabbedPoint;

        [Space] 
        [Header("vfx")] 
        public GameObject StunEffect;

        public UnitType UnitType => _unitType;

        public RadialProgress Progress => _progress;

        public Transform PickPoint => _pickPoint;

        public Rigidbody UnitRigidbody => _unitRigidbody;
        public Rigidbody PhysicRigidbody => _physicRigidbody;

        private readonly int ANIMATION_IDLE_KEY = Animator.StringToHash("idle");
        private readonly int ANIMATION_FALL_KEY = Animator.StringToHash("Fall");
        private readonly int ANIMATION_PICKUP_UNIT_KEY = Animator.StringToHash("PickUpUnit");
        private readonly int ANIMATION_SWINGING_KEY = Animator.StringToHash("Swinging");
        private readonly int ANIMATION_MOVE_KEY = Animator.StringToHash("MoveSpeed");
        private readonly int ANIMATION_PUNCH_ANIMATION_KEY = Animator.StringToHash("PunchAnimation");
        private readonly int ANIMATION_GETUP_KEY = Animator.StringToHash("GetUp");
        private readonly int ANIMATION_PICKUP_KEY = Animator.StringToHash("pickup");
        private readonly int ANIMATION_PUNCHING_KEY = Animator.StringToHash("Punching");
        private readonly int ANIMATION_THROW_UNIT_KEY = Animator.StringToHash("ThrowUnit");
        private readonly int ANIMATION_STUNNED_KEY = Animator.StringToHash("Stunned");
        private readonly int ANIMATION_PUSH_KEY = Animator.StringToHash("Push");
        private readonly int ANIMATION_MELEE_WEAPON_ATK_KEY = Animator.StringToHash("Melee Attack");
        private readonly int ANIMATION_RANGE_WEAPON_ATK_KEY = Animator.StringToHash("Range Attack");
        private readonly int ANIMATION_RANGE_GET_KEY = Animator.StringToHash("Range Get Weapon");
        private readonly int ANIMATION_VICTORY_KEY = Animator.StringToHash("Victory");
        private readonly int ANIMATION_VICTORY2_KEY = Animator.StringToHash("Victory 2");
        private readonly int ANIMATION_DANCE_KEY = Animator.StringToHash("Dance");

        private List<int> _bodiesLayers;
        private float _angularSpeed;
        
        public override ViewWithModel<UnitModel> SetModel(UnitModel model)
        {
            _progress.StartMe();
            _physicRigidbody = _ragdollPlayer.GetComponent<Rigidbody>();
            _physicsJoint = _ragdollPlayer.GetComponent<ConfigurableJoint>();

            // var driveOnRagdoll = _drives.First(x => x.DriveType == JointDrivePartType.OnRagdoll);
            // foreach (var part in _ragdollParts)
            // {
            //     part.slerpDrive = driveOnRagdoll.JointDrive;
            // }
            SetDriveType(JointDrivePartType.Low);
            
            return base.SetModel(model);
        }

        public override void Init()
        {
            _allBodies = GetComponentsInChildren<Rigidbody>().ToList();
            _animationControllers = GetComponentsInChildren<AnimationController>().ToList();
            _collisionMarkers = GetComponentsInChildren<CollisionMarker>().ToList();
            
            _bodiesLayers = new List<int>();
            foreach (var body in _allBodies)
            {
                _bodiesLayers.Add(body.gameObject.layer);
            }
            
            _angularSpeed = UnitType == UnitType.Player 
                ? GameBalance.Instance.PlayerData.TurnSpeed
                : GameBalance.Instance.BotParam.AngularSpeed;

            base.Init();
        }

        public override void StartMe()
        {
            foreach (var collisionMarker in _collisionMarkers)
            {
                collisionMarker.UnitModel = Model;
                collisionMarker.StartMe();
            }
            
            base.StartMe();
        }

        public void ResetMass()
        {
            foreach (var joint in _ragdollParts)
            {
                joint.massScale = 1f;
                joint.connectedMassScale = 1f;
            }
            // SetDriveType(JointDrivePartType.OnController);
        }

        public void SetLayers()
        {
            foreach (var body in _allBodies)
            {
                body.gameObject.layer = 6;
            }
        }

        public void ResetLayers()
        {
            for (var i = 0; i < _allBodies.Count; i++)
            {
                _allBodies[i].gameObject.layer = _bodiesLayers[i];
            }
        }

        public void SetGravity(bool value)
        {
            UnitRigidbody.useGravity = value;
            UnitRigidbody.isKinematic = !value;
            PhysicRigidbody.useGravity = value;
            PhysicRigidbody.isKinematic = !value;
            
            foreach (var body in _allBodies)
            {
                body.useGravity = value;
                // body.isKinematic = !value;
                if (!value)
                    body.gameObject.layer = 6;
            }

            // if (value)
            // {
            //     foreach (var animationController in _animationControllers)
            //     {
            //         animationController.AnimationControllerType = AnimationControllerType.Physics;
            //     }
            // }
            // else
            // {
            //     foreach (var animationController in _animationControllers)
            //     {
            //         animationController.AnimationControllerType = AnimationControllerType.Transform;
            //     }
            // }
        }

        private void Update()
        {
            PlayerGrounded();
            
            if (_ragdollPlayer.activeSelf)
            {
                PlayerAnimations();
            }
        }

        public override void FixedUpdateMe(float fixedTime)
        {
            if (Model.CurrentState == UnitState.Lifted) 
                return;
            
            if (_physicRigidbody == null)
                return;
            
            _physicRigidbody.position = _unitRigidbody.position;
            
            base.FixedUpdateMe(fixedTime);
        }

        private void PlayerAnimations()
        {
            if (RagdollMode && !CooledDown)
            {
                _animator.Play(ANIMATION_GETUP_KEY, 0);
            }
        }

        private void PlayerGrounded()
        {
            if (RagdollMode)
                return;

            JointDrivePart drive;
            // drive = _drives.First(x => x.DriveType == JointDrivePartType.OnController);
            // _physicsJoint.slerpDrive = drive.JointDrive;
            
            drive = _drives.First(x => x.DriveType == JointDrivePartType.OnRagdoll);
            _ragdollParts[1].slerpDrive = drive.JointDrive;
            _ragdollParts[2].slerpDrive = drive.JointDrive;
            _ragdollParts[3].slerpDrive = drive.JointDrive;
            _ragdollParts[4].slerpDrive = drive.JointDrive;
            _ragdollParts[5].slerpDrive = drive.JointDrive;
            _ragdollParts[6].slerpDrive = drive.JointDrive;
        }

        public void SetVelocity(Vector3 velocity)
        {
            _unitRigidbody.velocity = velocity;
        }
        
        public virtual void SetPosition(Vector3 pos)
        {
            transform.position = pos;
        }

        public virtual void SetRotation(Quaternion rotation)
        {
            // transform.rotation = rotation;
            _physicsJoint.targetRotation = Quaternion.Inverse(rotation);
        }

        public virtual void SetRotation(Quaternion rotation, float deltaTime)
        {
            //TODO: release
            _physicsJoint.targetRotation = Quaternion.Slerp(_physicsJoint.targetRotation, Quaternion.Inverse(rotation), deltaTime * _angularSpeed);
        }

        public void SetRotation(Vector3 rotation)
        {
            // transform.rotation = Quaternion.Euler(rotation);
            _physicsJoint.targetRotation = Quaternion.Euler(rotation);
        }

        public void Idle()
        {
            // if (Model.UnitType == UnitType.Player)
            // {
            //     Debug.Log("Idle");
            // }
            // Debug.Log(nameof(Idle));
            _animator.SetFloat(ANIMATION_MOVE_KEY, 0.0f);
            _animator.Play(ANIMATION_IDLE_KEY);
        }

        public void Move()
        {
            // if (Model.UnitType == UnitType.Player)
            // {
            //     Debug.Log("Move");
            // }
            _animator.SetFloat(ANIMATION_MOVE_KEY, 1.0f);
        }

        public void Punching()
        {
            // _animator.Play(ANIMATION_PUNCHING_KEY);
        }

        public void Throw()
        {
            // _animator.Play(ANIMATION_THROW_KEY);
        }

        public void ThrowUnit()
        {
            // if (Model.UnitType == UnitType.Player)
            // {
            //     Debug.Log("ThrowUnit");
            // }
            // Debug.Log(nameof(ThrowUnit));
            _animator.Play(ANIMATION_THROW_UNIT_KEY);
        }

        public void PickUp()
        {
            // if (Model.UnitType == UnitType.Player)
            // {
            //     Debug.Log("PickUp");
            // }
            // Debug.Log(nameof(PickUp));
            _animator.Play(ANIMATION_PICKUP_KEY);
        }

        public void PickUpUnit()
        {
            // if (Model.UnitType == UnitType.Player)
            // {
            //     Debug.Log("PickUpUnit");
            // }
            // Debug.Log(nameof(PickUpUnit));
            _animator.Play(ANIMATION_PICKUP_UNIT_KEY);
        }

        public void Stun()
        {
            // if (Model.UnitType == UnitType.Player)
            // {
            //     Debug.Log("Stun");
            // }
            // Debug.Log(nameof(Stun));
            _animator.Play(ANIMATION_STUNNED_KEY);
        }
        
        public void Push()
        {
            // if (Model.UnitType == UnitType.Player)
            // {
            //     Debug.Log("Push");
            // }
            // Debug.Log(nameof(Push));
            _animator.Play(ANIMATION_PUSH_KEY);
        }

        public void Fall()
        {
            // if (Model.UnitType == UnitType.Player)
            // {
            //     Debug.Log("Fall");
            // }
            // Debug.Log(nameof(Fall));
            _animator.Play(ANIMATION_FALL_KEY);
        }

        public void Swinging()
        {
            // Debug.Log(nameof(Swinging));
            _animator.Play(ANIMATION_SWINGING_KEY);
        }
        
        public void MeleeWeaponAttack()
        {
            // Debug.Log(nameof(MeleeWeaponAttack));
            _animator.Play(ANIMATION_MELEE_WEAPON_ATK_KEY);
        }
        
        public void RangeWeaponAttack()
        {
            // Debug.Log(nameof(RangeWeaponAttack));
            _animator.Play(ANIMATION_RANGE_WEAPON_ATK_KEY);
        }
        
        public void RangeGet()
        {
            // Debug.Log(nameof(RangeGet));
            _animator.Play(ANIMATION_RANGE_GET_KEY);
        }
        
        public void Victory(bool random = true)
        {
            if (random)
            {
                _animator.Play(Random.Range(0.0f, 1.0f) > 0.5f 
                    ? ANIMATION_VICTORY_KEY
                    : ANIMATION_VICTORY2_KEY,
                    0,
                    0.0f);
            }
            else
            {
                _animator.Play(ANIMATION_VICTORY_KEY, 0, 0.0f);
            }
        }

        public void Dance()
        {
            _animator.Play(ANIMATION_DANCE_KEY, 0, 0.0f);
        }

        public void SetLayerWeight(int layerIndex, float weight)
        {
            _animator.SetLayerWeight(layerIndex, weight);
        }

        public void GetRandomPunchingAnimation()
        {
            _animator.SetInteger(ANIMATION_PUNCH_ANIMATION_KEY, Random.Range(0, 3));
        }

        public void StopAnimation()
        {
            _animator.StopPlayback();
        }

        public bool IsAttack()
        {
            if (_animator.GetLayerWeight(2) > 0 || _animator.GetLayerWeight(3) > 0) return true;
            return false;
        }

        public bool RagdollMode = true;
        public bool CooledDown = false;
        public void PlayerGetUp()
        {
            //Slowly transition from ragdoll to active ragdoll
            if (!RagdollMode || CooledDown) 
                return;
            
            CooledDown = true;
            StartCoroutine(waitCoroutine());
                
            IEnumerator waitCoroutine()
            {
                yield return new WaitForSeconds(0.2f);

                JointDrivePart drive;
                // var drive = _drives.First(x => x.DriveType == JointDrivePartType.Medium);
                // foreach (ConfigurableJoint part in _ragdollParts)
                // {
                //    part.slerpDrive = drive.JointDrive;
                // }
                SetDriveType(JointDrivePartType.Medium);
                    
                drive = _drives.First(x => x.DriveType == JointDrivePartType.Off);
                _ragdollParts[7].slerpDrive = drive.JointDrive;
                _ragdollParts[8].slerpDrive = drive.JointDrive;
                _ragdollParts[9].slerpDrive = drive.JointDrive;
                _ragdollParts[10].slerpDrive = drive.JointDrive;
                _ragdollParts[11].slerpDrive = drive.JointDrive;
                _ragdollParts[12].slerpDrive = drive.JointDrive;
                _ragdollParts[13].slerpDrive = drive.JointDrive;
                _ragdollParts[14].slerpDrive = drive.JointDrive;
                    
                yield return new WaitForSeconds(0.2f);
                    
                // drive = _drives.First(x => x.DriveType == JointDrivePartType.High);
                // foreach (ConfigurableJoint part in _ragdollParts)
                // {
                //    part.slerpDrive = drive.JointDrive;  
                // }

                SetDriveType(JointDrivePartType.High);
                    
                drive = _drives.First(x => x.DriveType == JointDrivePartType.Off);
                _ragdollParts[7].slerpDrive = drive.JointDrive;
                _ragdollParts[8].slerpDrive = drive.JointDrive;
                _ragdollParts[9].slerpDrive = drive.JointDrive;
                _ragdollParts[10].slerpDrive = drive.JointDrive;
                _ragdollParts[11].slerpDrive = drive.JointDrive;
                _ragdollParts[12].slerpDrive = drive.JointDrive;
                _ragdollParts[13].slerpDrive = drive.JointDrive;
                _ragdollParts[14].slerpDrive = drive.JointDrive;
                    
                yield return new WaitForSeconds(0.2f);
                    
                drive = _drives.First(x => x.DriveType == JointDrivePartType.Low);
                _physicsJoint.slerpDrive = drive.JointDrive;
                    
                yield return new WaitForSeconds(0.2f);
                    
                drive = _drives.First(x => x.DriveType == JointDrivePartType.Medium);
                _physicsJoint.slerpDrive = drive.JointDrive;
                    
                SetDriveType(JointDrivePartType.Medium);
                // foreach (ConfigurableJoint part in _ragdollParts)
                // {
                //    part.slerpDrive = drive.JointDrive;  
                // }
                    
                yield return new WaitForSeconds(0.2f);
                    
                drive = _drives.First(x => x.DriveType == JointDrivePartType.High);
                _physicsJoint.slerpDrive = drive.JointDrive;
                    
                SetDriveType(JointDrivePartType.High);
                // foreach (ConfigurableJoint part in _ragdollParts)
                // {
                //    part.slerpDrive = drive.JointDrive;  
                // }
                    
                yield return new WaitForSeconds(0.2f);
                    
                RagdollMode = false;
                DeactivateRagdoll();
            }
        }
        
        private void DeactivateRagdoll()
        {
            SetDriveType(JointDrivePartType.OnRagdoll);
        }

        public void SetDriveType(JointDrivePartType driveType)
        {
            var drive = _drives.First(x => x.DriveType == driveType);
            foreach (var part in _ragdollParts)
            {
                part.slerpDrive = drive.JointDrive;  
            }
        }

        public void SetHandsDriveType(JointDrivePartType driveType)
        {
            var drive = _drives.First(x => x.DriveType == driveType);
            foreach (var part in _handParts)
            {
                part.slerpDrive = drive.JointDrive;  
            }
        }

        public void EnableEffect(GameEffectType effect)
        {
            switch (effect)
            {
                case GameEffectType.Stun:
                    StunEffect.SetActive(true);
                    break;
            }
        }

        public void DisableEffect(GameEffectType effect)
        {
            switch (effect)
            {
                case GameEffectType.Stun:
                    StunEffect.SetActive(false);
                    break;
            }
        }

        public void EnableTargetMark()
        {
            if (TargetMark == null)
                return;
            
            TargetMark.gameObject.SetActive(true);
            TargetMark.Play();
        }

        public void DisableTargetMark()
        {
            if (TargetMark == null)
                return;
            
            TargetMark.gameObject.SetActive(false);
        }

        private void OnCollisionEnter(Collision other)
        {
            Model.OnCollisionEnter(other);
        }

        //TODO: rework this method, get skin from GameBalance
        public void SetSkin(int id)
        {
            if (id >= _skins.Length)
                return;

            _mesh.sharedMesh = _skins[id];
        }

        public void EnableAnimator()
        {
            _animator.enabled = true;
        }

        public void DisableAnimator()
        {
            _animator.enabled = false;
        }

#if DEBUG
        [Button]
        public void FallUnit()
        {
            Model.Fall();
        }
#endif
    }
}