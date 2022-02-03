using _Idle.Scripts.Balance;
using DG.Tweening;
using GeneralTools.Model;
using UnityEngine;

namespace _Idle.Scripts.Model
{
    public class TimeScale : BaseModel
    {
        private float _lastFixedDeltaTime;
        
        public void SlowMotion(bool changeFOV = false)
        {
            _lastFixedDeltaTime = Time.fixedDeltaTime;
            DOTween.To(x => Time.fixedDeltaTime = x, _lastFixedDeltaTime, _lastFixedDeltaTime * 
                                                                          GameBalance.Instance.SlowMotionScale, GameBalance.Instance.TimeToSlow);
            DOTween.To(x => Time.timeScale = x, 1,GameBalance.Instance.SlowMotionScale,
                GameBalance.Instance.TimeToSlow).OnComplete(NormalTime);
            if (changeFOV)
            {
                GameCamera.Instance.ZoomInOut(GameBalance.Instance.TimeToSlow + GameBalance.Instance.TimeToNormal);
            }
        }

        private void NormalTime()
        {
            var tempFixedDeltaTime = Time.fixedDeltaTime;
            DOTween.To(x => Time.fixedDeltaTime = x, tempFixedDeltaTime, _lastFixedDeltaTime,
                GameBalance.Instance.TimeToNormal).SetEase(Ease.OutExpo);
            DOTween.To(x => Time.timeScale = x, GameBalance.Instance.SlowMotionScale,
                1, GameBalance.Instance.TimeToNormal).SetEase(Ease.OutExpo);
        }
    }
}