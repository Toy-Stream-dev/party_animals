using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GeneralTools.Tools
{
	public static class UnityExtensionMethods
	{

		public static void Activate(this GameObject go) => go.SetActive(true);

		public static void Deactivate(this GameObject go) => go.SetActive(false);

		public static T Activate<T>(this T component) where T : Component
		{
			component.gameObject.SetActive(true);
			return component;
		}

		public static bool IsActivate<T>(this T component) where T : Component
		{
			return component.gameObject.activeInHierarchy;
		}

		public static T Deactivate<T>(this T component) where T : Component
		{
			component.gameObject.SetActive(false);
			return component;
		}

		public static T SetActive<T>(this T component, bool active) where T : Component
		{
			component.gameObject.SetActive(active);
			return component;
		}

		public static T Copy<T>(this T source,
		                        Transform parent = null,
		                        string name = "",
		                        bool activate = true) where T : Component
		{
			if (source == null)
			{
				Debug.LogError("Null prefab");
				return null;
			}

#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				if (PrefabUtility.InstantiatePrefab(source, parent) is T copy)
				{
					copy.SetActive(activate);
					return copy;
				}
			}
#endif

			return source.gameObject.Copy(parent, name, activate).GetComponent<T>();
		}

		public static GameObject Copy(this GameObject source,
		                              Transform parent = null,
		                              string name = "",
		                              bool activate = true)
		{
			if (source == null)
			{
				Debug.LogError("Null prefab");
				return null;
			}

			var obj = Object.Instantiate(source, parent != null ? parent : source.transform.parent, false);
			obj.SetActive(activate);

			if (!string.IsNullOrEmpty(name))
			{
				obj.name = name;
			}
#if UNITY_EDITOR
			else
			{
				obj.name = obj.name.Replace("(Clone)", "");
			}
#endif

			return obj;
		}

		public static T SetParent<T>(this T behaviour, Transform parent, bool worldPositionStays = true) where T : BaseBehaviour
		{
			behaviour.transform.SetParent(parent, worldPositionStays);
			return behaviour;
		}

		public static void Destroy(this GameObject go, float delay = 0f)
		{
			if (go == null)
			{
				Debug.LogError("Can't destroy null obj");
				return;
			}

			if (!Application.isPlaying)
			{
				Object.DestroyImmediate(go, true);
				return;
			}

			if (delay > 0f)
			{
				Object.Destroy(go, delay);
			}
			else
			{
				Object.Destroy(go);
			}
		}

		public static void Destroy<T>(this GameObject go, float delay = 0f) where T : Component
		{
			if (go == null)
			{
				Debug.LogError("Can't destroy null obj");
				return;
			}

			go.GetComponent<T>().DestroyComponent();
		}

		public static void DestroyComponent(this Component component, float delay = 0f)
		{
			if (component == null)
			{
				Debug.LogError("Can't destroy null obj");
				return;
			}

			if (!Application.isPlaying)
			{
				Object.DestroyImmediate(component);
				return;
			}

			if (delay > 0f)
			{
				Object.Destroy(component, delay);
			}
			else
			{
				Object.Destroy(component);
			}
		}

		public static void DestroyGO<T>(this T component, float delay = 0f) where T : Component
		{
			if (component == null)
			{
				Debug.LogError("Can't destroy null component");
				return;
			}
            component.gameObject.Destroy(delay);
		}

		public static List<GameObject> GetChilds(this GameObject gameObject)
		{
			var result = new List<GameObject>();
			foreach (Transform child in gameObject.transform)
			{
				result.Add(child.gameObject);
			}
			return result;
		}

		public static Texture2D CopyAndRotate(this Texture2D texture)
		{
			var newTexture = new Texture2D(texture.height, texture.width, texture.format, false);
			for (var i = 0; i < texture.width; i++)
			{
				for (var j = 0; j < texture.height; j++)
				{
					newTexture.SetPixel(j, i, texture.GetPixel(texture.width - i, j));
				}
			}

			newTexture.Apply();
			return newTexture;
		}

		public static Texture2D CopyAndColorize(this Texture2D texture, Color color)
		{
			var w = texture.width;
			var h = texture.height;
			Texture2D newTexture = new Texture2D(w, h);
			for (int y = 0; y < h; y++)
			{
				for (int x = 0; x < w; x++)
				{
					newTexture.SetPixel(x, y, color);
				}
			}

			newTexture.Apply();
			return newTexture;
		}

		public static Texture2D ToTexture2D(this Sprite sprite, Vector2? size = null)
		{
			if (sprite == null) return null;
			var texture = new Texture2D((int) sprite.rect.width, (int) sprite.rect.height);
			var pixels = sprite.texture.GetPixels((int) sprite.rect.x,
				(int) sprite.rect.y,
				(int) sprite.rect.width,
				(int) sprite.rect.height);
			texture.SetPixels(pixels);
			texture.Apply();

			if (!size.HasValue) return texture;
			texture.Resize(size.Value.x, size.Value.y);
			texture.Apply();
			return texture;
		}
		
		public static Texture2D Resize(this Texture2D source, float width, float height)
		{
			source.filterMode = FilterMode.Bilinear;
			RenderTexture rt = RenderTexture.GetTemporary((int)width, (int)height);
			rt.filterMode = FilterMode.Bilinear;
			RenderTexture.active = rt;
			Graphics.Blit(source, rt);
			var nTex = new Texture2D((int)width, (int)height);
			nTex.ReadPixels(new Rect(0, 0, width, width), 0,0);
			nTex.Apply();
			RenderTexture.active = null;
			return nTex;
		}
		
		public static void SetLayerRecursively(this Transform transform, int layer)
		{
			foreach (var t in transform.GetComponentsInChildren<Transform>(true))
			{
				t.gameObject.layer = layer;
			}
		}
		
		public static void SetLayerRecursively(this GameObject gameObject, int layer)
		{
			foreach (var t in gameObject.GetComponentsInChildren<Transform>(true))
			{
				t.gameObject.layer = layer;
			}
		}
		
		public static Vector3 ToVector3(this Ray ray)
		{
			return new Vector3(ray.origin.x, ray.origin.y, ray.origin.z);
		}
        
		public static Vector2 ToVector2(this Ray ray)
		{
			return new Vector2(ray.origin.x, ray.origin.y);
		}

		public static bool HasLayer(this LayerMask layerMask, int layer)
		{
			return layerMask == (layerMask | (1 << layer));
		}

		public static List<int> HasLayers(this LayerMask layerMask)
		{
			var hasLayers = new List<int>();
			for (var i = 0; i < 32; i++)
				if (layerMask == (layerMask | (1 << i)))
					hasLayers.Add(i);

			return hasLayers;
		}

		public static AnimationClip GetAnimation(this Animator animator, string animationName)
		{
			return animator.runtimeAnimatorController.animationClips.FirstOrDefault(clip => clip.name == animationName);
		}

		public static int GetRandom(this RangeInt value)
		{
			return Random.Range(value.start, value.length);
		}
	}
}