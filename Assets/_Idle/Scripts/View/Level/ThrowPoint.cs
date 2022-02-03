using System.Collections.Generic;
using GeneralTools;
using UnityEngine;

namespace _Idle.Scripts.View.Level
{
    public class ThrowPoint : BaseBehaviour
    {
        [SerializeField] private Transform _lookDirection;
        
        public Transform LookDirection => _lookDirection;
    }
}