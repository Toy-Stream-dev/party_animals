using UnityEngine;

namespace GeneralTools.Tools.ExtensionMethods
{
	public static class VectorExtensionMethods
	{
		public static Vector2 Ceil(this Vector2 vec)
		{
			return new Vector2(Mathf.Ceil(vec.x), Mathf.Ceil(vec.y));
		}

		public static Vector2 Divide(this Vector2 vec, float value)
		{
			return vec / value;
		}

		public static Vector2 Multiply(this Vector2 vec, float value)
		{
			return vec * value;
		}
	}
}