using System;

namespace _Idle.Scripts.Ad
{
    public abstract class AdWrapper
    {
        public abstract void Init();
        public abstract void Start();

        public abstract void ShowRewardedVideo(Action<AdWatchType, AdVideoType, int> action);
        public abstract void ShowInterstitialVideo(Action<AdWatchType, AdVideoType, int> action);
        public abstract void LoadBannerVideo();
        public abstract void HideBannerVideo();
        public abstract void ShowBannerVideo();
        public abstract bool RewardVideoAvailable();
        public abstract bool InterstitialVideoAvailable();
    }
}