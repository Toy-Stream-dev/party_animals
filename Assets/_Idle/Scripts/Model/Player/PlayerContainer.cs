using _Idle.Scripts.Model.Base;
using _Idle.Scripts.Model.Unit;
using GeneralTools.Model;
using UnityEngine;

namespace _Idle.Scripts.Model.Player
{
    public class PlayerContainer : ModelsContainer<UnitModel>
    {
        private UnitModel _player;
        private GameModel _gameModel;
        
        public override BaseModel Start()
        {
            _gameModel = Models.Get<GameModel>();
            return base.Start();
        }
        
        public UnitModel SpawnPlayer()
        {
            _player = Create()
                .SpawnView(MainGame.PlayerContainer, true, view => view.UnitType == UnitType.Player)
                .Start();
            
            return _player;
        }

        public void Destroy()
        {
            _player.Destroy();
            All.Remove(_player);
            
            _gameModel.Player = null;
            _gameModel.LevelFailed();
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
        }

        public override void FixedUpdate(float deltaTime)
        {
            base.FixedUpdate(deltaTime);
        }
    }
}