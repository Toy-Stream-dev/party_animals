using System;
using System.Collections.Generic;
using System.Linq;
using GeneralTools.Model;
using UnityEngine;

namespace _Idle.Scripts.Model.Base
{
	public class ModelsContainer<T> : BaseModel where T : BaseModel
	{
		public bool Paused { get; set; }

		[SerializeField] protected List<T> All = new List<T>();

		public T Create(bool init = true)
		{
			T model = Activator.CreateInstance<T>();
			if (init) model.Init();
			All.Add(model);
			return model;
		}

		public T Get(Predicate<T> predicate)
		{
			return All.Find(predicate);
		}

		public override BaseModel Start()
		{
			foreach (var model in All)
			{
				model.Start();
			}

			return base.Start();
		}

		public override void Update(float deltaTime)
		{
			if (Paused) return;

			for (var i = 0; i < All.Count; i++)
			{
				All[i].Update(deltaTime);
			}
		}

		public override void FixedUpdate(float deltaTime)
		{
			if (Paused) return;

			for (var i = 0; i < All.Count; i++)
			{
				All[i].FixedUpdate(deltaTime);
			}
		}

		public IEnumerable<T> Where(Func<T, bool> predicate)
		{
			return All.Where(predicate);
		}
	}
}