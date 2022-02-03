using UnityEngine;

namespace GeneralTools
{
	public class BaseBehaviour : MonoBehaviour
	{
		private Transform _cachedTransform;
		private bool _transformIsCached;

		/// <summary>
		/// Cached Unity transform
		/// </summary>
		public new Transform transform
		{
			get
			{
				if (_transformIsCached) return _cachedTransform;
				_cachedTransform = base.transform;
				_transformIsCached = _cachedTransform != null;
				return _cachedTransform;
			}
		}

		public virtual void StartMe()
		{

		}

		public virtual void UpdateMe(float deltaTime)
		{
			
		}
		
		public virtual void FixedUpdateMe(float fixedTime)
		{
		}

		public virtual void LateUpdateMe()
		{
		}
	}
}