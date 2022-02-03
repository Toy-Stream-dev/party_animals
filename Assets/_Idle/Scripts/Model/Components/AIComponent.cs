using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using _Idle.Scripts.Balance;
using _Idle.Scripts.Enums;
using _Idle.Scripts.Model.Item;
using _Idle.Scripts.Model.Unit;
using _Idle.Scripts.View.Item;
using _Idle.Scripts.View.Level;
using _Idle.Scripts.View.Unit;
using DG.Tweening;
using GeneralTools.Model;
using GeneralTools.Tools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Android;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace _Idle.Scripts.Model.Components
{
    public class AIComponent : BaseComponent<UnitModel>
    {
        [ShowInInspector]
        private AIState _state;
        private BotParam _botParam;
        private GameModel _gameModel;
        [ShowInInspector]
        private InteractableItemModel _targetItem;
        [ShowInInspector]
        private InteractableItemModel _currentItem;
        [ShowInInspector]
        private UnitModel _targetEnemy;
        private ThrowPoint _targetThrowPoint;
        private float _distanceAimCoefficient;

        private NavMeshAgent _agent;
        private AIState _currentState;

        private MoveComponent _moveComponent;
        
        private Sequence _chooseTaskSequence;
        private Sequence _pickUpTaskSequence;
        
        public AIComponent(UnitModel model) : base(model)
        {
            _botParam = GameBalance.Instance.BotParam;
            _gameModel = Models.Get<GameModel>();
            model.OnPickUpUnit += OnPickUpUnit;
            model.OnDropUnit += OnDropUnit;
            model.OnDie += OnDie;
            model.OnDropWeapon += OnDropWeapon;
            model.OnWakeUp += OnWakeUp;
            model.OnRangeAttack += OnRangeAttack;
            //_agent = model.View.NavMeshAgent;

            _moveComponent = model.Get<MoveComponent>();
            
            _chooseTaskSequence = DOTween.Sequence().AppendInterval(1).OnComplete(() => ChooseTask()).Pause();
            _pickUpTaskSequence = DOTween.Sequence().AppendInterval(1f).OnComplete(() =>
            {
                if (_currentItem == null)
                {
                    // FindEnemy();
                    ChooseTask();
                }
            }).Pause();
        }

        private void OnDropWeapon()
        {
            if (FindEnemy())
            {
                SetState(AIState.FindEnemy);
            }
        }

        private void OnPickUpUnit(UnitModel unitModel)
        {
            if (_targetEnemy != null)
            {
                _targetEnemy.OnDie -= OnTargetEnemyDie;
                _targetEnemy = null;
            }
            var throwPoints = _gameModel.LevelLoader.CurrentLevel.View.ThrowPoints;
            float minDistance = float.MaxValue;
            var unitPosition = Model.View.transform.position;
            ThrowPoint closestPoint = null;
            foreach (var throwPoint in throwPoints)
            {
                var distance = (unitPosition - throwPoint.transform.position).sqrMagnitude;
                if (distance < minDistance)
                {
                    closestPoint = throwPoint;
                    minDistance = distance;
                }
            }

            if (closestPoint == null)
            {
                Debug.LogError($"Can`t find closest throw point");
            }
            else
            {
                unitModel.OnEscapeLift += OnEnemyEscapeLift;
                _targetThrowPoint = closestPoint;
                SetState(AIState.GoToBorder);
            }
        }

        private void OnDropUnit()
        {
            FindEnemy();
        }

        private void OnEnemyEscapeLift()
        {
            if (_targetEnemy != null)
            {
                _targetEnemy.OnEscapeLift -= OnEnemyEscapeLift;   
            }
            if(FindEnemy())
            {
                SetState(AIState.FindEnemy);   
            }
        }

        private void OnDie(UnitModel model)
        {
            model.OnDie -= OnDie;
            if (_targetEnemy != null)
            {
                _targetEnemy.OnDie -= OnTargetEnemyDie;
                _targetEnemy.OnEscapeLift -= OnEnemyEscapeLift;
                _targetEnemy.OnDropWeapon -= OnDropWeapon;
                _targetEnemy.OnWakeUp -= OnWakeUp;
                _targetEnemy.OnRangeAttack -= OnRangeAttack;
                Model.OnDropUnit -= OnDropUnit;
            }

            if (_targetItem != null)
            {
                _targetItem.SetOwner -= OnOwnedWeapon;
            }
            SetState(AIState.None);
        }

        private void OnWakeUp()
        {
            FindEnemy();
        }

        private void OnRangeAttack()
        {
            _distanceAimCoefficient -= _botParam.StepPercentAfterShot * _distanceAimCoefficient;
        }

        public void ChooseTask(AITaskType aiTaskType = default)
        {
            if (aiTaskType == default)
            {
                aiTaskType = _botParam.Tasks.GetRandomItem().Type;
            }
            
            switch (aiTaskType)
            {
                case AITaskType.Walk:
                    //TODO: release it, add to Balance
                    ChooseTask();
                    break;
                case AITaskType.FindWeapon:
                    FindWeapon();
                    break;
                case AITaskType.FindEnemy:
                    if (FindEnemy())
                    {
                        SetState(AIState.FindEnemy);   
                    }
                    break;
            }
        }

        private bool FindEnemy(UnitModel exclusion = default)
        {
            if (_targetEnemy != null)
            {
                _targetEnemy.OnDie -= OnTargetEnemyDie;
                _targetEnemy = null;   
            }
            
            var enemies = new List<UnitModel>(_gameModel.UnitsContainer.GetAll());
            if (_gameModel.Player != null)
            {
                enemies.Add(_gameModel.Player);
            }

            if (exclusion != default)
            {
                enemies.Remove(exclusion);
            }
            enemies.Remove(Model);
            
            if (enemies.Count != 0)
            {
                // _targetEnemy = FindClosestEnemy(enemies);   
                _targetEnemy = enemies.RandomValue();
            }
            
            if (_targetEnemy == null)
            {
                SetState(AIState.None);
                return false;
            }
            
            _targetEnemy.OnDie += OnTargetEnemyDie;
            return true;
        }

        private void FindWeapon()
        {
            var weapons =
                _gameModel.ItemsContainer.GetAll().Where(item => item.Params.IsWeapon && item.Owner == null && !item.IsInterctable).ToList();
            // _targetItem = FindClosestWeapon(weapons);
            _targetItem = weapons.RandomValue();
            
            if (_targetItem == null || Model.Armed)
            {
                Debug.Log("Can`t find weapon");
                FindEnemy();
            }
            else
            {
                _targetItem.SetOwner += OnOwnedWeapon;
                SetState(AIState.FindWeapon);
            }
        }

        private void OnTargetEnemyDie(UnitModel model)
        {
            SetState(AIState.None);
            model.OnDie -= OnTargetEnemyDie;
            if(FindEnemy(_targetEnemy))
            {
                SetState(AIState.FindEnemy);   
            }
        }

        private Vector3 GetPath(Vector3 targetPosition, out float distance)
        {
            var vector = Vector3.zero;
            distance = 100;
            
            NavMesh.CalculatePath(Model.Position, targetPosition, NavMesh.AllAreas, _path);
            _pathPointIndex = 1;
            
            
#if DEBUG
            for (int i = 0; i < _path.corners.Length - 1; i++)
                Debug.DrawLine(_path.corners[i], _path.corners[i + 1],Color.red);
#endif
                    
            if (_path.corners.Length > 0)
            {
                vector = _path.corners[_pathPointIndex] - Model.Position;
//                 var stepCount = 10;
//                 vector /= stepCount;
//                 for (var i = 0; i < stepCount - 1; i++)
//                 {
// #if DEBUG
//                     // for (int i = 0; i < _path.corners.Length - 1; i++)
//                     Debug.DrawLine(vector, vector * (i + 1), Random.ColorHSV());
// #endif
//                 }
            }

            distance = (Model.Position - targetPosition).sqrMagnitude;
            
            vector.y = 0;
            
            return vector;
        }

        private readonly NavMeshPath _path = new NavMeshPath();
        private int _pathPointIndex;
        private float distance = 0;
        
        public override void FixedUpdate(float deltaTime)
        {
            var vector = Vector3.zero;
            
            switch (_state)
            {
                case AIState.FindWeapon:
                    if (!Model.Has(UnitState.Idle) && !Model.Has(UnitState.Move))
                        return;
                    
                    if (_targetItem == null)
                    {
                        ChooseTask();
                        return;
                    }

                    vector = GetPath(_targetItem.Position, out distance);
                    
                    if (vector.sqrMagnitude <= _botParam.DistanceToItem)
                    {
                        SetState(AIState.TryPickUp);
                        _moveComponent.Move(Vector3.zero, deltaTime);
                        return;
                    }
                    
                    if (Model.Has(UnitState.Idle) || Model.Has(UnitState.Move))
                    {
                        Model.LookAt(vector);
                        _moveComponent.Move(Model.View.PhysicsTransform.forward, deltaTime);
                    }
                    
                    break;
                case AIState.FindEnemy:
                    if (_targetEnemy == null || _targetEnemy.View == null || _targetEnemy.Targets.Count > _botParam.TargetCountLimit)
                    {
                        ChooseTask();
                        return;
                    }
                    
                    vector = GetPath(_targetEnemy.Position, out distance);
                    
                    if (distance > _botParam.DistanceToEnemy)
                    {
                        if (!Model.CheckRangeAttackAvailable(out var hits))
                        {
                            _moveComponent.Move(Model.View.PhysicsTransform.forward, deltaTime);
                            // Model.NavMeshAgent.SetDestination(_targetEnemy.Position);
                        }
                    }
                    else
                    {
                        _moveComponent.Move(Vector3.zero, deltaTime);
                    }
                    
                    if (Model.Has(UnitState.Idle) || Model.Has(UnitState.Move))
                    {
                        Model.LookAt(vector);
                    }
                    
                    break;
                case AIState.AimToEnemy:
                    
                    if (_targetEnemy == null)
                    {
                        ChooseTask();
                        return;
                    }
                    
                    vector = GetPath(_targetEnemy.Position, out distance);
                    
                    if (distance < _botParam.DistanceToAim * _distanceAimCoefficient)
                    {
                        _moveComponent.Move(Vector3.zero, deltaTime);
                    }
                    else
                    {
                        Model.LookAt(vector);
                        _moveComponent.Move(Model.View.PhysicsTransform.forward, deltaTime);
                    }
                    
                    break;
                case AIState.GoToBorder:
                    
                    vector = GetPath(_targetThrowPoint.transform.position, out distance);
                    
                    Model.LookAt(vector);
                    
                    if (distance < _botParam.DistanceToItem)
                    {
                        SetState(AIState.ThrowEnemy);
                        _moveComponent.Move(Vector3.zero, deltaTime);
                    }
                    else
                    {
                        _moveComponent.Move(Model.View.PhysicsTransform.forward, deltaTime);
                    }
                    
                    break;
                default:
                    _moveComponent.Move(vector, deltaTime);
                    break;
            }
            

            if (Random.Range(0.0f, 1.0f) > 0.99f)
            {
                ChooseTask();
            }
        }

        private void SetState(AIState state)
        {
            if (_state == state)
                return;
            
            switch (state)
            {
                case AIState.ThrowEnemy:
                    Model.LookAt(_targetThrowPoint.LookDirection.transform.position - Model.View.transform.position, true);
                    _targetThrowPoint = null;
                    Model.StartDropUnit();
                    // _targetEnemy.OnDie -= OnDie;
                    // _targetEnemy = null;
                    // DOTween.Sequence().AppendInterval(1).OnComplete(() => ChooseTask());
                    _chooseTaskSequence.Play();
                    break;
                case AIState.None:
                    // DOTween.Sequence().AppendInterval(1).OnComplete(() => ChooseTask());
                    _chooseTaskSequence.Play();
                    break;
                case AIState.TryPickUp:
                    // DOTween.Sequence().AppendInterval(1f).OnComplete(() =>
                    // {
                    //     if (_currentItem == null)
                    //     {
                    //         FindEnemy();
                    //     }
                    // });
                    _pickUpTaskSequence.Play();
                    break;
                case AIState.AimToEnemy:
                    _distanceAimCoefficient = 1;
                    break;
            }
            _state = state;
        }

        private void OnOwnedWeapon()
        {
            if (_targetItem != null)
            {
                _targetItem.SetOwner -= OnOwnedWeapon;
                _currentItem = _targetItem;
                _targetItem = null;
                if (_currentItem.Owner == Model)
                {
                    if (_currentItem.Params.WeaponType == WeaponType.Range)
                    {
                        if (FindEnemy())
                        {
                            SetState(AIState.AimToEnemy);   
                        }
                    }
                    else if(_currentItem.Params.WeaponType == WeaponType.Melee)
                    {
                        SetState(AIState.None);
                        if(FindEnemy())
                        {
                            SetState(AIState.FindEnemy);   
                        }
                    }
                    return;
                }
                SetState(AIState.None);
                if(FindEnemy())
                {
                    SetState(AIState.FindEnemy);   
                }   
            }
        }

        private InteractableItemModel FindClosestWeapon(List<InteractableItemModel> weapons)
        {
            float minDistance = float.MaxValue;
            var unitPosition = Model.View.transform.position;
            InteractableItemModel closestWeapon = null;
            foreach (var weapon in weapons)
            {
                var distance = (unitPosition - weapon.View.transform.position).sqrMagnitude;
                if (distance < minDistance)
                {
                    closestWeapon = weapon;
                    minDistance = distance;
                }
            }
            
            return closestWeapon;
        }

        private UnitModel FindClosestEnemy(List<UnitModel> enemies)
        {
            float minDistance = float.MaxValue;
            if (Model.View == null) return null;
            var unitPosition = Model.View.transform.position;
            UnitModel closestEnemy = null;
            foreach (var enemy in enemies)
            {
                if(enemy.CurrentState == UnitState.Die) continue;
                var distance = (unitPosition - enemy.View.transform.position).sqrMagnitude;
                if (distance < minDistance)
                {
                    closestEnemy = enemy;
                    minDistance = distance;
                }
            }
            
            return closestEnemy;
        }
    }
}