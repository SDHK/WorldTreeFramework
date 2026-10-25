/****************************************

* 作者： 闪电黑客
* 日期： 2025/10/16

* 描述： 视图基类：绑定、层级与开关。

*/

namespace WorldTree
{
	/// <summary>
	/// 视图基类：作为UI打开的初始数据
	/// </summary>
	public abstract class View : Node, IView
		, AsComponentBranch
		, AsChildBranch
		, ChildOf<View>
		, AsRule<Awake>
		, AsRule<Open>
		, AsRule<Close>
		, AsRule<LayerChange>
	{
		/// <summary>
		/// 父级视图层组件
		/// </summary>
		public ViewLayer ParentLayer;

		public override INode Parent
		{
			get => base.Parent;
			set
			{
				base.Parent = value;

				// 层永远是排序域的根，不继承宿主的层
				if (this is ViewLayer)
				{
					ParentLayer = null;
					return;
				}

				// 父是层容器：窗口/子视图直接挂在层下
				if (value is ViewLayer parentLayer)
				{
					ParentLayer = parentLayer;
					return;
				}

				// 父本身就是 View：继承父视图所在的层与层内位置，Order 与父级相同（同层渲染）
				if (value is View directParentView)
				{
					ParentLayer = directParentView.ParentLayer;
					Layer = directParentView.Layer;
					return;
				}

				// 非视图父级，清空ParentLayer 和 Layer
				ParentLayer = null;
				Layer = 0;
			}
		}

		/// <summary>
		/// 视图层级（0-15）
		/// </summary>
		public byte Layer;

		/// <summary>
		/// 顶层层数 
		/// </summary>
		public byte LayerTop => (ParentLayer == null) ? (byte)0 : (byte)(ParentLayer.LayerCount);

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
				int mask = depth == 0 ? 0x07 : 0x0F;  // 掩码限制，防止用户设置错误的Layer值，0x07是因为int的最高位最大只能是7。
				int currentLayerOrder = (Layer & mask) << (28 - depth * 4);// 28是4*7深度，统一计算位移
				return ParentLayer == null ? currentLayerOrder : ParentLayer.Order | currentLayerOrder;
			}
		}


		/// <summary>
		/// 视图绑定
		/// </summary>
		public NodeRef<ViewBind> Bind { get; set; }

		/// <summary>
		/// 视图是否打开
		/// </summary>
		public bool IsOpen => !Bind.IsNull;

		/// <summary>
		/// 视图显隐
		/// </summary>
		public bool IsShow
		{
			get
			{
				ViewBind bind = Bind.Value;
				return bind != null && bind.IsActive;
			}
			set
			{
				ViewBind bind = Bind.Value;
				if (bind == null || bind.IsActive == value) return;
				bind.SetActive(value);
				if (value) NodeRuleHelper.TrySendRule(this, default(Show));
				else NodeRuleHelper.TrySendRule(this, default(Hide));
			}
		}

		public virtual int OnOpen()
		{
			if (IsOpen) return 0;

			if (this.Bind.IsNull) this.Bind = this.AddComponent(out ViewBind _);
			NodeRuleHelper.TrySendRule(this, default(Open));
			NodeRuleHelper.TrySendRule(this, default(LayerChange));
			if (IsShow) NodeRuleHelper.TrySendRule(this, default(Show));
			return 0;
		}

		public virtual int OnClose()
		{
			if (!IsOpen) return 0;

			if (IsShow) NodeRuleHelper.TrySendRule(this, default(Hide));
			NodeRuleHelper.TrySendRule(this, default(Close));
			this.Bind.Value.Dispose();
			this.Bind = null;
			return 0;
		}

		public virtual void LayerChange() => ViewProxyRule.LayerChanged(this);

		/// <summary>
		/// 释放前先关闭：发 Close、销毁 Bind。
		/// 级联回收走的是遍历里的 OnBeforeDispose，因此钩子放这里才能覆盖子级。
		/// </summary>
		public override void OnBeforeDispose()
		{
			OnClose();
			base.OnBeforeDispose();
		}

		/// <summary>
		/// 裁剪后清空树派生状态（RemoveNode 不会清 Parent）
		/// </summary>
		public override void OnCutSelf()
		{
			ParentLayer = null;
			Layer = 0;
			base.OnCutSelf();
		}
	}



	//========

	public static class ViewProxyRule
	{

		/// <summary>
		/// 视图层级变更时调用 
		/// </summary>
		public static void LayerChanged(View self)
		{
			//节点释放前序遍历处理,节点释放后续遍历处理
			NodeBranchTraversalHelper.TraversalPreorder(self, OnLayerChanged, false);
		}
		/// <summary>
		/// 视图层级变更时调用 
		/// </summary>
		public static void OnLayerChanged(INode self)
		{
			if (self is View) NodeRuleHelper.TrySendRule(self, default(LayerChange));
		}

	}
}
