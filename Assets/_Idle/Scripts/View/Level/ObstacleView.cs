using _Game.Scripts.Enums;
using _Idle.Scripts.Enums;
using _Idle.Scripts.Model;
using _Idle.Scripts.Model.Player;
using GeneralTools;
using GeneralTools.Model;
using UnityEngine;

namespace _Idle.Scripts.View.Level
{
	public class ObstacleView : BaseBehaviour
	{
		public int CostumeId;
		public CostumePartType PartType;
		public BonusType BonusType;
		public ParticleSystem CollisionParticle;

		public MeshFilter MeshFilter;
		public MeshRenderer Renderer;

		private bool _collected; 

		private void OnTriggerEnter(Collider other)
		{
			//TODO: release it
			if (other.TryGetComponent<PlayerView>(out var player))
			{
				// player.Model.EnableCostumePart(CostumeId, PartType);
				var effectModel = Models.Get<GameEffectModel>();
				var pos = player.transform.position;
				pos.z += 5;
				effectModel.Play(GameEffectType.Poof, pos);

				switch (BonusType)
				{
					case BonusType.Coin:
						var coins = Models.Get<GameModel>().GetParam(GameParamType.CollectedCoins);
						coins.Change(1);
						break;
					
					case BonusType.Boost:
						player.Model.Boost();
						break;
				}
				
				_collected = true;
				Hide();
			}
		}

		public void Show()
		{
			if (_collected) return;
			
			gameObject.SetActive(true);
		}

		public void Hide()
		{
			gameObject.SetActive(false);
		}
	}
}