using GeneralTools.Tools;
using Plugins.GeneralTools.Scripts.UI;

namespace _Idle.Scripts.UI.HUD
{
	public class HUDOverlayContainer : BaseWindow
	{
		public override void Init()
		{
			base.Init();
			gameObject.Activate();
		}
	}
}