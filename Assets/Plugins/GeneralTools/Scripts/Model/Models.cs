using System;
using System.Collections.Generic;
using UnityEngine;

namespace GeneralTools.Model
{
	public static class Models
	{
		private static readonly Dictionary<Type, BaseModel> _models = new Dictionary<Type, BaseModel>();

		public static T Add<T>(T model, bool overwriteExisting = false, bool init = true) where T : BaseModel
		{
			var type = typeof(T);

			if (_models.ContainsKey(type))
			{
				if (!overwriteExisting)
				{
					Debug.LogError($"Model of type {type} is already added to Models");
					return null;
				}

				_models.Remove(type);
			}

			if (init) model.Init();

			_models.Add(type, model);

			return model;
		}

		public static T Add<T>(bool overwriteExisting = false, bool init = true) where T : BaseModel
		{
			var model = Activator.CreateInstance(typeof(T)) as T;
			return Add(model, overwriteExisting, init);
		}

		public static T AddIfNotExists<T>(bool init = true) where T : BaseModel
		{
			return Has<T>() ? Get<T>() : Add<T>(init:init);
		}

		public static T AddIfNotExists<T>(T model, bool init = true) where T : BaseModel
		{
			return Has<T>() ? Get<T>() : Add(model, init:init);
		}

		public static bool Has<T>() where T : BaseModel
		{
			return _models.ContainsKey(typeof(T));
		}

		public static T Remove<T>(T model) where T : BaseModel
		{
			var type = typeof(T);
			if (!_models.ContainsKey(type)) return default;

			_models.Remove(type);

			return model;
		}

		public static T Get<T>() where T : BaseModel
		{
			foreach (var model in _models.Values)
			{
				if (model is T targetModel) return targetModel;
			}

			return null;
		}

		public static void Start()
		{
			foreach (var model in _models.Values)
			{
				model.Start();
			}
		}

		public static void Update(float deltaTime)
		{
			foreach (var model in _models.Values)
			{
				model.Update(deltaTime);
			}
		}

		public static void FixedUpdate(float deltaTime)
		{
			foreach (var model in _models.Values)
			{
				model.FixedUpdate(deltaTime);
			}
		}
	}
}