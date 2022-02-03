using GeneralTools.Editor;
using UnityEditor;

namespace _Idle.Scripts.Editor.Windows
{
	public class DevFlagsWindow : DevFlagsWindow<DevFlags>
	{
		[MenuItem("_Idle/Dev flags", false, -10)]
		public static void Init()
		{
			((DevFlagsWindow)GetWindow(typeof(DevFlagsWindow))).Show();
		}
	}
}