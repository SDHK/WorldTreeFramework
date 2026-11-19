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

	/// <summary>
	/// 平台加载绘制壳。
	/// </summary>
	/// <remarks>
	/// 资源加载可以走完。回来后若这次任务的令牌已取消，或节点已释放，不要 New 实例。
	/// 判断和 New 之间不能再 await。已经 New 出来才发现取消，把实例还回池，不要交给这次打开。
	/// </remarks>
	public interface ViewDrawOpen : ISendRuleAsync { }

	/// <summary>
	/// 字段组件打开：壳接上之后、Open 之前挂上字段里的子视图。
	/// </summary>
	/// <remarks>
	/// 给生成器写。手写会和生成的那份抢同一条名单。
	/// 加载可以走完。回来后若这次任务的令牌已取消，或节点已释放，不要把组件留给这次打开。
	/// </remarks>
	public interface ViewFieldOpen : ISendRuleAsync { }

	/// <summary>
	/// 绘制壳关闭：同步释放
	/// </summary>
	public interface ViewDrawClose : ISendRule { }

	/// <summary>
	/// 开场动画
	/// </summary>
	/// <remarks>
	/// 窗口已经打开。播放被取消时，这段 await 会返回，RunAnim 按新状态接着播。
	/// </remarks>
	public interface ViewOpenAnim : ISendRuleAsync { }

	/// <summary>
	/// 关闭动画
	/// </summary>
	/// <remarks>
	/// 窗口仍然开着。播放被取消时，这段 await 会返回，RunAnim 按新状态接着播。
	/// </remarks>
	public interface ViewCloseAnim : ISendRuleAsync { }

	/// <summary>
	/// 显示
	/// </summary>
	public interface Show : ISendRule { }

	/// <summary>
	/// 隐藏 
	/// </summary>
	public interface Hide : ISendRule { }

	/// <summary>
	/// 层级变更
	/// </summary>
	public interface LayerChange : ISendRule { }

	/// <summary>
	/// 子视图关闭   
	/// </summary>
	public interface SubViewClose : ISendRule<View> { }
}
