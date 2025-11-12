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
		public int OnOpen();
		/// <summary>
		/// 视图关闭
		/// </summary>
		public int OnClose();

		/// <summary>
		/// 视图层级改变 
		/// </summary>
		public void LayerChange();
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
		, AsRule<LayerChange>
		, AsRule<ViewRegister>
		, AsRule<ViewUnRegister>
	{
		/// <summary>
		/// 父级视图层组件
		/// </summary>
		public ViewLayer ParentLayer;

		/// <summary>
		/// 视图层级
		/// </summary>
		public byte Layer;

		/// <summary>
		/// 顶层层数 
		/// </summary>
		public byte LayerTop => (ParentLayer == null) ? (byte)0 : (byte)(ParentLayer.LayerCount);

		public override INode Parent
		{
			get => base.Parent;

			set
			{
				base.Parent = value;
				// 设置父级时自动更新ParentLayer
				if (value?.Parent is ViewLayer parentViewLayer)
				{
					this.ParentLayer = parentViewLayer;
				}
				else if (value?.Parent is View parentView)
				{
					this.ParentLayer = parentView.ParentLayer;
				}
				else // 非视图父级，清空ParentLayer 和 Layer
				{
					this.ParentLayer = null;
					this.Layer = 0;
				}
			}
		}


		/// <summary>
		/// 视图深度：（0-7），自动根据父级计算
		/// </summary>
		public virtual byte Depth => (ParentLayer == null) ? (byte)0 : (byte)(ParentLayer.Depth + 1);


		/// <summary>
		/// 视图排序：自动计算的排序值，值越大越靠前显示
		/// </summary>
		public virtual int Order
		{
			get
			{
				byte depth = Depth;
				int mask = depth == 0 ? 0x07 : 0x0F;  // 掩码限制，防止用户设置错误的Layer值
				int currentLayerOrder = (Layer & mask) << (28 - depth * 4);// 28是4*7深度，统一计算位移
				return ParentLayer == null ? currentLayerOrder : ParentLayer.Order | currentLayerOrder;
			}
		}


		/// <summary>
		/// 视图绑定
		/// </summary>
		public NodeRef<ViewBind> Bind { get; set; }

		/// <summary>
		/// 视图绑定类型
		/// </summary>
		public virtual long ViewBindType { get; }

		/// <summary>
		/// 视图是否打开
		/// </summary>
		public bool IsOpen => !Bind.IsNull;

		/// <summary>
		/// 视图显隐
		/// </summary>
		public bool IsShow
		{
			get => !Bind.IsNull && Bind.Value.IsActive;
			set => Bind.Value?.SetActive(value);
		}


		public virtual int OnOpen()
		{
			if (IsOpen) return 0;

			if (this.Bind.IsNull)
			{
				this.Bind = new(NodeBranchHelper.AddNode(this, default(ComponentBranch), this.ViewBindType, this.ViewBindType, out INode _) as ViewBind);
			}
			return 0;
		}

		public virtual int OnClose()
		{
			if (!IsOpen) return 0;

			this.Bind.Value.Dispose();
			this.Bind = null;
			return 0;
		}

		public virtual void LayerChange() => ViewProxyRule.LayerChanged(this);
	}

	/// <summary>
	/// 视图主体基类 
	/// </summary>
	public abstract class ViewObject : View
		, ComponentOf<ViewLayerBind>
	{

	}

	/// <summary>
	/// 视图主体基类 
	/// </summary>
	public abstract class ViewObject<VB> : ViewObject
		, AsComponentBranch
		where VB : ViewBind
	{
		public override long ViewBindType => this.TypeToCode<VB>();

	}


	/// <summary>
	/// 视图组件基类 
	/// </summary>
	public abstract class ViewWidget : View { }


	/// <summary>
	/// 视图泛型基类
	/// </summary>
	public abstract class View<VB> : View
		, AsComponentBranch
		where VB : ViewBind
	{
		/// <summary>
		/// 视图绑定
		/// </summary>
		public VB ViewBind => Bind.Value as VB;

		public override long ViewBindType => this.TypeToCode<VB>();
	}




	//========

	/// <summary>
	/// 视图绑定
	/// </summary>
	public class ViewTestWindowBind : ViewBind
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
	public class ViewTestWindow : ViewObject<ViewTestWindowBind>
	{
		/// <summary>
		/// 名称
		/// </summary>
		public string Name;

	}

	//================================

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
