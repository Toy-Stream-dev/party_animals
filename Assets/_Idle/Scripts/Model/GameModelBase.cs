using System;
using System.Collections.Generic;
using System.Linq;
using _Idle.Scripts.Balance;
using _Idle.Scripts.Enums;
using _Idle.Scripts.Model.Numbers;
using _Idle.Scripts.Saves;
using GeneralTools.Model;
using GeneralTools.Tools;
using UnityEngine;

namespace _Idle.Scripts.Model
{
	[Flags]
	public enum GameFlag
	{
		None = 0,
		TutorialFinished = 1 << 1,
		AllAdsRemoved = 1 << 2
	}

	public class GameModelBase : BaseModel, ISerializationCallbackReceiver
	{
		[SerializeField] private GameFlag _serializedFlags;
		[SerializeField] private List<GameParam> _params = new List<GameParam>();
		[SerializeField] private List<GameProgress> _progresses = new List<GameProgress>();
		[SerializeField] private protected List<GameData> _datas = new List<GameData>();

		private readonly Queue<Action> _actionsQueue = new Queue<Action>();

		protected GameBalance Balance { get; private set; }
		
		public FlagsContainer<GameFlag> Flags { get; } = new FlagsContainer<GameFlag>();
		public GameData CurrentData { get; set; }
		public override BaseModel Init()
		{
			Balance = GameBalance.Instance;
			
			return base.Init();
		}

		public override BaseModel Start()
		{
			foreach (var param in _params)
			{
				UpdateParamValue(param);
			}
			return base.Start();
		}
		
		protected void InitGameData()
		{
			CurrentData = _datas.FirstOrDefault();
			if (CurrentData != null) return;
			CurrentData = new GameData(1);
			_datas.Add(CurrentData);
		}

		protected GameParam CreateParam(GameParamType type, double baseValue = 0f, bool updateParamValue = true)
		{
			var param = new GameParam(type, baseValue);

			_params.Add(param);

			if (updateParamValue) UpdateParamValue(param);
			param.LevelChanged += () => UpdateParamValue(param);

			return param;
		}

		private void UpdateParamValue(GameParam param)
		{
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

		public override void Update(float deltaTime)
		{
			while (_actionsQueue.Count > 0)
			{
				var action = _actionsQueue.Dequeue();
				action.Invoke();
			}
		}

		protected void PostponeAction(Action action)
		{
			_actionsQueue.Enqueue(action);
		}

		protected void CopyFrom(GameModelBase source)
		{
			_params.CopyFrom(source._params);
			_progresses.CopyFrom(source._progresses);
			Flags.CopyFrom(source._serializedFlags);
		}

		public virtual void OnBeforeSerialize()
		{
			_serializedFlags = Flags.Value;
		}

		public virtual void OnAfterDeserialize()
		{
			Flags.CopyFrom(_serializedFlags);
		}
	}
}