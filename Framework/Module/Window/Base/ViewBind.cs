/****************************************

* 作者： 闪电黑客
* 日期： 2025/10/16 19:49

* 描述： 视图适配绑定基类
* 
* 作为具体UI组件的桥接，负责将具体UI组件与视图数据进行绑定
* 本身不包含具体UI逻辑，所有UI逻辑应在具体的View中实现

*/
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
		, AsRule<ViewRegister>
		, AsRule<ViewUnRegister>
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
