using System;
using GameAnalyticsSDK;

namespace _Idle.Scripts.Analytics
{
	public class GameAnalyticsWrapper : AnalyticsWrapper
	{
		public override void Init()
		{
			GameAnalytics.Initialize();
		}

		public override void SendEvent(string eventType, params (string, object)[] args)
		{
		}

		public override void CustomEventProcess(Enum eventType, params object[] parameters)
		{
			if (!(eventType is GameEvents metaEvents)) return;

			switch (metaEvents)
			{
				case GameEvents.FirstSession:
					GameAnalytics.NewDesignEvent(AnalyticsEvents.FIRST_SESSION);
					break;
				case GameEvents.GameStart:
					GameAnalytics.NewDesignEvent(AnalyticsEvents.GAME_START);
					break;
				
				case GameEvents.LevelStart:
					GameAnalytics.NewDesignEvent($"{AnalyticsEvents.LEVEL_START}:{(int)parameters[0]}");
					break;
				case GameEvents.LevelComplete:
					GameAnalytics.NewDesignEvent($"{AnalyticsEvents.LEVEL_COMPLETE}:{(int)parameters[0]}");
					break;
				case GameEvents.LevelFail:
					GameAnalytics.NewDesignEvent($"{AnalyticsEvents.LEVEL_FAIL}:{(int)parameters[0]}");
					break;
                
				case GameEvents.BannerStart:
					GameAnalytics.NewDesignEvent(AnalyticsEvents.BANNER_START);
					break;
				case GameEvents.BannerClosed:
					GameAnalytics.NewDesignEvent(AnalyticsEvents.BANNER_CLOSED);
					break;
                
				case GameEvents.InterstitialVideoAdStarted:
					GameAnalytics.NewDesignEvent($"{AnalyticsEvents.INTER_START}:{parameters[0]}");
					break;
				case GameEvents.InterstitialVideoAdShowed:
					GameAnalytics.NewDesignEvent($"{AnalyticsEvents.INTER_SHOWN}:{parameters[0]}");
					break;
				case GameEvents.InterstitialVideoAdError:
					GameAnalytics.NewDesignEvent($"{AnalyticsEvents.INTER_FAILED}:{parameters[0]}");
					break;
                
				case GameEvents.RewardVideoAdStarted:
					GameAnalytics.NewDesignEvent($"{AnalyticsEvents.REWARDED_START}:{parameters[0]}");
					break;
				case GameEvents.RewardVideoAdShowed:
					GameAnalytics.NewDesignEvent($"{AnalyticsEvents.REWARDED_SHOWN}:{parameters[0]}");
					break;
				case GameEvents.RewardVideoAdError:
					GameAnalytics.NewDesignEvent($"{AnalyticsEvents.REWARDED_FAILED}:{parameters[0]}");
					break;
				case GameEvents.RewardVideoAdClosed:
					GameAnalytics.NewDesignEvent($"{AnalyticsEvents.REWARDED_CLOSED}:{parameters[0]}");
					break;
				
				case GameEvents.SkinPurchase:
					GameAnalytics.NewDesignEvent($"{AnalyticsEvents.SKIN_PURCHASE}:{(int)parameters[0]}");
					break;
			}
		}
	}
}