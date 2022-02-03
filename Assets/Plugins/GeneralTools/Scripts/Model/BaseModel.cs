using System;
using System.Collections.Generic;
using UnityEngine;

namespace GeneralTools.Model
{
	[Serializable]
	public abstract class BaseModel
	{
		protected readonly List<BaseComponent> Components = new List<BaseComponent>();

		public virtual BaseModel Init()
		{
			return this;
		}

		public virtual BaseModel Start()
		{
			foreach (var component in Components)
			{
				component.Start();
			}

			return this;
		}

		public virtual BaseModel End()
		{
			foreach (var component in Components)
			{
				component.End();
			}

			return this;
		}

		public virtual void Update(float deltaTime)
		{
			UpdateComponents(deltaTime);
		}

		public virtual void FixedUpdate(float deltaTime)
		{
			FixedUpdateComponents(deltaTime);
		}

		protected void UpdateComponents(float deltaTime)
		{
			for (int i = 0; i < Components.Count; i++)
			{
				Components[i].Update(deltaTime);
			}
		}

		protected void FixedUpdateComponents(float deltaTime)
		{
			for (int i = 0; i < Components.Count; i++)
			{
				Components[i].FixedUpdate(deltaTime);
			}
		}

		public void StartComponents()
		{
			for (int i = 0; i < Components.Count; i++)
			{
				Components[i].Start();
			}
		}

		public void AddComponents(params BaseComponent[] components)
		{
			foreach (var component in components)
			{
				AddComponent(component);
			}
		}

		public T AddComponent<T>(T component) where T : BaseComponent
		{
			if (Get<T>() != null)
			{
				Debug.LogError($"{this} already has component of type {typeof(T)}");
			}
			else
			{
				Components.Add(component);
			}

			return component;
		}

		public T AddMultiComponent<T>(T component) where T : BaseComponent
		{
			Components.Add(component);

			return component;
		}

		public T Get<T>() where T : BaseComponent
		{
			var type = typeof(T);
			return Components.Find(component => component.GetType() == type) as T;
		}

		public T RemoveComponent<T>() where T : BaseComponent
		{
			var component = Get<T>();
			if (component != null) Components.Remove(component);

			return component;
		}

		public void RemoveAllComponents()
		{
			foreach (var component in Components) component.End();
			Components.Clear();
		}
	}
}