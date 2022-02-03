using UnityEngine;

namespace _Idle.Scripts.Utilities
{
	public static class VectorExtensions
	{
		public static Vector3 Clamp(this Vector3 v, float min, float max)
		{
			return new Vector3(Mathf.Clamp(v.x, min, max), Mathf.Clamp(v.y, min, max), Mathf.Clamp(v.z, min, max));
		}
		
		public static Vector2 Clamp(this Vector2 v, float min, float max)
		{
			return new Vector2(Mathf.Clamp(v.x, min, max), Mathf.Clamp(v.y, min, max));
		}
		
		public static float GetRandomValue(this Vector2 v)
		{
			return UnityEngine.Random.Range(v.x, v.y);
		}
		
		public static int GetIntRandomValue(this Vector2 v)
		{
			return (int)UnityEngine.Random.Range(v.x, v.y);
		}
	}
}