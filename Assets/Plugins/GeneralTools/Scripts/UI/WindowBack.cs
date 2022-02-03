using System;

namespace GeneralTools.UI
{
	public class WindowBack : InteractableUIItem
	{
		public event Action Clicked;
		private BaseButton _closeWindowButton;

		public void Init(BaseButton closeWindowButton)
		{
			_closeWindowButton = closeWindowButton;
		}

		protected override void OnClick()
		{
			Clicked?.Invoke();

			if (_closeWindowButton == null ||
			    !_closeWindowButton.gameObject.activeInHierarchy) return;

			_closeWindowButton.SimulateClick();
		}
	}
}