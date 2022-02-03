using GeneralTools.UI;
using TMPro;
using UnityEngine;

namespace _Idle.Scripts.UI.HUD
{
    public class Fps : BaseUIBehaviour
    {
        private float _updateInterval = 0.5F;
        private float _accum;
        private int _frames;
        private float _timeLeft;
        
        [SerializeField] private TextMeshProUGUI _fpsText;

        private void Start()
        {
            _timeLeft = _updateInterval;  
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;
            _timeLeft -= deltaTime;
            _accum += Time.timeScale / deltaTime;
            ++_frames;
            if (_timeLeft > 0) return;
            
            var fps = _accum/_frames;
            var format = $"{fps:F2} FPS";
            _fpsText.text = format;
                
            _timeLeft = _updateInterval;
            _accum = 0.0F;
            _frames = 0;
        }
    }
}