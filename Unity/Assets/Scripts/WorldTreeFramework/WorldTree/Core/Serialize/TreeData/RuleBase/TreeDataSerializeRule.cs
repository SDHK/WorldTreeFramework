/****************************************

* 作者：闪电黑客
* 日期：2024/8/21 11:22

* 描述：

*/
using System;

namespace WorldTree
{

	/// <summary>
	/// 树包序列化法则接口：用于序列化未知泛型，解除AsRule的法则限制
	/// </summary>
	public interface ITreeDataSerialize : IRule { }

	/// <summary>
	/// 树包反序列化法则接口：用于反序列化未知泛型，解除AsRule的法则限制
	/// </summary>
	public interface ITreeDataDeserialize : IRule { }

	/// <summary>
	/// 树数据序列化法则
	/// </summary>
	public interface TreeDataSerialize : ISendRefRule<object, int>, ITreeDataSerialize, ISourceGeneratorIgnore { }

	/// <summary>
	/// 树数据反序列化法则
	/// </summary>
	public interface TreeDataDeserialize : ISendRefRule<object, int>, ITreeDataDeserialize, ISourceGeneratorIgnore { }

	/// <summary>
	/// 树数据序列化法则基类
	/// </summary>
	/// <remarks>打破常规写法，以参数类型为主，支持继承法则</remarks>
	public abstract class TreeDataSerializeRuleBase<N, R, T1> : Rule<T1, R>, ISendRefRule<object, int>
		where N : class, INode, AsRule<R>
		where R : ISendRefRule<object, int>
	{
		/// <summary>
		/// 调用
		/// </summary>
		public virtual void Invoke(INode self, ref object value, ref int nameCode) => Execute(self as N, ref value, ref nameCode);
		/// <summary>
		/// 执行
		/// </summary>
		protected abstract void Execute(N self, ref object value, ref int nameCode);
	}

	/// <summary>
	/// 树数据序列化法则
	/// </summary>
	public abstract class TreeDataSerializeRule<GT> : TreeDataSerializeRuleBase<TreeDataByteSequence, TreeDataSerialize, GT>
	{ }

	/// <summary>
	/// 树数据反序列化法则
	/// </summary>
	public abstract class TreeDataDeserializeRule<GT> : TreeDataSerializeRuleBase<TreeDataByteSequence, TreeDataDeserialize, GT>
	{ }

}
