using GeneralTools.Model;
using GeneralTools.View;
using Sirenix.OdinInspector;
#if ODIN_INSPECTOR

#endif

namespace Plugins.GeneralTools.Scripts.View
{
	public class ViewWithModel<T> : BaseView, IWithModel where T : BaseModel
	{
#if ODIN_INSPECTOR
		[ReadOnly, ShowInInspector]
#endif
		public T Model { get; private set; }

		public virtual ViewWithModel<T> SetModel(T model)
		{
			Model = model;
			return this;
		}

		public void SetModel(BaseModel model)
		{
			SetModel(model as T);
		}

		protected virtual void RemoveModel()
		{
			Model = null;
		}

		public override void Clear()
		{
			RemoveModel();
		}
	}
}