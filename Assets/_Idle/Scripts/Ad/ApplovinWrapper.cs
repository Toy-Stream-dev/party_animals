using System;
using UnityEngine;

namespace _Idle.Scripts.Ad
{
    public class ApplovinWrapper : AdWrapper
    {
        private Action<AdWatchType, AdVideoType, int> _rewardedEvent;
        private AdVideoType _videoType;
        private const string rewardedAdUnitId = "27d65bcd13d05ad2";
        private const string interstitialAdUnitId = "e5acbc549fd793ed";
        private const string bannerAdUnitId = "24fc4ea31f6637d1";

        public override void Start()
        {
            MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
            {
                Init();
            };
            
            MaxSdk.SetSdkKey("CvFQ6rVnxKktdZBfCvb-FTbPJcBiFOV7Fp94Oau3NX4O5ywf5948NLCrRGKdKSH9vg2zI_AyEuxLTP9-wIZZDn");
            MaxSdk.InitializeSdk();
        }
        
        public override void Init()
        {
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
            
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
            
            LoadRewardedAd();
            LoadInterstitial();

            // MaxSdk.ShowMediationDebugger();
        }
        
        private void LoadRewardedAd()
        {
            MaxSdk.LoadRewardedAd(rewardedAdUnitId);
        }
        
        private void LoadInterstitial()
        {
            MaxSdk.LoadInterstitial(interstitialAdUnitId);
        }
        
        public override void ShowRewardedVideo(Action<AdWatchType, AdVideoType, int> rewardedEvent)
        {
            _rewardedEvent = rewardedEvent;
            _videoType = AdVideoType.Reward;
            MaxSdk.ShowRewardedAd(rewardedAdUnitId);
        }
        
        public override void ShowInterstitialVideo(Action<AdWatchType, AdVideoType, int> rewardedEvent)
        {
            _rewardedEvent = rewardedEvent;
            _videoType = AdVideoType.Interstitial;
            MaxSdk.ShowInterstitial(interstitialAdUnitId);
        }

        public override void LoadBannerVideo()
        {
            MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);
            MaxSdk.SetBannerBackgroundColor(bannerAdUnitId, Color.white);
        }

        public override void ShowBannerVideo()
        {
            MaxSdk.ShowBanner(bannerAdUnitId);
        }

        public override void HideBannerVideo()
        {
            MaxSdk.HideBanner(bannerAdUnitId);
        }

        public override bool RewardVideoAvailable()
        {
            return MaxSdk.IsRewardedAdReady(rewardedAdUnitId);
        }
        
        public override bool InterstitialVideoAvailable()
        {
            return MaxSdk.IsInterstitialReady(interstitialAdUnitId);
        }

        #region Rewards

        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            _rewardedEvent?.Invoke(AdWatchType.Watched, _videoType, 0);
            _rewardedEvent = null;
        }
        
        private void OnRewardedAdLoadedEvent(string arg1, MaxSdkBase.AdInfo arg2)
        {
            // Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(rewardedAdUnitId) will now return 'true'

            // Reset retry attempt
            // retryAttempt = 0;
        }

        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            _rewardedEvent?.Invoke(AdWatchType.ErrorDisplay, _videoType, (int)errorInfo.Code);
            _rewardedEvent = null;
            
            // Rewarded ad failed to display. We recommend loading the next ad
            LoadRewardedAd();
        }

        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {}

        private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInf)
        {
            _rewardedEvent?.Invoke(AdWatchType.Clicked, _videoType, 0);
            _rewardedEvent = null;
        }
        
        private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _rewardedEvent?.Invoke(AdWatchType.Canceled, _videoType, 0);
            _rewardedEvent = null;
            // Rewarded ad is hidden. Pre-load the next ad
            LoadRewardedAd();
        }

        #endregion

        #region Interstitial

        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'

            // Reset retry attempt
            // retryAttempt = 0;
        }

        private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Interstitial ad failed to load 
            // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)

            // retryAttempt++;
            LoadInterstitial();
        }

        private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {}

        private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
            LoadInterstitial();
        }

        private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {}

        private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad is hidden. Pre-load the next ad.
            LoadInterstitial();
        }

        #endregion
    }
}