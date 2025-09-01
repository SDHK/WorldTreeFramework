namespace WorldTree
{
	/// <summary>
	/// 视图接口
	/// </summary>
	public interface IView : INode
	{
		/// <summary>
		/// 视图类型
		/// </summary>
		public long ViewBindType { get; }
	}

	/// <summary>
	/// 视图适配绑定接口
	/// </summary>
	public interface IViewBind : INode
	{
	}



	/// <summary>
	/// 视图
	/// </summary>
	public abstract class View<VB> : Node, IView
		, ComponentOf<WindowManager>
		, AsComponentBranch
		where VB : class, IViewBind
	{
		public long ViewBindType => this.TypeToCode<VB>();

		/// <summary>
		/// 组件
		/// </summary>
		public NodeRef<VB> ViewBind;

	}

	/// <summary>
	/// 视图适配绑定
	/// </summary>
	public abstract class ViewBind : Node, IViewBind
		, ComponentOf<WindowManager>
		, AsComponentBranch
	//where V : class, IView
	{

	}


	/// <summary>
	/// 按钮
	/// </summary>
	public class Button : View<ViewBind>
	{
		/// <summary>
		/// 点击事件
		/// </summary>
		public RuleUnicast<ISendRule> OnClick;
	}
}
