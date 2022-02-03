using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GeneralTools.Editor
{
	public abstract class DevFlagsWindow<T> : EditorWindow
	{
		private ConstantsUpdater _constantsUpdater;

		protected new void Show()
		{
			base.Show();
			titleContent.text = "Dev flags";
		}

		protected void OnGUI()
		{
			if (_constantsUpdater == null) InitConstants();
			//if (GUILayout.Button("Init")) InitConstants();
			EditorUITools.BeginScrollView(this);
			_constantsUpdater.OnGUI();
			EditorUITools.EndScrollView();
		}

		private void InitConstants()
		{
			_constantsUpdater = new ConstantsUpdater(typeof(T));
		}
	}

	public class ConstantsUpdater
	{
		private readonly Type _type;
		private readonly Dictionary<string, bool> _sourceValues, _newValues;
		private readonly string _filePath;

		public ConstantsUpdater(Type type, string path = "")
		{
			_type = type;
			_filePath = GetFilePath(path);
			_sourceValues = GetConstants(type);
			_newValues = new Dictionary<string, bool>(_sourceValues);
		}

		private string GetFilePath(string path)
		{
			if (!string.IsNullOrEmpty(path))
			{
				if (File.Exists(path)) return path;

				path = Path.Combine(Application.dataPath, path);

				if (File.Exists(path)) return path;
			}

			var expectingFileName = $"{_type.Name}.cs";
			var allFiles = Directory.GetFiles(Application.dataPath,
			                                  "*.cs",
			                                  SearchOption.AllDirectories);
			var filePath = allFiles.FirstOrDefault(p => p.Contains(expectingFileName));

			return string.IsNullOrEmpty(filePath) ? string.Empty : filePath;
		}

		public static Dictionary<string, bool> GetConstants(Type type, bool includeStaticFields = true)
		{
			var fields = type.GetFields(BindingFlags.Public |
			                            BindingFlags.Static |
			                            BindingFlags.FlattenHierarchy)
			                 .Where(fi => fi.FieldType == typeof(bool))
			                 .Where(fi => includeStaticFields || (fi.IsLiteral && !fi.IsInitOnly));

			var values = new Dictionary<string, bool>();
			foreach (var fieldInfo in fields)
			{
				var name = fieldInfo.Name;
				var value = (bool)type.GetField(name).GetValue(null);
				values.Add(name, value);
			}

			return values;
		}

		public void OnGUI()
		{
			var changedValues = RedrawAndGetChangedValues();

			ApplyChangedValues(changedValues);

			if (HasChanges() && GUILayout.Button("Apply"))
			{
				ApplyChangesToFile();
			}
		}

		private List<string> RedrawAndGetChangedValues()
		{
			List<string> changedValues = null;
			foreach (var keyValuePair in _newValues)
			{
				var key = keyValuePair.Key;
				var value = keyValuePair.Value;
				var newValue = GUILayout.Toggle(value, key.Replace("_", " "));

				if (newValue == value) continue;
				if (changedValues == null) changedValues = new List<string>() {key};
				else changedValues.Add(key);
			}

			return changedValues;
		}

		private void ApplyChangedValues(List<string> changedValues)
		{
			if (changedValues == null) return;
			foreach (var key in changedValues)
			{
				_newValues[key] = !_newValues[key];
			}
		}

		private bool HasChanges()
		{
			foreach (var keyValuePair in _sourceValues)
			{
				if (_newValues[keyValuePair.Key] != keyValuePair.Value) return true;
			}
			return false;
		}

		private void ApplyChangesToFile()
		{
			var text = File.ReadAllText(_filePath);
			foreach (var pair in _sourceValues)
			{
				var newValue = _newValues[pair.Key];
				if (newValue == pair.Value) continue;
				var prev = $"{pair.Key} = {pair.Value.ToString().ToLower()}";
				var current = $"{pair.Key} = {newValue.ToString().ToLower()}";
				text = text.Replace(prev, current);
			}

			File.WriteAllText(_filePath, text);
			AssetDatabase.Refresh();
		}
	}
}