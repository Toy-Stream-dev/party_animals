namespace _Idle.Scripts
{
	public enum GameEvents
	{
		None,

		PlayTime,
		Rewarded_10,
		Rewarded_20,
		RV10_d0,
		RV20_d0,

		Timer,
		Timer24,
		InAppPurchased,
		FirstInAppPurchased,

		FirstSession,
		GameStart,
		LevelStart,
		LevelComplete,
		LevelFail,
		
		Rate_us,
		
		BannerStart,
		BannerClosed,
		
		InterstitialVideoAdStarted,
		InterstitialVideoAdShowed,
		InterstitialVideoAdError,
		
		RewardVideoAdStarted,
		RewardVideoAdShowed,
		RewardVideoAdError,
		RewardVideoAdClosed,
		
		SkinPurchase
	}
}