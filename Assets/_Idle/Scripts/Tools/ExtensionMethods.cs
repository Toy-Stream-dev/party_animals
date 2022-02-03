using UnityEngine;

namespace _Idle.Scripts.Tools
{
	public static class ExtensionMethods
	{
		public static bool EqualTo(this double a, double b, double precision = 0.0001)
		{
			return (a - b).Abs() < precision;
		}

		public static double Abs(this double a)
		{
			return a < 0 ? -a : a;
		}

		public static float XZDirectionToAngle(this Vector3 direction)
		{
			var angle = Mathf.Atan2(-direction.z, direction.x) * Mathf.Rad2Deg;
			angle = angle < 0 ? 360 + angle : angle;
			return angle;
		}

		public static string ToColorTag(this Color color)
		{
			return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>";
		}

		public static string ToColoredStr(this object obj, Color color)
		{
			return $"{color.ToColorTag()}{obj}</color>";
		}
		
		/// <summary>
		/// Sets the target rotation of the configurable joint to be the given rotation relative to the original rotation
		/// </summary>
		/// <param name="joint">The joint whose target rotation is to be set</param>
		/// <param name="currentRotation">The orientation you would like the joint to be in</param>
		/// <param name="originalRotation">The original orientation of the joint</param>
		public static void SetTargetRotation(this ConfigurableJoint joint, Quaternion currentRotation, Quaternion originalRotation)
		{
			joint.targetRotation = Quaternion.identity * (originalRotation * Quaternion.Inverse(currentRotation));
		}
	}
}