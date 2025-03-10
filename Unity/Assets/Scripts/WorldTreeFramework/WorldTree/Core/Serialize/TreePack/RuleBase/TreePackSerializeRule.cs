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
	public interface ITreePackSerialize : ISendRefRule { }

	/// <summary>
	/// 树包反序列化法则接口：用于反序列化未知泛型，解除AsRule的法则限制
	/// </summary>
	public interface ITreePackDeserialize : ISendRefRule { }

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
	public abstract class TreePackSerializeUnmanagedRule<T> : SendRefRule<TreePackByteSequence, TreePackSerializeUnmanaged<T>, T> { }

	/// <summary>
	/// 树包反序列化非托管法则
	/// </summary>
	public abstract class TreePackDeserializeUnmanagedRule<T> : SendRefRule<TreePackByteSequence, TreePackDeserializeUnmanaged<T>, T> { }

	#region 非常规法则


	/// <summary>
	/// 树包序列化法则
	/// </summary>
	/// <remarks>打破常规写法，以参数类型为主，支持继承法则</remarks>
	public abstract class TreePackSerializeRule<GT> : Rule<GT, ITreePackSerialize>, ISendRefRule<GT>
	{
		/// <summary>
		/// 调用
		/// </summary>
		public virtual void Invoke(INode self, ref GT arg1) => Execute(self as TreePackByteSequence, ref arg1);
		/// <summary>
		/// 执行
		/// </summary>
		protected abstract void Execute(TreePackByteSequence self, ref GT arg1);
	}

	/// <summary>
	/// 树包反序列化法则
	/// </summary>
	/// <remarks>打破常规写法，以参数类型为主，支持继承法则</remarks>
	public abstract class TreePackDeserializeRule<GT> : Rule<GT, ITreePackDeserialize>, ISendRefRule<GT>
	{
		/// <summary>
		/// 调用
		/// </summary>
		public virtual void Invoke(INode self, ref GT arg1) => Execute(self as TreePackByteSequence, ref arg1);
		/// <summary>
		/// 执行
		/// </summary>
		protected abstract void Execute(TreePackByteSequence self, ref GT arg1);
	}
	#endregion
}
