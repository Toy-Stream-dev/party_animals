using _Game.Scripts.Enums;
using GeneralTools;
using GeneralTools.Model;
using UnityEngine;

namespace _Game.Scripts.Model.Vfx
{
    public class BaseGameEffect : BaseBehaviour
    {
        [SerializeField] private GameEffectType _type;
        [SerializeField] private ParticleSystem _vfx;
        
        protected ParticleSystem Vfx => _vfx;
       
        public bool IsPlaying => _vfx.isPlaying;

        public GameEffectType Type => _type;
        public Transform OwnerTransform { get; private set; }
        public BaseModel OwnerModel { get; private set; }

        public virtual void Play()
        {
            if (_vfx == null) return;
            _vfx.Play();
        }
        public virtual void Stop()
        {
            _vfx?.Stop();
        }

        public virtual void Play(Vector3 pos)
        {
            if (_vfx == null) return;
            transform.position = pos;
            _vfx.Play();
        }
        
        public virtual void Play(Vector2 pos)
        {
            if (_vfx == null) return;
            transform.GetComponent<RectTransform>().anchoredPosition = pos;
            _vfx.Play();
        }

        public override void UpdateMe(float deltaTime)
        {
            base.UpdateMe(deltaTime);
        }

        public virtual void Hide()
        {
            _vfx.Stop();
        }

        public virtual void SetOwner(Transform owner, BaseModel model)
        {
            OwnerTransform = owner;
            OwnerModel = model;
        }
    }
}