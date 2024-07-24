/****************************************

* 作者：闪电黑客
* 日期：2024/7/23 15:57

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 通知法则基类接口
	/// </summary>
	public interface ISendRefRule<T1> : IRule
	{
		/// <summary>
		/// 调用
		/// </summary>
		void Invoke(INode self, ref T1 arg1);
	}

	/// <summary>
	/// 通知法则基类
	/// </summary>
	public abstract class SendRefRule<N, R, T1> : Rule<N, R>, ISendRefRule<T1>
		where N : class, INode, AsRule<R>
		where R : ISendRefRule<T1>
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
	/// 序列化法则接口：用于序列化未知泛型，解除AsRule的法则限制
	/// </summary>
	public interface ISerialize : IRule { }

	/// <summary>
	/// 反序列化法则接口：用于反序列化未知泛型，解除AsRule的法则限制
	/// </summary>
	public interface IDeserialize : IRule { }

	/// <summary>
	/// 序列化法则
	/// </summary>
	public interface Serialize<T> : ISerialize, ISendRule<T> { }

	/// <summary>
	/// 反序列化法则
	/// </summary>
	public interface Deserialize<T> : IDeserialize, ISendRule<T> { }
}
