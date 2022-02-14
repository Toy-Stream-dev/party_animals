using System;
using System.Collections.Generic;
using _Idle.Scripts.Data;
using _Idle.Scripts.Enums;
using _Idle.Scripts.Model.Item;
using _Idle.Scripts.Model.Level;
using GeneralTools.Tools;
using GoogleParse;
using GoogleParse.Attributes;
using UnityEngine;

namespace _Idle.Scripts.Balance
{
	[CreateAssetMenu(fileName = "GameBalance", menuName = "_Idle/GameBalance", order = 0)]
	[TableSheetName("Constants")]
	public class GameBalance : SingletonScriptableObject<GameBalance>, IParsable
	{
		public int RewardSoftCurrencyForLevel = 100;
		public int RewardSoftCurrencyForCoin = 5;
		public int RandomLevelFromCount = 2;
		public float EmulateLoadingTime = 0.7f;
		// public float InteractionTime = 0.5f;
		public float AttackRaycastDistanceArmed = 0.15f;
		public float AttackRaycastDistanceUnarmed = 0.05f;
		public float RangeAttackRaycastDistance = 3.0f;
		public float RangeAttackMinDistance = 0.15f;
		public LayerMask AttackLayermask = 1 << 9;
		public LayerMask GroundLayer;
		public LayerMask PlayerLayer;
		public LayerMask ItemLayer;
		public float ThrowForce = 5;
		public float ProjectilePushForce = 5.0f;
		public float PushForce = 100.0f;
		public float DropUnitForce;
		public float PushInterval = 2.0f;
		public Vector3 MeleeWeaponScale = Vector3.one * 0.15f;
		public Vector3 RangeWeaponScale = Vector3.one * 0.21f;
		public float ThrowUnitDelay;

		public float HandPunchPower;
		public float MeleeWeaponBreakForceMultiplier = 1.0f;
		public float RangeWeaponBreakForceMultiplier = 0.2f;
		public float AttackRadius = 3.0f;

		[Header("TimeScaleSetting")]
		public float SlowMotionScale;
		public float TimeToSlow;
		public float TimeToNormal;

		[Header("TimeScaleSetting")]
		public float StartFOV;
		public float SlowMotionFOV;

		[Header("Delays")]
		public float EndLevelDelay;
		public float UnitDestroyDelay;

		[Space] 
		[Header("AI")] 
		public bool DisableAI = false;
		

		[Space]
		public PlayerData PlayerData;
		[Space]
		public BotParam BotParam;
		public int StartRatingPlace = 1500;
		public InteractableItemParams[] ItemsParams;
		public LevelParams[] LevelParams;
		public string[] NickNames;
		public Item[] ItemSkins;
		public Mesh[] UnitSkins;

		[Header("Input")] public float TouchpadSense = 5f;
		
		[Header("Other")]
		public List<DailyRewardConfig> DailyRewards;
		public List<OfferConfig> Offers;
		
		[Space, Header("Experience")]
		public List<CharacterLevel> CharacterLevels;
		[Space, Header("Killing phrases")]
		public List<KillingTextConfig> KillingTextConfigs;
		[Space, Header("Win streak rewards")]
		public List<WinStreakRewardItem> WinStreakRewards;


#if UNITY_EDITOR
		[UnityEditor.MenuItem("_Idle/Balance")]
		private static void OpenBalance()
		{
			UnityEditor.Selection.activeObject = Instance;
		}
#endif
		public void OnParsed()
		{
		}
	}
	
		
	[Serializable]
	public class KillingTextConfig
	{
		public int KillCount;
		[TextArea]
		public string Text;
		public Color KillerColor;
		public Color VictimColor;
	}
		
	[Serializable]
	public class CharacterLevel
	{
		public int Level;
		public int Experience;
		public RewardItemConfig[] Rewards;
	}
		
	[Serializable]
	public class Item
	{
		public string Name;
		public string Identificator;
		public int Id;
		public PurchaseType PurchaseType = PurchaseType.Soft;
		public int PriceAmount;
		public Mesh Mesh;
	}

	[Serializable]
	public class DailyRewardConfig
	{
		public int Day;
		public int Amount;
	}
	
	[Serializable]
	public class WinStreakRewardItem
	{
		public int WinCount;
		public RewardItemConfig[] Rewards;
	}
	
	[Serializable]
	public class RewardItemConfig
	{
		public GameParamType RewardType;
		public int Amount;
	}
	
	[Serializable]
	public class OfferConfig
	{
		public int Id;
		public bool NonConsumable;
		public string ShopId;
	}

	[Serializable]
	public class BotParam
	{
		public List<AITask> Tasks;
		public float DistanceToItem;
		public float DistanceToEnemy;
		public float DistanceToAim;
		[Range(0, 0.5f)]
		public float StepPercentAfterShot;
		public float MoveSpeed;
		public int TargetCountLimit = 0;
		
		[Header("Turn settings")] 
		public float AngularSpeed = 5.0f;
		
		[Header("Stun settings")]
		public float StunTime;
		public float StunResetTime;
		public int StunPoints = 10;
		public int FallPoint = 5;
	}
	
	[Serializable]
	public class AITask : IRandomizedByWeight
	{
		public AITaskType Type;
		public float Priority;
	
		public float RandomWeight => Priority;
	}
}