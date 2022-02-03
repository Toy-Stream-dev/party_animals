namespace GeneralTools.Model
{
	public abstract class BaseComponent<T> : BaseComponent where T : BaseModel
	{
		protected new virtual T Model { get; }

		protected BaseComponent(T model) : base(model)
		{
			Model = model;
		}
	}

	public abstract class BaseComponent
	{
		protected virtual BaseModel Model { get; }

		protected BaseComponent(BaseModel model)
		{
			Model = model;
		}

		public virtual void Init()
		{
		}

		public virtual void Start()
		{
		}

		public virtual void Update(float deltaTime)
		{
		}

		public virtual void FixedUpdate(float deltaTime)
		{
		}

		public virtual void End()
		{
		}
	}
}