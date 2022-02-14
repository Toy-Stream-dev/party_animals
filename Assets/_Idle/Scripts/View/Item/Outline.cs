using GeneralTools;
using UnityEngine;

namespace _Idle.Scripts.View.Item
{
	public class Outline : BaseBehaviour
	{
		[SerializeField] private Renderer _mesh;
		[Range(0.1f, 4.0f)]
		[SerializeField] private float _width = 1.0f;
		[SerializeField] private Color _color = Color.black;
		[SerializeField] private bool _enableOnStart;

		private readonly int _widthId = Shader.PropertyToID("_OutlineWidth");
		private readonly int _colorId = Shader.PropertyToID("_OutlineColorVertex");
		
		private void Start()
		{
			if (_enableOnStart)
				EnableOutline();
		}

		public void EnableOutline()
		{
			_mesh.material.SetFloat(_widthId, _width);
			_mesh.material.SetColor(_colorId, _color);
		}

		public void DisableOutline()
		{
			_mesh.material.SetFloat("_OutlineWidth", 0);
		}

		public void SetWidth(float newWidth)
		{
			_width = newWidth;
		}

		public void SetColor(Color newColor)
		{
			_color = newColor;
		}
	}
}