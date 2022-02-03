using UnityEngine;

namespace GeneralTools.Tools.ExtensionMethods
{
	public static class UsefulExtensionsMethods
	{
		public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);

		public static bool IsVertical(this Direction direction)
		{
			return direction == Direction.Up || direction == Direction.Down;
		}

		public static Vector2 ToVec2(this Direction direction)
		{
			switch (direction)
			{
				case Direction.Up:
					return Vector2.up;

				case Direction.Right:
					return Vector2.right;

				case Direction.Down:
					return Vector2.down;

				case Direction.Left:
					return Vector2.left;
			}

			return new Vector2();
		}

		public static Vector3 ToVec3(this Direction direction) => direction.ToVec2();
	}
}