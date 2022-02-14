using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Enums;
using _Game.Scripts.Model.Vfx;
using GeneralTools.Model;
using GeneralTools.Pooling;
using UnityEngine;

namespace _Idle.Scripts.Model
{
    public class GameEffectModel : BaseModel
    {
        private readonly List<BaseGameEffect> _effects = new List<BaseGameEffect>();
        private float _time;

        public override BaseModel Start()
        {
            Pool.Spawn<BaseGameEffect>(3, effect => effect.Type.Equals(GameEffectType.Poof));
            Pool.Spawn<BaseGameEffect>(1, effect => effect.Type.Equals(GameEffectType.ConfettiExplosion));
            
            return base.Start();
        }

        public override void Update(float deltaTime)
        {
            for (int i = 0; i < _effects.Count; i++)
            {
                _effects[i].UpdateMe(deltaTime);
            }
            
            base.Update(deltaTime);
        }

        public BaseGameEffect Play(GameEffectType type, BaseModel model, Vector3 pos)
        {
            var effect = _effects.FirstOrDefault(e => e.Type == type && e.OwnerModel == model && !e.IsPlaying);
            if (effect == null) effect = GetEffect(type);
            effect.Play(pos);
            
            _effects.Add(effect);

            return effect;
        }
        
        public BaseGameEffect Play(GameEffectType type, Vector3 pos)
        {
            var effect = _effects.FirstOrDefault(e => e.Type == type && !e.IsPlaying);
            if (effect == null) effect = GetEffect(type);
            effect.Play(pos);
            
            _effects.Add(effect);
            
            return effect;
        }
        
        public BaseGameEffect GetEffect(GameEffectType type, Transform transform = null)
        {
            var effect = Pool.Pop<BaseGameEffect>(MainGame.EffectContainer, true, true, effect => effect.Type.Equals(type));
           
            switch (type)
            {
            }

            return effect;
        }

        public void StopAll()
        {
            foreach (var effect in _effects)
            {
                effect.Stop();
            }
        }

        public void Remove(BaseGameEffect effect)
        {
            _effects.Remove(effect);
        }
    }
}