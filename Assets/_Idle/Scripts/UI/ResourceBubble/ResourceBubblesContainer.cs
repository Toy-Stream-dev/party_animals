using System;
using System.Collections.Generic;
using GeneralTools.Pooling;
using GeneralTools.Tools;
using Plugins.GeneralTools.Scripts.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Idle.Scripts.UI.ResourceBubble
{
	public enum BubbleTypes
	{
		Soft,
		Hard
	}

	[Serializable]
    public class ResourceBubbleSetup
    {
        public BubbleTypes Type;
        public RectTransform Target;
        public int Conversion = 1;
        public int Max = 10;
    }

    public class ResourceBubblesContainer : BaseWindow
    {
	    [Range(0f, 1f), SerializeField] private float _firstPhaseDuration = 0.2f;
	    [Range(0f, 1f), SerializeField] private float _secondPhaseDelay = 0.2f;
	    [Range(0f, 1f), SerializeField] private float _secondPhaseDuration = 0.2f;
	    [Range(0f, 1f), SerializeField] private float _animDelay = 0.02f;

	    [SerializeField] private int _range = 100;

	    [SerializeField] private List<ResourceBubbleSetup> _setups;
	    private readonly List<ResourceBubbleUI> _bubbles = new List<ResourceBubbleUI>();

	    public override void Init()
	    {
		    Pool.Spawn<ResourceBubbleUI>(30);
		    base.Init();

		    this.Activate();
	    }

	    public void Show(BubbleTypes type, int count)
	    {
		    if (count == 0) return;

		    var pos = GetPossibleButtonCenter() ??
		              GetPossibleScreenCenter() ??
		              Input.mousePosition;

		    Show(pos, type, count);
	    }

	    public void Show(BubbleTypes type, float count, Vector2 pos)
	    {
		    if (count == 0) return;
		    Show(pos, type, count);
	    }

	    private void Show(Vector2 startPos, BubbleTypes type, float count)
	    {
		    if (count < 0) return;

		    var value = count;

		    var setup = _setups.Find(s => s.Type == type);
		    if (setup == null) return;

		    this.Activate();

		    count = Mathf.Clamp((int) (count / setup.Conversion), 1, setup.Max);

		    var target = setup.Target.position;
		    var isFirst = true;
		    var delay = 0f;

		    while (count-- > 0)
		    {
			    ShowBubble(startPos, target)
				    .Redraw(type, value)
				    .SetRange(_range)
				    .SetDelay(delay);

			    delay += _animDelay;

			    if (!isFirst) continue;
			    isFirst = false;
		    }
	    }

	    private Vector2? GetPossibleButtonCenter()
	    {
		    var currentEventSystem = EventSystem.current;
		    var pointerData = new PointerEventData(currentEventSystem)
		    {
			    pointerId = -1, position = Input.mousePosition,
		    };

		    var results = new List<RaycastResult>();
		    EventSystem.current.RaycastAll(pointerData, results);

		    foreach (var result in results)
		    {
			    if (result.gameObject.GetComponent<Button>() == null) continue;
			    return result.gameObject.GetComponent<RectTransform>().position;
		    }

		    return null;
	    }

	    private Vector2? GetPossibleScreenCenter()
	    {
		    return new Vector2(Screen.width / 2f, Screen.height / 2f);
	    }

	    private ResourceBubbleUI ShowBubble(Vector2 startPos, Vector2 targetPos)
	    {
		    var bubble = Pool.Pop<ResourceBubbleUI>(transform);
		    bubble.OnEventCompleted += OnCompleted;
		    bubble.SetPositions(startPos, targetPos)
			    .SetTimings(_firstPhaseDuration, _secondPhaseDuration, _secondPhaseDelay)
			    .Play();
		    
		    _bubbles.Add(bubble);

		    return bubble;
	    }

	    private void OnCompleted(ResourceBubbleUI bubble)
	    {
		    bubble.OnEventCompleted -= OnCompleted;
		    bubble.PushToPool();
		    _bubbles.Remove(bubble);
	    }
    }
}