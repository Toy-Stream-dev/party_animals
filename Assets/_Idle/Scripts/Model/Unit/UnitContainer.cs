using System;
using System.Collections.Generic;
using System.Linq;
using _Idle.Scripts.Balance;
using _Idle.Scripts.Enums;
using _Idle.Scripts.Model.Base;
using _Idle.Scripts.Model.Unit;
using _Idle.Scripts.Utilities;
using _Idle.Scripts.View.Level;
using GeneralTools.Model;
using GeneralTools.Tools;

namespace _Idle.Scripts.Model.Player
{
    public class UnitContainer : ModelsContainer<UnitModel>
    {
        private GameModel _gameModel;

        public Action<UnitModel, Balance.Item> OnSpawnUnit;

        public override BaseModel Start()
        {
            _gameModel = Models.Get<GameModel>();
            return base.Start();
        }

        public UnitModel SpawnUnit()
        {
            var unit = Create()
                .SpawnView(MainGame.EnemiesContainer, true, view => view.UnitType == UnitType.EnemyBase)
                .Start();
            
            return unit;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
        }

        public override void FixedUpdate(float deltaTime)
        {
            base.FixedUpdate(deltaTime);
        }

        public void SpawnUnits()
        {
            var level = _gameModel.LevelLoader.CurrentLevel;
            
            var spawnPoints = level.View.transform.GetComponentsInChildren<SpawnPoint>().Where(x => x.PointType == SpawnPointType.Unit);
            var spawnedSkins = new List<Balance.Item>();

            var playerSkinId = _gameModel.GetParam(GameParamType.SelectedSkin).IntValue;
            var playerSkin = GameBalance.Instance.ItemSkins[playerSkinId];
            spawnedSkins.Add(playerSkin);
            
            Balance.Item skin;
            var allSkins = GameBalance.Instance.ItemSkins;
            var playersCount = level.LevelParams.IsTutorial 
                ? 1
                : level.LevelParams.PlayersCount.RandomValue();

            for (var i = 0; i < playersCount; i++)
            {
                var spawnPoint = spawnPoints.Where(point => point.Available).RandomValue();
                spawnPoint.Available = false;
                
                if (allSkins.Length == spawnedSkins.Count)
                {
                    skin = allSkins.RandomValue();
                }
                else
                {
                    skin = allSkins.Except(spawnedSkins).RandomValue();
                    spawnedSkins.Add(skin);
                }
                
                var unit = SpawnUnit();
                unit.SetSkin(skin.Mesh);
                unit.SetPosition(spawnPoint.transform.position);
                unit.LevelStart();
                OnSpawnUnit?.Invoke(unit, skin);
            }
        }
        
        public void Destroy(UnitModel unitModel)
        {
            unitModel.Destroy();
            All.Remove(unitModel);
            if (All.Count == 0)
            {
                Models.Get<GameModel>().LevelComplete();
            }
        }
        
        public void DestroyAll()
        {
            foreach (var unit in All)
            {
                unit.Destroy();
            }
            All.Clear();
        }

        public List<UnitModel> GetAll()
        {
            return All;
        }
    }
}