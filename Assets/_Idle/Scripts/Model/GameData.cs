using System;
using System.Collections.Generic;
using System.Linq;
using _Idle.Scripts.Balance;
using _Idle.Scripts.Enums;
using _Idle.Scripts.Model.Numbers;
using _Idle.Scripts.Saves;
using UnityEngine;

namespace _Idle.Scripts.Model
{
    [Serializable]
    public class GameData
    {
        [SerializeField] private int _region;
        [SerializeField] private List<GameParam> _params = new List<GameParam>();
        [SerializeField] private List<GameProgress> _progresses = new List<GameProgress>();

        private GameBalance _balance;
        
        public int Region => _region;
        
        public DateTime WentBackgroundTime { get; private set; }

        public GameData(int region)
        {
            _region = region;
            _balance = GameBalance.Instance;

            CreateParam(GameParamType.Soft, 0, false);
        }

        public void IncRegion()
        {
            _region++;
        }
       
        private GameParam CreateParam(GameParamType type, double baseValue = 0f, bool updateParamValue = true)
        {
            var param = new GameParam(type, baseValue);

            _params.Add(param);

            if (updateParamValue)
            {
                UpdateParamValue(param);
                param.LevelChanged += () => UpdateParamValue(param);
            }

            return param;
        }

        private void UpdateParamValue(GameParam param)
        {
            // var progression = _balance.Progressions.Find(p => p.Type == param.Type);
            // if (progression == null) return;
            //
            // var value = progression.GetRoundedValue(param.Level);
            //
            // param.SetValue(value);
        }
		
        public GameParam GetParam(GameParamType type, bool createIfNotExists = true)
        {
            var param = _params.Find(p => p.Type == type);

            if (param == null && createIfNotExists)
            {
                param = new GameParam(type);
                _params.Add(param);
            }

            return param;
        }

        public bool HasParam(GameParamType type) => GetParam(type, false) != null;

        public GameProgress CreateProgress(GameParamType type, BigNumber target, bool looped = true)
        {
            var progress = new GameProgress(type, target, looped);
            _progresses.Add(progress);
            return progress;
        }

        public GameProgress GetProgress(GameParamType type) => _progresses.Find(p => p.Type == type);

        public IEnumerable<GameParamType> GetCurrentParams()
        {
            return _params.Select(p => p.Type);
        }
        
        public void CopyFrom(GameData source)
        {
            _params.CopyFrom(source._params);
            _progresses.CopyFrom(source._progresses);
        }
    }
}