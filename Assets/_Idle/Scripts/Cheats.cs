using _Idle.Scripts.Enums;
using _Idle.Scripts.Model;
using _Idle.Scripts.Model.Numbers;
using _Idle.Scripts.UI.Windows;
using _Idle.Scripts.UI.Windows.DailyRewards;
using GeneralTools.Model;
using GeneralTools.Tools;
using GeneralTools.UI;
using UnityEngine;

namespace _Idle.Scripts
{
    public class Cheats
    {
        private static bool IsKeyDown(KeyCode keyCode) => Input.GetKeyDown(keyCode);
        
        public static void Update()
        {
#if !UNITY_EDITOR
    
#endif
            Pause();

            if (Input.GetKeyDown(KeyCode.F11)) CheckTimeScale(3f);
            if (Input.GetKeyDown(KeyCode.F12)) CheckTimeScale(10f);
            
            
            if (Input.GetKeyDown(KeyCode.S))
            {
                Models.Get<GameModel>().GetParam(GameParamType.Soft).Change(500);
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                Models.Get<GameModel>().GetParam(GameParamType.Hard).Change(500);
            }
            
            if (Input.GetKeyDown(KeyCode.D))
            {
                GameUI.Get<LevelCompletedWindow>().Open();
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Models.Get<GameModel>().GetProgress(GameParamType.RatingExperience).Change(10);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Models.Get<GameModel>().GetProgress(GameParamType.RatingExperience).Change(50);
            }

            if (Input.GetKeyUp(KeyCode.N)) Models.Get<GameModel>().LevelComplete();
            if (Input.GetKeyUp(KeyCode.W)) GameUI.Get<WinStreakRewardsWindow>().Open();
           
            
            void CheckTimeScale(float value) => Time.timeScale = Time.timeScale > 1f ? 1f : value;
        }

        private static void Pause()
        {
            if (!IsKeyDown(KeyCode.P)) return;
            Time.timeScale = Time.timeScale >= 1 ? 0 : 1;
        }
    }
}