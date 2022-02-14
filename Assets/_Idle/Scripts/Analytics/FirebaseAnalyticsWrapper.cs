using System;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;

namespace _Idle.Scripts.Analytics
{
	public class FirebaseAnalyticsWrapper : AnalyticsWrapper
    {
        private FirebaseApp _app;
        
        public override void Init()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(t =>
            {
                _app = FirebaseApp.DefaultInstance;
            });
        }
        
        public override void CustomEventProcess(Enum eventType, params object[] parameters)
        {
            if (!(eventType is GameEvents metaEvents)) return;

            if (_app == null) return;
            
            switch (metaEvents)
            {
                case GameEvents.FirstSession:
                    FirebaseAnalytics.LogEvent(AnalyticsEvents.FIRST_SESSION);
                    break;
                case GameEvents.GameStart:
                    FirebaseAnalytics.LogEvent(AnalyticsEvents.GAME_START);
                    break;
                
                case GameEvents.LevelStart:
                    var startId = parameters[0].ToString();
                    while (startId.Length < 4)
                    {
                        startId = "0" + startId;
                    }
                    FirebaseAnalytics.LogEvent($"{AnalyticsEvents.LEVEL_START}:{startId}");
                    break;
                case GameEvents.LevelComplete:
                    var endId = parameters[0].ToString();
                    while (endId.Length < 4)
                    {
                        endId = "0" + endId;
                    }
                    FirebaseAnalytics.LogEvent($"{AnalyticsEvents.LEVEL_COMPLETE}:{endId}");
                    break;
                case GameEvents.LevelFail:
                    var failId = parameters[0].ToString();
                    while (failId.Length < 4)
                    {
                        failId = "0" + failId;
                    }
                    FirebaseAnalytics.LogEvent($"{AnalyticsEvents.LEVEL_FAIL}:{failId}");
                    break;
                
                case GameEvents.BannerStart:
                    FirebaseAnalytics.LogEvent(AnalyticsEvents.BANNER_START);
                    break;
                case GameEvents.BannerClosed:
                    FirebaseAnalytics.LogEvent(AnalyticsEvents.BANNER_CLOSED);
                    break;
                
                case GameEvents.InterstitialVideoAdStarted:
                    FirebaseAnalytics.LogEvent($"{AnalyticsEvents.INTER_START}:{parameters[0]}");
                    break;
                case GameEvents.InterstitialVideoAdShowed:
                    FirebaseAnalytics.LogEvent($"{AnalyticsEvents.INTER_SHOWN}:{parameters[0]}");
                    break;
                case GameEvents.InterstitialVideoAdError:
                    FirebaseAnalytics.LogEvent($"{AnalyticsEvents.INTER_FAILED}:{parameters[0]}");
                    break;
                
                case GameEvents.RewardVideoAdStarted:
                    FirebaseAnalytics.LogEvent($"{AnalyticsEvents.REWARDED_START}:{parameters[0]}");
                    break;
                case GameEvents.RewardVideoAdShowed:
                    FirebaseAnalytics.LogEvent($"{AnalyticsEvents.REWARDED_SHOWN}:{parameters[0]}");
                    break;
                case GameEvents.RewardVideoAdError:
                    FirebaseAnalytics.LogEvent($"{AnalyticsEvents.REWARDED_FAILED}:{parameters[0]}");
                    break;
                case GameEvents.RewardVideoAdClosed:
                    FirebaseAnalytics.LogEvent($"{AnalyticsEvents.REWARDED_CLOSED}:{parameters[0]}");
                    break;
                
                case GameEvents.SkinPurchase:
                    FirebaseAnalytics.LogEvent($"{AnalyticsEvents.SKIN_PURCHASE}:{parameters[0]}");
                    break;
            }
        }
        
        public override void SendEvent(string eventType, params (string, object)[] args)
        {
        }

        public override void ForceSend()
        {
        }
    }
}