using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace _Idle.Scripts
{
    [CreateAssetMenu(fileName = "GameResources", menuName = "IdleCity/GameResources", order = 1)]
    public class GameResources : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private List<TextureDataValues> _spritesData;

        [SerializeField] private Sprite _placeholder;

        private static GameResources _instance;
        
        public static GameResources Instance
        {
            get
            {
                if (_instance == null) Init();
                return _instance;
            }
        }

        public static void Init()
        {
            if (_instance != null) return;
            _instance = Resources.Load<GameResources>("GameResources");
        }
        
#if UNITY_EDITOR
        [ContextMenu("Save")]
        public void Save()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("_Idle/Game resources", false, 1)]
        public static void SelectManager()
        {
            Selection.activeObject = Instance;
        }
#endif
        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
        }

        [Button("Parse")]
        public void Parse()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
        
        public Sprite GetSprite<T>(T type) where T: Enum
        {
            var result = _spritesData.FirstOrDefault(s => s.Key.Equals(type.ToString()));
            return result == null ? _placeholder : result.Value;
        }
        
        public Sprite GetSprite(string key)
        {
            var result = _spritesData.FirstOrDefault(s => s.Key == key);
            return result == null ? _placeholder : result.Value;
        }
    }
    
    public static class GameResourcesTools
    {
        public static Sprite GetSprite(this Enum type)
        {
            return GameResources.Instance.GetSprite(type);
        }
        
        public static Sprite GetSprite(this string key)
        {
            return GameResources.Instance.GetSprite(key);
        }
    }

    [Serializable]
    public class TextureDataValues
    {
        public string Key;
        public Sprite Value;
    }
}