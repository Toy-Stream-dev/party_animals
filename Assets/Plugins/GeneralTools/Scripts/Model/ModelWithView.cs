using System;
using GeneralTools.Pooling;
using GeneralTools.Tools;
using GeneralTools.View;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace GeneralTools.Model
{
	public class ModelWithView<T> : BaseModel where T : BaseView
	{
#if ODIN_INSPECTOR
		[ReadOnly, ShowInInspector]
#endif
		private T _view;

		public T View => _view;
		protected Transform Transform { get; private set; }

		public Vector3 Position
		{
			get => Transform.position;
			set => Transform.position = value;
		}

		public Vector3 LocalPosition
		{
			get => Transform.localPosition;
			set => Transform.localPosition = value;
		}

		public virtual ModelWithView<T> SetView(T view)
		{
			_view = view;
			Transform = _view != null ? _view.transform : null;
			if (_view is IWithModel viewWithModel)
			{
				viewWithModel.SetModel(this);
			}

			return this;
		}

		public virtual ModelWithView<T> SpawnView(Transform root, bool resetScale = true, bool activate = true, Predicate<T> predicate = default)
		{
			var view = Pool.Pop<T>(root, resetScale, activate, predicate);
			if (view == null)
			{
#if DEBUG
				Debug.LogWarning($"View is NULL");
#endif
				return null;
			}
			
			view.Init();
			return SetView(view);
		}

		public virtual ModelWithView<T> DestroyView()
		{
			if (_view != null)
			{
				_view.Clear();
				_view.PushToPool();
				_view = null;
			}

			return this;
		}
		
		public virtual ModelWithView<T> DeleteView()
		{
			if (_view != null)
			{
				_view.Clear();
				_view.DestroyGO();
				_view = null;
			}

			return this;
		}

		public override BaseModel Start()
		{
			if (_view != null) _view.StartMe();
			return base.Start();
		}
	}
}