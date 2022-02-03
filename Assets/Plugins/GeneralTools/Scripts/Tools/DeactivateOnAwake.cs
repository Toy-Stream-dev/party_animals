namespace GeneralTools.Tools
{
	public class DeactivateOnAwake : BaseBehaviour
	{
		private void Awake()
		{
			gameObject.SetActive(false);
		}
	}
}