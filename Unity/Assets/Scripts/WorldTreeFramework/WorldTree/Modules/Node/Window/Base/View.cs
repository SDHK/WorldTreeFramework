using System.Collections.Generic;

namespace WorldTree
{

	/// <summary>
	/// 打开
	/// </summary>
	public interface Open : ISendRule { }

	/// <summary>
	/// 关闭
	/// </summary>
	public interface Close : ISendRule { }



	//=========

	//IWidget
	/// <summary>
	/// 视图元素：具体平台实现的UI组件接口，自动反向绑定到IViewBind
	/// </summary>
	public interface IViewElement
	{

	}


	//V和VB都是纯数据，V是初始化数据，VB负责桥接具体UI组件。

	//Register子级组件> Open，Refresh，Close> UnRegister子级组件，方法写到View里。
	//Close融入Dispose



	/// <summary>
	/// 视图基类：作为UI打开的初始数据
	/// </summary>
	public abstract class View : Node
		, AsComponentBranch
		, ChildOf<ViewBind>
		, AsRule<Awake>
		, AsRule<Open>
	{
		/// <summary>
		/// 组件
		/// </summary>
		public NodeRef<ViewBind> Bind { get; set; }
		/// <summary>
		/// 视图绑定类型
		/// </summary>
		public long ViewBindType { get; set; }


		/// <summary>
		/// 是否打开
		/// </summary>
		public bool IsOpen;

		/// <summary>
		/// 是否显示？？？
		/// </summary>
		public bool IsShow;




	}


	/// <summary>
	/// 视图泛型基类
	/// </summary>
	public abstract class View<VB> : View
		, ComponentOf<WindowManager>
		, AsComponentBranch
		where VB : ViewBind
	{
		/// <summary>
		/// 视图绑定
		/// </summary>
		public VB ViewBind => Bind.Value as VB;


	}


	/// <summary>
	/// 视图适配绑定基类, 桥接具体的UI组件
	/// </summary>
	public abstract class ViewBind : Node
		, ComponentOf<WindowManager>
		, ComponentOf<View>
		, AsComponentBranch
		, AsChildBranch
		, AsRule<Awake>
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
		/// <summary>
		/// 名称
		/// </summary>
		public string Name;

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
	/// 按钮绑定
	/// </summary>
	public class ViewGroupBind : ViewBind { }

	/// <summary>
	/// 窗口组
	/// </summary>
	public class ViewGroup : View<ViewGroupBind>
		, AsBranch<ChildBranch>
	{
		/// <summary>
		/// 子窗口数据集合
		/// </summary>
		public HashSet<NodeRef<View>> nodeHash;
	}
}
