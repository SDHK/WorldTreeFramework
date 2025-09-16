using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 视图接口：作为UI打开的初始数据
	/// </summary>
	public interface IView : INode
	{

		/// <summary>
		/// 视图绑定类型
		/// </summary>
		public long ViewBindType { get; set; }
	}

	/// <summary>
	/// 视图适配绑定接口：桥接具体的UI组件
	/// </summary>
	public interface IViewBind : INode
	{


	}

	//IWidget
	/// <summary>
	/// 视图元素：具体平台实现的UI组件接口，自动反向绑定到IViewBind
	/// </summary>
	public interface IViewElement
	{

	}


	/// <summary>
	/// 视图基类
	/// </summary>
	public abstract class View<VB> : Node, IView
		, ComponentOf<WindowManager>
		, AsComponentBranch
		where VB : class, IViewBind
	{
		/// <summary>
		/// 组件
		/// </summary>
		public NodeRef<IViewBind> ViewBind;
		public long ViewBindType { get; set; }
	}

	/// <summary>
	/// 视图适配绑定基类, 反找IView,反映到具体的UI组件
	/// </summary>
	public abstract class ViewBind : Node, IViewBind
		, ComponentOf<WindowManager>
		, ComponentOf<IView>
		, AsComponentBranch
	{
		/// <summary>
		/// UI实例
		/// </summary>
		public IViewElement ui;

	}
	//========

	/// <summary>
	/// 视图绑定
	/// </summary>
	public class ViewTestBind : ViewBind
	{
		/// <summary>
		/// 测试文本
		/// </summary>
		public Text text;

		/// <summary>
		/// 测试按钮
		/// </summary>
		public Button button;
	}


	/// <summary>
	/// 测试视图
	/// </summary>
	public class ViewTest : View<ViewTestBind>
	{


	}


	//=====================

	/// <summary>
	/// 按钮绑定
	/// </summary>
	public class ButtonBind : ViewBind { }

	/// <summary>
	/// 按钮
	/// </summary>
	public class Button : View<ButtonBind>
	{
		/// <summary>
		/// 点击事件
		/// </summary>
		public RuleUnicast<ISendRule> OnClick;
	}

	/// <summary>
	/// 文本
	/// </summary>
	public class Text : View<ViewBind>
	{
		/// <summary>
		/// 文本内容
		/// </summary>
		public string Content;

		/// <summary>
		/// 改变事件
		/// </summary>
		public RuleUnicast<ISendRule> OnChange;
	}

	/// <summary>
	/// 图片
	/// </summary>
	public class Image : View<ViewBind>
	{
		/// <summary>
		/// 图片地址
		/// </summary>
		public string Url;

		/// <summary>
		/// 改变事件
		/// </summary>
		public RuleUnicast<ISendRule> OnChange;
	}

	/// <summary>
	/// 窗口组
	/// </summary>
	public class ViewGroup : View<ViewBind>
		, AsBranch<ChildBranch>
	{
		/// <summary>
		/// 子窗口数据集合
		/// </summary>
		public HashSet<NodeRef<IView>> nodeHash;
	}
}
