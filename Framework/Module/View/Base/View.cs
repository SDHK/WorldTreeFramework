/****************************************

* 作者： 闪电黑客
* 日期： 2025/10/16

* 描述： 视图基类：绑定、层级与开关。

*/

namespace WorldTree
{
	/// <summary>
	/// 视图状态。OpenAnim、Open、CloseAnim 都表示已打开，子组件和壳都在。
	/// 加载未完成就被关掉时，取消这一次的令牌并回到 Close，不再把 Load 和 Close 叠在一起。
	/// </summary>
	[System.Flags]
	public enum ViewState
	{
		/// <summary>
		/// 关闭。没有组件和壳。
		/// </summary>
		Close = 1,

		/// <summary>
		/// 加载中，意图是打开。还没有组件和壳。
		/// </summary>
		Load = 2,

		/// <summary>
		/// 已打开，正在播打开动画。
		/// </summary>
		OpenAnim = 4,

		/// <summary>
		/// 已打开，没在播。
		/// </summary>
		Open = 8,

		/// <summary>
		/// 已打开，正在播关闭动画。
		/// </summary>
		CloseAnim = 16,
	}

	/// <summary>
	/// 视图基类：作为显示视图打开的初始数据
	/// </summary>
	public abstract class View : Node, IView
		, AsComponentBranch
		, AsChildBranch
		, AsViewBranch
		, ViewOf<View>
		, AsRule<Awake>
		, AsRule<Awake<string>>
		, AsRule<Open>
		, AsRule<Close>
		, AsRule<ViewDrawOpen>
		, AsRule<ViewDrawClose>
		, AsRule<ViewOpenAnim>
		, AsRule<ViewCloseAnim>
		, AsRule<LayerChange>
	{
		/// <summary>
		/// 视图名称。添加时为空则写成类名。控件可在添加时用字符串覆盖。
		/// </summary>
		public string ViewName { get; set; }

		/// <summary>
		/// 当前这趟的壳。由 ViewDrawOpen 写入。壳还池后 Value 为空。
		/// </summary>
		public NodeRef<IViewDraw> Draw;

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

				// 父是层容器：层条目/子视图直接挂在层下
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
		/// 视图状态。OpenAnim、Open、CloseAnim 都表示已打开。
		/// </summary>
		public ViewState ViewState = ViewState.Close;

		/// <summary>
		/// 视图显示。
		/// </summary>
		private bool show = true;

		/// <summary>
		/// 当前这次加载或动画的令牌。同一时间只用一颗。
		/// 加载回来就卸掉，再换一颗给动画。上一趟由那次调用自己拿着，卸的时候不动已经换成下一趟的。
		/// </summary>
		private TreeTaskToken taskToken;

		/// <summary>
		/// 视图是否在加载。只有 Load。壳和组件都还没有。
		/// </summary>
		public bool IsLoad => ViewState == ViewState.Load;

		/// <summary>
		/// 视图是否打开。OpenAnim 和 CloseAnim 也算打开。
		/// </summary>
		public bool IsOpen => ViewState == ViewState.OpenAnim || ViewState == ViewState.Open || ViewState == ViewState.CloseAnim;

		/// <summary>
		/// 视图显隐
		/// </summary>
		/// <remarks>
		/// 只记下意图并发 Show / Hide。壳的激活由平台法则处理。
		/// </remarks>
		public bool IsShow
		{
			get => show;
			set
			{
				if (show == value) return;
				show = value;
				if (!IsOpen) return;
				if (value) NodeRuleHelper.TrySendRule(this, default(Show));
				else NodeRuleHelper.TrySendRule(this, default(Hide));
			}
		}

		/// <summary>
		/// 执行打开
		/// </summary>
		public virtual async TreeTask OnOpen()
		{
			await this.TreeTaskCompleted();
			if (IsOpen)
			{
				// CloseAnim 中重新打开：不加载，不重建组件。正在播的那段看到取消后改播打开动画。
				if (ViewState != ViewState.CloseAnim) return;
				ViewState = ViewState.OpenAnim;
				CancelToken();
				return;
			}
			// 已经在加载。这一次就是当前这次，不用再开。
			if (IsLoad) return;

			ViewState = ViewState.Load;
			TreeTaskToken token = BeginToken();
			try
			{
				TreeTask task = NodeRuleHelper.SendRuleAsync(this, default(ViewDrawOpen));
				task.SetToken(token);
				await task;

				// 只认这一次的令牌。后面可能已经又打开了，不要改它的状态。
				if (token == null || token.IsDisposed || token.State == TokenState.Cancel) return;

				ReleaseToken(token);

				ViewState = ViewState.OpenAnim;
				NodeRuleHelper.TrySendRule(this, default(Open));
				NodeRuleHelper.TrySendRule(this, default(LayerChange));
				if (IsShow) NodeRuleHelper.TrySendRule(this, default(Show));

				await RunAnim();
			}
			catch
			{
				if (token != null && !token.IsDisposed && token.State != TokenState.Cancel) FailOpen();
				else if (IsOpen && taskToken == null) FailOpen();
				throw;
			}
			finally
			{
				ReleaseToken(token);
			}
		}

		/// <summary>
		/// 执行关闭
		/// </summary>
		public virtual async TreeTask OnClose()
		{
			await this.TreeTaskCompleted();
			if (ViewState == ViewState.Close) return;
			if (IsLoad)
			{
				// 还没有组件和壳。取消这次加载，直接关掉。令牌留着，池子回来才看得到。
				CancelToken();
				ViewState = ViewState.Close;
				return;
			}
			if (ViewState == ViewState.OpenAnim || ViewState == ViewState.CloseAnim)
			{
				// 已经在播。只改状态，由正在等的那段接着播。
				if (ViewState == ViewState.OpenAnim)
				{
					ViewState = ViewState.CloseAnim;
					CancelToken();
				}
				return;
			}

			ViewState = ViewState.CloseAnim;
			await RunAnim();
		}

		/// <summary>
		/// 顺着当前动画播。方向被改掉就接着播另一段，窗口保持已打开。
		/// </summary>
		private async TreeTask RunAnim()
		{
			await this.TreeTaskCompleted();
			try
			{
				while (ViewState == ViewState.OpenAnim || ViewState == ViewState.CloseAnim)
				{
					ViewState playing = ViewState;
					TreeTaskToken token = BeginToken();
					try
					{
						TreeTask task = playing == ViewState.OpenAnim
							? NodeRuleHelper.SendRuleAsync(this, default(ViewOpenAnim))
							: NodeRuleHelper.SendRuleAsync(this, default(ViewCloseAnim));
						task.SetToken(token);
						await task;
					}
					finally
					{
						ReleaseToken(token);
					}

					if (ViewState != playing) continue;

					if (playing == ViewState.CloseAnim) CommitClose();
					else ViewState = ViewState.Open;
					break;
				}
			}
			catch
			{
				if (IsOpen) CommitClose();
				throw;
			}
		}

		/// <summary>
		/// 换一颗令牌。上一颗只取消，不在这里卸，卸掉池子就看不到取消。
		/// </summary>
		private TreeTaskToken BeginToken()
		{
			CancelToken();
			this.AddTemp(out taskToken);
			return taskToken;
		}

		/// <summary>
		/// 取消当前这颗令牌。令牌留着，等这次 await 自己结束再卸。
		/// </summary>
		private void CancelToken()
		{
			TreeTaskToken token = taskToken;
			if (token == null || token.IsDisposed) return;
			if (token.State != TokenState.Cancel) token.Cancel();
		}

		/// <summary>
		/// 卸掉这一次的令牌。字段已经换成下一趟时，不动那一趟。
		/// </summary>
		private void ReleaseToken(TreeTaskToken token)
		{
			if (taskToken == token) taskToken = null;
			if (token == null || token.IsDisposed) return;
			if (token.State != TokenState.Cancel) token.Cancel();
			token.Dispose();
		}

		/// <summary>
		/// 打开失败时卸掉已经挂上的组件和壳。
		/// </summary>
		private void FailOpen()
		{
			CancelToken();
			if (IsOpen) CommitClose();
			else if (IsLoad) ViewState = ViewState.Close;
		}

		/// <summary>
		/// 关闭动画播完，卸掉组件和壳。
		/// </summary>
		private void CommitClose()
		{
			if (!IsOpen) return;
			CloseCore();
			NodeRuleHelper.TrySendRule(this, default(ViewDrawClose));
			ViewState = ViewState.Close;
		}

		/// <summary>
		/// 关闭的同步公共部分：Hide、Close，释放显示子级。
		/// </summary>
		private void CloseCore()
		{
			if (IsShow) NodeRuleHelper.TrySendRule(this, default(Hide));
			NodeRuleHelper.TrySendRule(this, default(Close));

			// 显示子级先释放。壳由平台在 ViewDrawClose 里卸掉。
			this.RemoveAllView();
		}

		public virtual void LayerChange() => ViewProxyRule.LayerChanged(this);

		/// <summary>
		/// 释放前执行同步关闭，不播放关闭动画。
		/// 级联回收走的是遍历里的 OnBeforeDispose，因此钩子放这里才能覆盖子级。
		/// </summary>
		public override void OnBeforeDispose()
		{
			bool open = IsOpen;
			ViewState = ViewState.Close;
			CancelToken();
			if (open)
			{
				// 释放不等动画。组件和壳一起拆。
				CloseCore();
				NodeRuleHelper.TrySendRule(this, default(ViewDrawClose));
			}
			base.OnBeforeDispose();
			ViewName = null;
		}

		/// <summary>
		/// 裁剪后清空树派生状态（RemoveNode 不会清 Parent）
		/// </summary>
		public override void OnCutSelf()
		{
			ParentLayer = null;
			Layer = 0;
			show = true;
			ViewState = ViewState.Close;
			CancelToken();
			base.OnCutSelf();
		}
	}



	//========

	/// <summary>
	/// 视图名称。窗口走类名。字符串参数只给控件添加时覆盖。
	/// </summary>
	[NodeRuleTarget(nameof(View))]
	public static partial class ViewRule
	{
		/// <summary>
		/// 没有传入名字时写成类名。Awake 已写入的不覆盖。
		/// </summary>
		class Add : AddRule<View>
		{
			protected override void Execute(View self)
			{
				if (string.IsNullOrEmpty(self.ViewName)) self.ViewName = self.GetType().Name;
			}
		}

		/// <summary>
		/// 控件添加时传入的名字。空字符串忽略，留给 Add 写成类名。
		/// 子类若另写 Awake&lt;string&gt;，这条不会执行，需要自己赋值。
		/// </summary>
		class Awake : AwakeRule<View, string>
		{
			protected override void Execute(View self, string name)
			{
				if (string.IsNullOrEmpty(name)) return;
				self.ViewName = name;
			}
		}
	}

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
