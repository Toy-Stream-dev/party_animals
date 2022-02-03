using _Idle.Scripts.Model.Base;

namespace _Idle.Scripts.Model.Level
{
	public class LevelContainer : ModelsContainer<LevelModel>
	{
		public LevelModel Spawn(int levelIndex)
		{
			var model = Create()
				.SpawnView(MainGame.LevelContainer, true, level => level.LevelIndex.Equals(levelIndex));
				// .Start();
				
			return model;
		}

		public bool TryRemove(int levelIndex)
		{
			var currentLevel = Get(model => model.View.LevelIndex.Equals(levelIndex));
			
			if (currentLevel == null) 
				return false;
			
			Remove(currentLevel);
			return true;
		}

		public bool TryRemove(LevelModel level)
		{
			if (level == null)
				return false;

			Remove(level);
			return true;
		}

		private void Remove(LevelModel level)
		{
			level.DestroyView();
			level.End();
			All.Remove(level);
		}
	}
}