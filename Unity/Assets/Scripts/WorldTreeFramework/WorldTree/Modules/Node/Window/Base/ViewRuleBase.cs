namespace WorldTree
{

	/// <summary>
	/// 视图元素加载
	/// </summary>
	public interface ViewElementLoad : ISendRule { }

	/// <summary>
	/// 视图元素卸载
	/// </summary>
	public interface ViewElementUnLoad : ISendRule { }

	/// <summary>
	/// 组件注册
	/// </summary>
	public interface ViewRegister : ISendRule { }

	/// <summary>
	/// 组件注销
	/// </summary>
	public interface ViewUnRegister : ISendRule { }

	/// <summary>
	/// 打开
	/// </summary>
	public interface Open : ISendRule { }

	/// <summary>
	/// 关闭
	/// </summary>
	public interface Close : ISendRule { }

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
	public interface SubViewClose : ISendRule<INode> { }

}
