using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR

#endif

namespace GeneralTools.Tools
{
    public static class UnityEditorTools
    {
        public enum FilterTypes
        {
            AudioClip,
            Prefab,
            Sprite,
            Texture
        }

        private static readonly Dictionary<FilterTypes, string> _fileFilters = new Dictionary<FilterTypes, string>()
        {
            {FilterTypes.AudioClip, "t:AudioClip"},
            {FilterTypes.Prefab, "t:Prefab"},
            {FilterTypes.Texture, "t:Texture"},
            {FilterTypes.Sprite, "t:Sprite"}
        };

        public static Sprite CreateSprite(Texture2D texture2D, float pixelPerUnit = 100.0f)
        {
            return Sprite.Create(texture2D,
                new Rect(0.0f, 0.0f, texture2D.width, texture2D.height),
                new Vector2(0.5f, 0.5f), pixelPerUnit);
        }
        
        public static List<T> Find<T>(string[] paths, FilterTypes filter) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            return AssetDatabase.FindAssets(_fileFilters[filter], paths)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<T>).Where(asset => asset != null)
                .ToList();
#else
            return null;
#endif
        }
    }
}