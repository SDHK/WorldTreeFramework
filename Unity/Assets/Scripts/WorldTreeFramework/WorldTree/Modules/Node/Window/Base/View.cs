using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 视图元素：具体平台实现的UI组件接口，自动反向绑定到IViewBind
	/// </summary>
	public interface IViewElement { }

	//V和VB都是纯数据，V是初始化数据，VB负责桥接具体UI组件。

	//Register子级组件> Open，Refresh，Close> UnRegister子级组件，方法写到View里。
	//Close融入Dispose
	//show融入IsActive
	//由于视图都有层级概念，所以分支考虑使用ListNode？
	//父级需要知道子级的Open事件

	//容器组件需要知道子组件的Open事件

	/// <summary>
	/// 视图接口
	/// </summary>
	public interface IView
	{
		/// <summary>
		/// 视图打开
		/// </summary>
		public int Open();
		/// <summary>
		/// 视图关闭
		/// </summary>
		public int Close();
	}

	/// <summary>
	/// 视图基类：作为UI打开的初始数据
	/// </summary>
	public abstract class View : Node, IView
		, AsComponentBranch
		, ChildOf<ViewBind>
		, ListNodeOf<ViewLayerBind>
		, AsRule<Awake>
		, AsRule<Open>
		, AsRule<Close>
	{
		/// <summary>
		/// 视图层级 
		/// </summary>
		public int Layer;

		/// <summary>
		/// 视图绑定
		/// </summary>
		public NodeRef<ViewBind> Bind { get; set; }

		/// <summary>
		/// 视图绑定类型
		/// </summary>
		public long ViewBindType { get; set; }

		/// <summary>
		/// 视图是否打开
		/// </summary>
		public bool IsOpen => Bind != null;

		/// <summary>
		/// 视图显隐
		/// </summary>
		public bool IsShow
		{
			get => Bind != null && Bind.Value.IsActive;
			set => Bind.Value?.SetActive(value);
		}

		public virtual int Open()
		{
			if (IsOpen) return 0;

			if (NodeRuleHelper.TryCallRule(this.Parent.Parent, default(SubViewOpen), out int code))
			{
				if (code != 0) return code;
			}

			if (this.Bind == null)
			{
				this.Bind = new(NodeBranchHelper.AddNode(this, default(ComponentBranch), this.ViewBindType, out ViewBind _));
			}
			return 0;
		}

		public virtual int Close()
		{
			if (!IsOpen) return 0;

			if (NodeRuleHelper.TryCallRule(this.Parent.Parent, default(SubViewClose), out int code))
			{
				if (code != 0) return code;
			}

			this.Bind.Value.Dispose();
			this.Bind = null;
			return 0;
		}

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
