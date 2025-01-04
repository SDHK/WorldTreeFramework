/****************************************

* 作者：闪电黑客
* 日期：2024/8/14 11:59

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 树包序列化法则接口：用于序列化未知泛型，解除AsRule的法则限制
	/// </summary>
	public interface ITreePackSerialize : IRule { }

	/// <summary>
	/// 树包反序列化法则接口：用于反序列化未知泛型，解除AsRule的法则限制
	/// </summary>
	public interface ITreePackDeserialize : IRule { }

	/// <summary>
	/// 树包序列化非托管法则
	/// </summary>
	public interface TreePackSerializeUnmanaged<T> : ISendRefRule<T>, ITreePackSerialize, ISourceGeneratorIgnore { }

	/// <summary>
	/// 树包反序列化非托管法则
	/// </summary>
	public interface TreePackDeserializeUnmanaged<T> : ISendRefRule<T>, ITreePackDeserialize, ISourceGeneratorIgnore { }

	/// <summary>
	/// 树包序列化非托管法则
	/// </summary>
	public abstract class TreePackSerializeUnmanagedRule<N, T> : SendRefRule<N, TreePackSerializeUnmanaged<T>, T> where N : class, INode, AsRule<TreePackSerializeUnmanaged<T>> { }

	/// <summary>
	/// 树包反序列化非托管法则
	/// </summary>
	public abstract class TreePackDeserializeUnmanagedRule<N, T> : SendRefRule<N, TreePackDeserializeUnmanaged<T>, T> where N : class, INode, AsRule<TreePackDeserializeUnmanaged<T>> { }

	#region 非常规法则

	/// <summary>
	/// 树包序列化法则基类
	/// </summary>
	/// <remarks>打破常规写法，以参数类型为主，支持继承法则</remarks>
	public abstract class TreePackSerializeRuleBase<N, R, T1> : Rule<T1, R>, ISendRefRule<T1>
		where N : class, INode, AsRule<R>
		where R : IRule
	{
		/// <summary>
		/// 调用
		/// </summary>
		public virtual void Invoke(INode self, ref T1 arg1) => Execute(self as N, ref arg1);
		/// <summary>
		/// 执行
		/// </summary>
		protected abstract void Execute(N self, ref T1 arg1);
	}

	/// <summary>
	/// 树包序列化法则
	/// </summary>
	public abstract class TreePackSerializeRule<N, GT> : TreePackSerializeRuleBase<N, ITreePackSerialize, GT>
			where N : class, INode, AsRule<ITreePackSerialize>
	{}

	/// <summary>
	/// 树包反序列化法则
	/// </summary>
	public abstract class TreePackDeserializeRule<N, GT> : TreePackSerializeRuleBase<N, ITreePackDeserialize, GT>
			where N : class, INode, AsRule<ITreePackDeserialize>
	{}

	#endregion
}
