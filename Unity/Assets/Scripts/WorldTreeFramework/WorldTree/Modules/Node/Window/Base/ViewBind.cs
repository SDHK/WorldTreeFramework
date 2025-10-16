namespace WorldTree
{
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

		public override void OnAddSelfToTree() => ViewBindProxyRule.OnAddSelfToTree(this);

		public override void Dispose() => ViewBindProxyRule.Dispose(this);

		public override void OnBeforeDispose() => ViewBindProxyRule.OnBeforeDispose(this);

		public override void OnDispose() => ViewBindProxyRule.OnDispose(this);
	}
}
