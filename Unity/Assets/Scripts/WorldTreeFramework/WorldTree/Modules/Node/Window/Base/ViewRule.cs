namespace WorldTree
{
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
}
