using GeneralTools.UI;
using Plugins.GeneralTools.Scripts.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Idle.Scripts.UI.Windows
{
	public class BaseSoundableWindow : BaseWindow
	{
		[SerializeField] private bool _enableOpenSound = true;
		[ShowIf(nameof(_enableOpenSound))]
		[SerializeField] private GameSoundType _openSound = GameSoundType.Open;
		[SerializeField] private bool _enableCloseSound = true;
		[ShowIf(nameof(_enableCloseSound))]
		[SerializeField] private GameSoundType _closeSound = GameSoundType.Close;

		public override BaseUI Open()
		{
			if (_enableOpenSound)
			{
				GameSounds.Instance.PlaySound(GameSoundType.Open);
			}
			
			return base.Open();
		}

		public override void Close()
		{
			if (_enableCloseSound)
			{
				GameSounds.Instance.PlaySound(GameSoundType.Close);
			}
			
			base.Close();
		}
	}
}