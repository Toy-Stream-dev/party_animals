using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Enums;
using _Idle.Scripts.Balance;
using _Idle.Scripts.Data;
using _Idle.Scripts.Enums;
using _Idle.Scripts.Model.Components;
using _Idle.Scripts.Model.Components.Timer;
using _Idle.Scripts.Model.Item;
using _Idle.Scripts.Model.Numbers;
using _Idle.Scripts.Model.Player;
using _Idle.Scripts.UI;
using _Idle.Scripts.UI.HUD;
using _Idle.Scripts.View.Item;
using _Idle.Scripts.View.Unit;
using DG.Tweening;
using GeneralTools.Model;
using GeneralTools.Pooling;
using GeneralTools.Tools;
using GeneralTools.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace _Idle.Scripts.Model.Unit
{
    public class UnitModel : ModelWithView<UnitView>, IAnimationEventsListener
    {
        private const int LAYERS_COUNT = 7;
        
        private Vector3 _position;
        private Quaternion _lookRotation;
        private MoveComponent _moveComponent;
        [ShowInInspector]
        private AIComponent _aiComponent;

        private float _deltaTime;
        private bool _interaction;
        [ShowInInspector, ReadOnly]
        private bool _armed;
        private WeaponType _armedType;
        [ShowInInspector, ReadOnly]
        private InteractableItemModel _currentItem; //TODO: reset to null

        private GameModel _gameModel;
        private Hud _hud;
        private GameProgress _stunReset;
        private GameProgress _stunProgress;
        private GameProgress _damageProgress;
        private GameProgress _stun;
        private GameProgress _lift;

        private Progress _applyRangeDamageTimeout;

        private GameProgress _fallTime;
        
        private GameProgress _interactionProgress;
        private float _shootTimeLeft = 0;
        private Transform _target;
        [ShowInInspector, ReadOnly]
        private UnitModel _targetUnitModel;
        [ShowInInspector, ReadOnly]
        private UnitModel _liftedUnit;

        private PlayerData _playerData;
        private BotParam _botData;
        
        [ShowInInspector, ReadOnly]
        public bool BlockMovement { get; set; }
        private bool _ableDrop;

        public UnitType UnitType => View.UnitType;
        
        public bool InSearchZone { get; set; }
        public Rigidbody PhysicsBody => View.UnitRigidbody;
        
        [ShowInInspector]
        public UnitState CurrentState { get; protected set; }
        [ShowInInspector, ReadOnly]
        public UnitModel LiftedUnitOwner{ get; protected set; }

        public Vector3 RealPosition => _position;
        public InteractableItemModel CurrentItem => _currentItem;
        public bool Armed => _armed;
        public WeaponType ArmedType => _armedType;

        public NavMeshAgent NavMeshAgent => View.NavMeshAgent;

        public bool AblePickUpUnit => _liftedUnit == null;

        public event Action<UnitModel> OnPickUpUnit;
        public event Action<UnitModel> OnDie;
        public event Action OnEscapeLift;
        public event Action OnDropWeapon;
        public event Action OnWakeUp;
        public event Action OnRangeAttack;
        public event Action OnDropUnit;
        public event Action<UnitModel> OnFall;

        [ShowInInspector]
        public int UnitID { get; protected set; }

        public new UnitModel Start()
        {
            _moveComponent = AddComponent(new MoveComponent(this));
            _playerData = GameBalance.Instance.PlayerData;
            _botData = GameBalance.Instance.BotParam;
            
            _moveComponent.MoveSpeed = UnitType == UnitType.Player 
                ? _playerData.MoveSpeed
                : _botData.MoveSpeed;
            
            
            if (View.UnitType == UnitType.EnemyBase && !GameBalance.Instance.DisableAI)
            {
                _aiComponent = AddComponent(new AIComponent(this));
            }
            
            _stunReset = new GameProgress(GameParamType.Progress, _playerData.StunResetTime, false);
            _stunReset.CompletedEvent += () =>
            {
                _stunProgress.Reset();
                _stunReset.Reset();
                _stunReset.Pause();
            };
            _stunReset.Pause();
           
            _stunProgress = new GameProgress(GameParamType.Progress, UnitType == UnitType.Player 
                    ? _playerData.StunPoints
                    : _botData.StunPoints, 
                false);
            _stunProgress.CompletedEvent += () =>
            {
                SetState(UnitState.Stunned);
            };
            
            _damageProgress = new GameProgress(GameParamType.Progress, UnitType == UnitType.Player 
                    ? _playerData.FallPoint
                    : _botData.FallPoint, 
                false);
            _damageProgress.CompletedEvent += () =>
            {
                Fall();
                _damageProgress.Reset();
            };
            
            _stun = new GameProgress(GameParamType.Progress, UnitType == UnitType.Player 
                    ? _playerData.StunTime
                    : _botData.StunTime, 
                false);
            _stun.CompletedEvent += () =>
            {
                View.DisableEffect(GameEffectType.Stun);
                SetState(UnitState.Idle);
                _stunProgress.Reset();
                OnWakeUp?.Invoke();
            };
            _stun.Pause();
           
            _fallTime = new GameProgress(GameParamType.Progress, _playerData.FallTime, false);
            _fallTime.CompletedEvent += () =>
            {
                _stunProgress.Reset();
                _damageProgress.Reset();
                View.SpineCollider.Deactivate();
                //View.SpineCollider.InteractableItemView.Model.StopPickUp();
                View.DisableEffect(GameEffectType.Stun);
                SetState(UnitState.Idle);
                OnWakeUp?.Invoke();
            };

            _lift = new GameProgress(GameParamType.Progress, _playerData.LiftTime, false);
            _lift.CompletedEvent += () =>
            {
                // StartDropUnit();
                ReleaseUnit();
            };
            _lift.Pause();

            _applyRangeDamageTimeout = new Progress(_playerData.RangeDamageTimeout, false);
            
            View.AnimationEventsSender.AssignListener(this);
            
            base.Start();
            return this; 
        }

        public override BaseModel Init()
        {
            _gameModel = Models.Get<GameModel>();
            _hud = GameUI.Get<Hud>();
            
            return this;
        }
        
        public virtual UnitModel SpawnView(Transform root, bool activate = true, Predicate<UnitView> predicate = default)
        {
            base.SpawnView(root, false, activate, predicate);
            
            UnitID = Transform.GetInstanceID();

            View.AttackRadiusListener.OnOnTriggerEnter += AttackRadiusEnter;
            View.AttackRadiusListener.OnOnTriggerExit += AttackRadiusExit;
            View.AttackRadiusCollider.radius = GameBalance.Instance.AttackRadius;
            
            foreach (var inputHit in View.InputHitListeners)
            {
                inputHit.OnOnCollisionEnter += InputHit;
            }
            
            var item = _gameModel.ItemsContainer.CreateItem(View.SpineCollider.InteractableItemView);
            item.Owner = this;
            
            View.EnableAnimator();
            SetState(UnitState.Idle);
            
            //View.SetDriveType(JointDrivePartType.Low);

            return this;
        }

        public void LevelStart()
        {
            Fall();
            ShowNickname();
        }

        private Nickname _nickname; 
        public void ShowNickname()
        {
            _nickname = Pool.Pop<Nickname>(MainGame.WorldSpaceCanvas, false);
            _nickname.Init(UnitType);
            _nickname.transform.position = View.transform.position + new Vector3(0, 0.28f, 0);
        }

        public void HideNickname()
        {
            _nickname.PushToPool();
        }

        public string GetNickname()
        {
            return _nickname.NicknameText;
        }

        private void UpdateNicknamePosition()
        {
            if (_nickname == null) 
                return;
            
            _nickname.transform.position = View.transform.position + new Vector3(0, 0.28f, 0);
        }

        public void SetSkin(Mesh mesh)
        {
            foreach (var meshRenderer in View.UnitMesh)
            {
                meshRenderer.sharedMesh = mesh;
            }
        }

        [ShowInInspector]
        private Dictionary<int, UnitModel> _targets = new Dictionary<int, UnitModel>();
        private RaycastHit[] _hitResult = new RaycastHit[1];
        private Collider[] _rangeHitResult = new Collider[10];

        public Dictionary<int, UnitModel> Targets => _targets;

        private void InputHit(Collision obj)
        {
            switch (CurrentState)
            {
                case UnitState.Lifted:
                case UnitState.Fall:
                    return;
            }
            // if (/*_liftedUnit != null*/)
            //     return;
            
            if (!obj.transform.TryGetComponent<CollisionMarker>(out var marker))
                 return;
            
            if (marker == null || marker.UnitModel == null || marker.UnitModel == this || !marker.UnitModel.View.IsAttack()) 
                return;
            
            var damage = marker.ItemParams.PunchPower;
            
            var effectModel = Models.Get<GameEffectModel>();
            var contact = obj.GetContact(0);
            
            switch (marker.MarkerType)
            {
                case CollisionMarkerType.Hand:
                    if (obj.impulse.magnitude < _playerData.MinHandAttackImpulse)
                        return;
                    
                    effectModel.Play(GameEffectType.HandHit, contact.point);
                    var rndSound = Random.Range(0, 3);
                    GameSoundType sound = GameSoundType.HandHit_1;
                    switch (rndSound)
                    {
                        case 1:
                            sound = GameSoundType.HandHit_2;
                            break;
                        case 2:
                            sound = GameSoundType.HandHit_3;
                            break;
                    }
                    GameSounds.Instance.PlaySound(sound, 0.5f, Random.Range(0.6f, 1.4f));
                    // ApplyDamage(damage);
                    _damageProgress.Change(damage);
                    break;
                case CollisionMarkerType.MeleeWeapon:
                    // Debug.Log($"{obj.relativeVelocity} | {obj.impulse}");
                    effectModel.Play(GameEffectType.MeleeWeaponHit, contact.point);
                    GameSounds.Instance.PlaySound(GameSoundType.MeleeWeaponHit_1, 0.5f, Random.Range(0.8f, 1.2f));
                    
                    MeleeWeaponStrike(obj.GetContact(0).normal);
                    marker.UnitModel?.CurrentItem?.AppendMeleeStrike();
                    marker.UnitModel?.CurrentItem?.Drop(Vector3.Reflect(contact.point, contact.normal), contact.point);
                    // ApplyDamage(damage);
                    Fall();
                    break;
            }
            
            BlockMovement = false;
            CurrentItem?.DropRandom();
            _rangeTarget?.DisableTargetMark();
            StopInteractable();
            
            if (_liftedUnit != null)
            {
                ReleaseUnit();
            }
        }

        private void ApplyDamage(float damage)
        {
            _stunReset.SetValue(0);
            _stunReset.Play();
            
            if (!Has(UnitState.Fall))
            {
                _stunProgress.Change(damage);
            }
        }

        private void AttackRadiusEnter(Collider obj)
        {
            if (!obj.TryGetComponent<UnitView>(out var unit))
                return;

            if (unit.Model.UnitID.Equals(UnitID))
                return;

            _targets[unit.Model.UnitID] = unit.Model;
            unit.Model.OnDie += RemoveUnitFromTarget;

            // SetLayerWeight(_armed ? 2 : 1, 1);
            // SetState(UnitState.Attack);

            //TODO: add to List
            // Debug.Log(nameof(AttackRadiusEnter));
        }

        private void AttackRadiusExit(Collider obj)
        {
            if (!obj.TryGetComponent<UnitView>(out var unit))
                return;
            
            unit.Model.OnDie -= RemoveUnitFromTarget;
            _targets.Remove(unit.Model.UnitID);
            
            // if (_targets.Count > 0)
            //     return;
            
            // SetLayerWeight(1, 0);
            // SetLayerWeight(2, 0);
            //TODO: remove to List
            // Debug.Log(nameof(AttackRadiusExit));
        }

        private void RemoveUnitFromTarget(UnitModel unit)
        {
            unit.OnDie -= RemoveUnitFromTarget;
            _targets.Remove(unit.UnitID);
        }

        public void AfterInit()
        {
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            UpdateNicknamePosition();
            
            if (CurrentState == UnitState.Die) 
                return;
            
            _fallTime.Change(deltaTime);
            _stun.Change(deltaTime);
            _stunReset.Change(deltaTime);
            _lift.Change(deltaTime);
            _applyRangeDamageTimeout.Change(deltaTime);
            
            _deltaTime = deltaTime;
            _shootTimeLeft -= deltaTime;
            _pushTimeLeft -= deltaTime;
            _position = View.UnitRigidbody.position;
            
            SetLayerWeight(0,0);
            
            switch (CurrentState)
            {
                case UnitState.Stunned:
                case UnitState.Lift:
                case UnitState.Lifted:
                case UnitState.Fall:
                    SetLayerWeight(2, 0);
                    SetLayerWeight(3, 0);
                    SetLayerWeight(4, 0);
                    _rangeTarget?.DisableTargetMark();
                    return;
            }
            
            if (_isDisable)
                return;
            
            if (_unitThrowing)
                return;

            var hits = 0;

            if (_interaction)
            {
                _interactionProgress?.Change(deltaTime);
                SetLayerWeight(2, 0);
                SetLayerWeight(3, 0);
                SetLayerWeight(4, 0);
                return;
            }

            {
                if (CheckRangeAttackAvailable(out hits)) 
                    return;
            }

            if (_targets.Count <= 0)
            {
                SetLayerWeight(2, 0);
                View.SetHandsDriveType(JointDrivePartType.OnRagdoll);
                
                if (!_armed)
                {
                    SetLayerWeight(3, 0);
                    SetLayerWeight(4, 0);
                }
                
                return;
            }

            {
                var distance = /*_armed
                    ? GameBalance.Instance.AttackRaycastDistanceArmed
                    : */GameBalance.Instance.AttackRaycastDistanceUnarmed;

                // Debug.DrawLine(View.RaycastPoint.position,
                //     View.RaycastPoint.position + View.RaycastPoint.forward * distance,
                //     Color.red, deltaTime);


                if (_armed)
                {
                    SetLayerWeight(3, 1);
                    View.MeleeWeaponAttack();
                }
                else
                {
                    View.GetRandomPunchingAnimation();
                    SetLayerWeight(2, 1);
                    View.SetHandsDriveType(JointDrivePartType.CustomAttack);
                }
                
                hits = Physics.RaycastNonAlloc(View.RaycastPoint.position, View.RaycastPoint.forward, _hitResult,
                    distance, GameBalance.Instance.AttackLayermask);
                
                if (hits > 0)
                {
                    var unit = _hitResult[0].transform.GetComponentInParent<UnitView>();
                    if (unit == null)
                    {
                        Debug.Log($"Unit is NULL ???");
                        return;
                    }

                    if (unit.Model == this)
                    {
                        return;
                    }

                    // if (_armed)
                    // {
                    //     //View.SetDriveType(JointDrivePartType.OnController);
                    //     View.MeleeWeaponAttack();
                    //     return;
                    // }

                    if (unit.Model.Has(UnitState.Stunned))
                    {
                        if (_pushTimeLeft > 0.0f)
                            return;

                        _pushTimeLeft = GameBalance.Instance.PushInterval;
                        Push(unit.Model);
                    }
                    else if (!unit.Model.Has(UnitState.Fall))
                    {
                        SetLayerWeight(2, 1);
                    }
                }
                // else
                // {
                //     if (!_armed)
                //     {
                //         SetLayerWeight(2, 0);
                //         SetLayerWeight(3, 0);
                //         SetLayerWeight(4, 0);
                //     }
                // }
            }
        }

        public bool CheckRangeAttackAvailable(out int hits)
        {
            hits = 0;
                
            if (_armed && _armedType == WeaponType.Range)
            {
                //TODO: find target
                hits = Physics.OverlapSphereNonAlloc(Transform.position,
                    GameBalance.Instance.RangeAttackRaycastDistance,
                    _rangeHitResult, GameBalance.Instance.AttackLayermask);

                if (hits > 2) //2 - the first and second will always be the unit itself
                {
                    Collider nearest = null;
                    float min = 9999;

                    for (var i = 0; i < hits; i++)
                    {
                        if (_rangeHitResult[i].transform == Transform ||
                            _rangeHitResult[i].transform.position.y - Transform.position.y > 0.1f)
                            continue;

                        var dist = _rangeHitResult[i].transform.position - Position;
                        if (dist.sqrMagnitude > min ||
                            dist.sqrMagnitude <= GameBalance.Instance.RangeAttackMinDistance)
                            continue;

                        min = dist.sqrMagnitude;
                        nearest = _rangeHitResult[i];
                    }

                    if (nearest == null)
                        return false;

                    _rangeTarget?.DisableTargetMark();
                    _rangeTarget = nearest.transform.GetComponentInParent<UnitView>();
                    if (UnitType == UnitType.Player)
                    {
                        _rangeTarget?.EnableTargetMark();
                    }

                    if (CurrentState == UnitState.Idle)
                    {
                        LookAt(nearest.transform.position - Position);

                        //TODO: Get WeaponParams by WeaponType
                        if (_currentItem != null && _shootTimeLeft < 0.0f)
                        {
                            _target = nearest.transform;

                            _shootTimeLeft = _currentItem.Params.ShootInterval;

                            View.RangeWeaponAttack();
                            OnRangeAttack?.Invoke();

                            // Debug.DrawLine(Transform.position,
                            //     nearest.transform.position,
                            //     Color.blue, deltaTime);
                            return true;
                        }
                    }
                }
                else
                {
                    _rangeTarget?.DisableTargetMark();
                    _rangeTarget = null;
                }
            }
            else
            {
                _rangeTarget?.DisableTargetMark();
                _rangeTarget = null;
            }

            return false;
        }

        private UnitView _rangeTarget;
        private float _pushTimeLeft;
        public override void FixedUpdate(float deltaTime)
        {
            View.FixedUpdateMe(deltaTime);
            base.FixedUpdate(deltaTime);
        }

        public void SetPosition(Vector3 position)
        {
            if (UnitType == UnitType.Player)
            {
                View.SetPosition(position);
            }
            else if (UnitType == UnitType.EnemyBase)
            {
                View.SetPosition(position);
                // View.NavMeshAgent.Warp(position);
                
                if (!GameBalance.Instance.DisableAI)
                    _aiComponent.ChooseTask();
            }
        }

        public void SetRotation(Vector3 rotation)
        {
            View.SetRotation(rotation);
        }

        public void Move(Vector3 deltaPosition)
        {
            if (BlockMovement)
                return;
            
            if (CurrentState == UnitState.Stunned || CurrentState == UnitState.Fall)
            {
                SetLayerWeight(1, 0);
                return;
            }

            if (deltaPosition == Vector3.zero && _liftedUnit != null && _ableDrop)
            {
                StartDropUnit();
            }
            
            if (_unitThrowing)
            {
                SetLayerWeight(1, 0);
                return;
            }
            
            if (deltaPosition == Vector3.zero)
            {
                // View.SetVelocity(deltaPosition);
                SetLayerWeight(1, 0);
                SetState(UnitState.Idle);
                return;
            }

            if (UnitType == UnitType.Player)
                LookAt(deltaPosition);

            SetLayerWeight(1, 1);
            View.SetVelocity(deltaPosition);
            SetState(UnitState.Move); 
        }

        public void LookAt(Vector3 direction, bool force = false)
        {
            _lookRotation = Quaternion.LookRotation(direction);
            _lookRotation.x = 0.0f;
            _lookRotation.z = 0.0f;
            
            if (force)
            {
                View.SetRotation(_lookRotation);
            }
            else
            {
                View.SetRotation(_lookRotation, _deltaTime);
            }
        }
        
        public void StartInteractable(InteractableItemModel item, InteractableItemParams @params)
        {
            if (@params.IsWeapon && _armed)    
                return;
            
            if (_armed || _interaction || _liftedUnit != null)
                return;
            
            item.IsInterctable = true;
            
            _interaction = true;
            
            View.Progress.SetFullTime(@params.InteractableTime);
            View.Progress.Show();
            
            _interactionProgress = new GameProgress(GameParamType.Progress, @params.InteractableTime, false);
            _interactionProgress.UpdatedEvent += () =>
            {
                View.Progress.UpdateProgress(_interactionProgress.CurrentValue);
            };
            _interactionProgress.CompletedEvent += () =>
            {
                //TODO: release method
                
                StopInteractable();

                if (item.View != null && item.View.CollisionMarker != null)
                {
                    item.View.CollisionMarker.UnitModel = this;
                }
                
                switch (@params.ItemType)
                {
                    case InteractableItemType.Lollipop:
                    case InteractableItemType.Mailbox:
                    case InteractableItemType.Spoon:
                        LookAt(item.Position - Position, true);
                        PickUpStart(item);
                        _armedType = WeaponType.Melee;
                        break;
                    case InteractableItemType.Bomb:
                    case InteractableItemType.Cake:
                    case InteractableItemType.Tomatoes:
                        LookAt(item.Position - Position, true);
                        PickUpStart(item);
                        _armedType = WeaponType.Range;
                        break;
                    case InteractableItemType.Unit:
                        LookAt(item.Position - Position, true);
                        PickUpUnit(item);
                        _armedType = WeaponType.Unit;
                        break;
                }
                
                
                _interaction = false;
                _interactionProgress = null;
            };
        }

        private InteractableItemModel _pickupItem;
        private bool _hasJoint = false;
        public Rigidbody GrabbedObject;

        public void PickUpStart(InteractableItemModel item)
        {
            if (item.Params.IsWeapon && item.Owner != null)
                return;
            
            // Debug.Log(nameof(PickUpStart));
            _pickupItem = item;
            View.PickUp();
        }

        public bool HasItemPriority(InteractableItemModel item)
        {
            if (_pickupItem != null && _pickupItem.Priority >= item.Priority)
                return false;
            
            _pickupItem?.OnTriggerExit(View);
            _pickupItem = null;
            return true;
        }

        public void PickUpUnit(InteractableItemModel item)
        {
            // if (item.Owner != null)
            //     return;
            
            BlockMovement = true;
            _pickupItem = item;
            // item.IsInterctable = false;
            View.PickUpUnit();
            SetState(UnitState.Lift);
        }

        public void PickUpEnd(InteractableItemModel item)
        {
            if (item == null)
                return;
            
            Debug.Log(nameof(PickUpEnd));
            // DOTween.Sequence().AppendInterval(0.8f).OnComplete(() =>
            // {
            if (item.Params.IsWeapon)
            {
                _armed = true;
            }
            
            if (item.Params.ItemType != InteractableItemType.Unit)
            {
                item.Owner = this;
            }
            
            item.PickedUp();
            
            _currentItem = item;
            _currentItem.OnDrop += OnDropItem;

            var dif = Vector3.zero;
            
            if (item.Params.WeaponType == WeaponType.Melee)
            {
                _currentItem.View.ItemRigidbody.isKinematic = true;
                _currentItem.View.Collider.enabled = false;
                _currentItem.DisableMeleeWeaponCollider();

                _currentItem.View.GrabbedObject.transform.SetParent(View.RightHand.transform);
                // _currentItem.View.GrabbedObject.transform.position = View.WeaponAnimationTarget.position;

                var rot = View.WeaponAnimationTarget.rotation.eulerAngles;
                // rot.y += 180;

                _currentItem.View.GrabbedObject.transform.rotation = Quaternion.Euler(rot);

                dif = View.RightHand.transform.position - _currentItem.View.GrabbedPoint1.position;
                _currentItem.View.GrabbedObject.transform.position += dif;

                //     View.PhysicRigidbody.rotation * Quaternion.Euler(0, 180, 0);//* View.RightHand.transform.rotation * View.WeaponAnimationTarget.rotation; //Quaternion.Euler(rot);

                // SetLayerWeight(2, 1);
                // return;
                // var actrl = _currentItem.View.GrabbedObject.AddComponent<WeaponAnimationController>();
                // actrl.SetTarget(View.WeaponAnimationTarget);
                // actrl.RightHand = View.RightHand.transform;
                // actrl.LeftHand = View.LeftHand.transform;
            
                SetLayerWeight(3, 1);
                return;
            }
            if (item.Params.WeaponType == WeaponType.Range)
            {
                View.RangeWeaponBag.SetActive(true);
                
                _currentItem.View.ItemRigidbody.isKinematic = true;
                _currentItem.View.Collider.enabled = false;
                _currentItem.DisableRangeWeaponCollider();
                
                _currentItem.View.GrabbedObject.transform.SetParent(View.RightHand.transform);

                _currentItem.View.transform.rotation = Quaternion.identity;
                dif = View.RangeWeaponGrabbedPoint.position - _currentItem.View.GrabbedPoint1.position;

                _currentItem.Position += dif;
                // var fixedJoint = View.RightHand.AddComponent<FixedJoint>(); 
                // fixedJoint.breakForce = Mathf.Infinity;
                // fixedJoint.connectedBody = GrabbedObject;

                _currentItem.View.GrabbedObject.SetActive(false);
                SetLayerWeight(4, 1);
                View.RangeGet();
                return;
            }

            // if (item.Params.WeaponType == WeaponType.Range)
            // {
            //     _currentItem.View.GrabbedObject.transform.SetParent(View.RightHand.transform);
            //     
            //     var rot = View.WeaponAnimationTarget.rotation.eulerAngles;
            //     
            //     _currentItem.View.GrabbedObject.transform.rotation = Quaternion.Euler(rot);
            //     
            //     dif = View.RightHand.transform.position - _currentItem.View.GrabbedPoint1.position;
            //     _currentItem.View.GrabbedObject.transform.position += dif;
            //     
            //     SetLayerWeight(2, 1);
            // }
            
            return;
            
            if (item.Params.IsWeapon && item.Params.WeaponType == WeaponType.Melee)
            {
                dif = View.LeftHand.transform.position - item.View.GrabbedPoint2.position;
                
                item.Position += dif;
                View.LeftHand.AddComponent<FixedJoint>();
                View.LeftHand.GetComponent<FixedJoint>().breakForce = Mathf.Infinity;
                View.LeftHand.GetComponent<FixedJoint>().connectedBody = GrabbedObject;
            }
            // });
        }

        private void OnDropItem()
        {
            _currentItem.OnDrop -= OnDropItem;
            _currentItem.Owner = null;
            _currentItem = null;
            
            Unarmed();
        }

        public void Unarmed()
        {
            SetLayerWeight(2, 0);
            SetLayerWeight(3, 0);
            SetLayerWeight(4, 0);
            
            _armed = false;
            
            if (View.RightHand.gameObject.TryGetComponent<FixedJoint>(out var fixedJoint))
            {
                fixedJoint.DestroyComponent();
            }
            
            View.RangeWeaponBag.SetActive(false);
            OnDropWeapon?.Invoke();
        }

        public void StopInteractable()
        {
            _interaction = false;
            _interactionProgress = null;
            _pickupItem = null;
            View.Progress.Hide();
        }
		
        public void SetState(UnitState state)
        {
            if (CurrentState == UnitState.Die) 
                return;
            
            if (CurrentState == state)
                return;
            
            // Debug.Log($"New {Transform.name} state: {CurrentState}");
            CurrentState = state;
            
            //View.DisableEffect(GameEffectType.Stun);
            
            switch (state)
            {
                case UnitState.Idle:
                    View.Idle();
                    break;
                case UnitState.Move:
                    View.Move();
                    break;
                case UnitState.Attack:
                    View.Punching();
                    break;
                case UnitState.Stunned:
                    SetLayerWeight(1, 0);
                    SetLayerWeight(2, 0);
                    SetLayerWeight(3, 0);
                    _stun.Reset();
                    View.Stun();
                    View.EnableEffect(GameEffectType.Stun);
                    break;
                case UnitState.Lift:
                    _lift.Reset();
                    break;
                case UnitState.Lifted:
                    // View.SpineCollider.Deactivate();
                    SetLayerWeight(1, 0);
                    SetLayerWeight(2, 0);
                    SetLayerWeight(3, 0);
                    SetLayerWeight(4, 0);
                    // View.Swinging();
                    _fallTime.Pause();
                    _fallTime.SetValue(0);
                    break;
                case UnitState.Fall:
                    _fallTime.Reset();
                    _stun.Pause();
                    _stun.SetValue(0);
                    break;
            }
        }
        
        public bool Has(UnitState state)
        {
            return CurrentState == state;
        }
        
        public void SetLayerWeight(int layerIndex, float weight)
        {
            View.SetLayerWeight(layerIndex, weight);
        }

        public void ResetAllLayersWeight()
        {
            for (var i = 1; i < LAYERS_COUNT; i++)
            {
                SetLayerWeight(i, 0.0f);
            }   
        }

        public void ExecuteEvent(AnimationEventType eventType)
        {
            switch (eventType)
            {
                case AnimationEventType.Pickup:
                    PickUpEnd(_pickupItem);
                    _pickupItem = null;
                    break;
                case AnimationEventType.Throw:
                    if (_target != null)
                    {
                        _currentItem?.View.GrabbedObject.SetActive(false);
                        _currentItem.Throw(View.RightHand.transform.position, _target.position - View.RightHand.transform.position);
                    }
                    break;
                case AnimationEventType.Push:
                    if (_targetUnitModel != null)
                    {
                        //_targetUnitModel.LookAt(View.transform.position);
                        _targetUnitModel.GetPushFrom(this, View.PhysicRigidbody.transform.forward);
                        _targetUnitModel = null;
                    }
                    break;
                case AnimationEventType.MeleeWeaponAttack:
                    //TODO: release it
                    break;
                case AnimationEventType.SwingMeleeWeapon:
                    if (_currentItem != null && _currentItem.Params.IsWeapon && _currentItem.Params.WeaponType == WeaponType.Melee)
                    {
                        _currentItem.EnableMeleeWeaponCollider();
                    }
                    break;
                case AnimationEventType.EndSwingMeleeWeapon:
                    if (_currentItem != null && _currentItem.Params.IsWeapon && _currentItem.Params.WeaponType == WeaponType.Melee)
                    {
                        _currentItem.DisableMeleeWeaponCollider();
                    }
                    break;
                case AnimationEventType.RangeGet:
                    _currentItem?.View.GrabbedObject.SetActive(true);
                    break;
                case AnimationEventType.Lifting:
                    BlockMovement = false;
                    // SetState(UnitState.Lift);
                    SetLayerWeight(5, 1);
                    _liftedUnit = _pickupItem.Owner;
                    OnPickUpUnit?.Invoke(_liftedUnit);
                    _liftedUnit.BlockMovement = true;
                    _liftedUnit.SetState(UnitState.Lifted);
                    _liftedUnit.View.SetGravity(false);
                    _liftedUnit.View.SpineCollider.Deactivate();
                    _liftedUnit.View.SetParent(View.UnitPoint);
                    // _liftedUnit.SetLayerWeight(1,0);
                    _liftedUnit.SetLayerWeight(6,1);
                    _liftedUnit.View.SetLayers();

                    _liftedUnit.View.transform.localRotation = Quaternion.Euler(Vector3.zero);
                    _liftedUnit.SetRotation(Vector3.zero);
                    _liftedUnit.View.transform.localPosition = Vector3.zero;

                    _liftedUnit.LiftedUnitOwner = this;
                    
                    // var fixedJoint = View.RightHand.AddComponent<FixedJoint>(); 
                    // fixedJoint.breakForce = Mathf.Infinity;
                    // fixedJoint.connectedBody = _liftedUnit.PhysicsBody;
                    // fixedJoint = View.LeftHand.AddComponent<FixedJoint>(); 
                    // fixedJoint.breakForce = Mathf.Infinity;
                    // fixedJoint.connectedBody = _liftedUnit.PhysicsBody;
                    
                    DOTween.Sequence().AppendInterval(GameBalance.Instance.ThrowUnitDelay).OnComplete(() =>
                    {
                        _ableDrop = true;
                    });
                    break;
                case AnimationEventType.ThrowUnit:
                    EndDropUnit();
                    SetState(UnitState.Idle);
                    break;
            }
        }
        
        public void ProjectilePush(ProjectileArgs args)
        {
            if (!_applyRangeDamageTimeout.IsCompleted)
                return;
            
            _applyRangeDamageTimeout.Reset();
            
            View.UnitRigidbody.AddForce(args.Velocity.normalized * GameBalance.Instance.ProjectilePushForce, ForceMode.Impulse);
            
            CurrentItem?.DropRandom();
            _rangeTarget?.DisableTargetMark();
            StopInteractable();
            
            if (_liftedUnit != null)
            {
                ReleaseUnit();
            }
            
            ApplyDamage(args.Power);
            
            switch (args.ProjectileType)
            {
                case InteractableItemType.Bomb:
                    Models.Get<GameEffectModel>().Play(GameEffectType.BombExplosion, args.Collision.GetContact(0).point);
                    GameSounds.Instance.PlaySound(GameSoundType.BombExplosion_1, 0.5f, Random.Range(0.6f, 1.4f));
                    //TODO: if Player - do camera shake
                    break;
                case InteractableItemType.Cake:
                    Models.Get<GameEffectModel>().Play(GameEffectType.CakeExplosion, args.Collision.GetContact(0).point);
                    GameSounds.Instance.PlaySound(GameSoundType.CakeExplosion_1, 0.5f, Random.Range(0.6f, 1.4f));
                    break;
                default:
                    Models.Get<GameEffectModel>().Play(GameEffectType.RangeWeaponHit, args.Collision.GetContact(0).point);
                    break;
            }
        }

        public void Push(UnitModel unitModel)
        {
            _targetUnitModel = unitModel;
            View.Push();
        }

        /// <summary>
        /// Unit push other Unit
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="impulse"></param>
        public void GetPushFrom(UnitModel unit, Vector3 impulse)
        {
            View.UnitRigidbody.AddForce(impulse.normalized * GameBalance.Instance.PushForce, ForceMode.Impulse);
            Fall();
        }

        public void Fall()
        {
            OnFall?.Invoke(this);
            
            SetState(UnitState.Fall);
            View.Fall();
            DOTween.Sequence().AppendInterval(0.7f).OnComplete(() => View.SpineCollider.Activate());
        }

        private bool _unitThrowing;
        public void StartDropUnit()
        {
            if (_unitThrowing)
                return;
            
            _unitThrowing = true;
            _lift.Pause();
            // SetLayerWeight(2, 0);
            // SetLayerWeight(3, 0);
            // SetLayerWeight(4, 0);
            // SetLayerWeight(5, 0);
            View.ThrowUnit();
        }

        private void EndDropUnit()
        {
            SetLayerWeight(2, 0);
            SetLayerWeight(3, 0);
            SetLayerWeight(4, 0);
            SetLayerWeight(5, 0);
            if (UnitType == UnitType.Player)
            {
                _gameModel.TimeScale.SlowMotion(true);
            }
            _ableDrop = false;
            
            if (_liftedUnit != null && _liftedUnit.View != null)
            {
                _liftedUnit.View.SetParent(null);
                _liftedUnit.View.SetGravity(true);
                _liftedUnit.View.ResetLayers();
                _liftedUnit.View.DisableEffect(GameEffectType.Stun);
                var direction = View.PhysicRigidbody.transform.forward;
                direction.y = 1;
                _liftedUnit.GetDropFrom(direction, _lift.IsCompleted);
                _liftedUnit = null;
                OnDropUnit?.Invoke();
            }
            
            _lift.Reset();
            _lift.Pause();
            _unitThrowing = false;
            _pickupItem = null;
            //View.SpineCollider.InteractableItemView.Model.StopPickUp();
        }

        private void ReleaseUnit()
        {
            SetLayerWeight(2, 0);
            SetLayerWeight(3, 0);
            SetLayerWeight(4, 0);
            SetLayerWeight(5, 0);
            SetState(UnitState.Idle);
            BlockMovement = false;
            _ableDrop = false;
            
            if (_liftedUnit != null && _liftedUnit.View != null)
            {
                _liftedUnit.View.SetParent(null);
                _liftedUnit.View.SetGravity(true);
                _liftedUnit.View.ResetLayers();
                _liftedUnit.View.DisableEffect(GameEffectType.Stun);
                _liftedUnit.BlockMovement = false;
                _liftedUnit.GetDropFrom();
            }
            
            _lift.Reset();
            _lift.Pause();
            _liftedUnit = null;
            _pickupItem = null;
            OnDropUnit?.Invoke();
            _unitThrowing = false;
        }

        public void GetDropFrom(Vector3 impulse, bool zeroForce = false)
        {
            View.UnitRigidbody.AddForce(impulse.normalized * (zeroForce ? 0 : GameBalance.Instance.DropUnitForce), ForceMode.Impulse);
            
            GetDropFrom(zeroForce);
        }

        public void GetDropFrom(bool zeroForce = false)
        {
            SetLayerWeight(6, 0);
            _stunProgress.Reset();
            _damageProgress.Reset();
            _stunReset.Reset();
            _stunReset.Pause();
            Transform.rotation = Quaternion.identity;
            // SetState(UnitState.Idle);
            SetState(UnitState.Fall);
           
            if (zeroForce)
            {
                OnEscapeLift?.Invoke();
            }

            BlockMovement = false;
            // AddComponent(new TimerComponent(this, 1.0f, () =>
            // {
            //     BlockMovement = false;
            // }));
        }

        public void MeleeWeaponStrike(Vector3 impulse)
        {
            //TODO: apply melee weapon strike
            SetState(UnitState.Stunned);
            View.UnitRigidbody.AddForce(impulse.normalized * GameBalance.Instance.PushForce, ForceMode.Impulse);
        }

        public void Kill()
        {
            if (Has(UnitState.Die))
                return;
            
            _gameModel.ShowKillingInfo(new KillingInfo
            {
                Killer = LiftedUnitOwner,
                Victim = this
            });
            
            Models.Get<GameEffectModel>().Play(GameEffectType.Die, Position);
            View.DisableAnimator();
            
            SetState(UnitState.Die);
            View.SetDriveType(JointDrivePartType.Off);
            RemoveAllComponents();
            
            switch (UnitType)
            {
                case UnitType.Player:
                    DOTween.Sequence().AppendInterval(GameBalance.Instance.UnitDestroyDelay).OnComplete(() =>
                    {
                        OnDie?.Invoke(this);
                        _gameModel.PlayerContainer.Destroy();
                    });
                    break;
                case UnitType.EnemyBase:
                    DOTween.Sequence().AppendInterval(GameBalance.Instance.UnitDestroyDelay).OnComplete(() =>
                    {
                        OnDie?.Invoke(this);
                        _gameModel.UnitsContainer.Destroy(this);
                    });
                    break;
            }
            
            HideNickname();
        }

        public void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer == 13)
            {
            }

            if (Has(UnitState.Lifted) && ((1 << other.gameObject.layer) & GameBalance.Instance.GroundLayer) != 0)
            {
                Models.Get<GameEffectModel>().Play(GameEffectType.MeleeWeaponHit, other.contacts[0].point);
                Transform.rotation = Quaternion.Euler(Vector3.zero);
                View.RagdollMode = true;
                View.CooledDown = false;
                View.PlayerGetUp();
                SetState(UnitState.Idle);
                View.SpineCollider.Deactivate();
            }
            
            // Debug.Log(other.impulse.sqrMagnitude);
            // if (other.impulse.sqrMagnitude < Mathf.Epsilon)
            //     return;
            //
            // if (!other.gameObject.TryGetComponent<CollisionMarker>(out var marker))
            //     return;
            //
            //
            // switch (marker.MarkerType)
            // {
            //     case CollisionMarkerType.MeleeWeapon:
            //         MeleeWeaponStrike(other.impulse);
            //
            //         var item = other.gameObject.GetComponentInParent<InteractableItemView>();
            //         item?.Model.AppendMeleeStrike();
            //         break;
            // }
        }

        public void Destroy()
        {
            DeleteView();
            RemoveAllComponents();
        }

        public void ResetPosition()
        {
            View.transform.rotation = Quaternion.Euler(Vector3.up * 180);
            SetRotation(Vector3.up * 180);
            ResetAllLayersWeight();
            View.RangeWeaponBag.SetActive(false);
            View.Idle();
            SetState(UnitState.Idle);
        }

        private bool _isDisable;
        public void Disable()
        {
            RemoveAllComponents();
            _isDisable = true;
        }
    }
}