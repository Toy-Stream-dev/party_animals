using _Idle.Scripts.Balance;
using _Idle.Scripts.Enums;
using _Idle.Scripts.Model;
using _Idle.Scripts.Saves;
using _Idle.Scripts.Tools;
using GeneralTools;
using GeneralTools.Model;
using GoogleParse;
using UnityEditor;
using UnityEngine;
using static GeneralTools.Editor.EditorUITools;
using LocalizationsContainer = _Idle.Scripts.Tools.LocalizationsContainer;

namespace _Idle.Scripts.Editor.Windows
{
	public class DevToolsWindow : EditorWindow
	{
		private string _command;
		private string _value;

		[MenuItem("_Idle/Dev tools", false, -10)]
		static void Init()
		{
			var window = (DevToolsWindow)GetWindow(typeof(DevToolsWindow));
			window.Show();
			window.titleContent.text = "Dev tools";
		}

		private void OnGUI()
		{
			// if (Button("Run test")) TestTools.RunMagicTest();
			DrawInGame();
			DrawInEditor();

			DrawGoogleParsing();

			if (Button("Parse prefabs and icons"))
			{
				Prefabs.InitOrJustParsePrefabs();
				GameResources.Instance.Parse();
				Select(GameResources.Instance);
			}

			Space();

			DrawOpenSection();
			DrawSaves();
			DrawSkips();
		}

		private void DrawSaves()
		{
			BeginHorizontal();

			if (Application.isPlaying)
			{
				if (Button("Save")) GameSave.Save();
			}
			else
			{
				if (GameSave.Exists() && Button("Delete")) GameSave.DeleteSave();
				if (GameSave.BackupExists() && Button("Restore")) GameSave.Restore();
			}

			if (GameSave.Exists() && Button("Backup")) GameSave.Backup();

			EndHorizontal();
			Space();
		}

		private void DrawInGame()
		{
			if (!Application.isPlaying) return;

			BeginHorizontal();

			_value = TextField(_value, 100);

			var value = double.TryParse(_value, out var v) ? v : -1;

			EndHorizontal();

			Space();
		}

		private void DrawInEditor()
		{
			if (Application.isPlaying) return;
		}

		private void DrawSkips()
		{
			if (Application.isPlaying || !GameSave.Exists()) return;

			BeginHorizontal();
			Label("Skip:");
			// if (Button("1m")) TestTools.SkipTime(1);
			// if (Button("1h")) TestTools.SkipTime(60);
			// if (Button("1d")) TestTools.SkipTime(60 * 24);
			EndHorizontal();
		}

		private void Select(Object obj)
		{
			Selection.activeObject = obj;
		}

		private void DrawGoogleParsing()
		{
			if (Application.isPlaying) return;

			var parser = GoogleParser.Instance;

			BeginHorizontal();
			Label("Parse google:");
			if (Button("All")) parser.ParseAllTables();
			foreach (var tableName in parser.GetTableNames())
			{
				if (Button(tableName))
				{
					parser.ParseTable(tableName);
				}
			}
			EndHorizontal();
		}

		private void DrawOpenSection()
		{
			BeginHorizontal();
			Label("Open:");
			if (Button("Balance")) Select(GameBalance.Instance);
			if (Button("Prefabs")) Select(Prefabs.Instance);
			if (Button("Resources")) Select(GameResources.Instance);
			if (Button("Loc")) Select(LocalizationsContainer.Instance);
			EndHorizontal();
			Space();
		}
	}
}