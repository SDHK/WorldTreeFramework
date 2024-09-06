/****************************************

* 作者：闪电黑客
* 日期：2024/8/21 11:22

* 描述：

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// 树数据序列化法则接口：用于序列化未知泛型，解除AsRule的法则限制
	/// </summary>
	public interface ITreeDataSerialize : IRule { }

	/// <summary>
	/// 树数据反序列化法则接口：用于反序列化未知泛型，解除AsRule的法则限制
	/// </summary>
	public interface ITreeDataDeserialize : IRule { }


	/// <summary>
	/// 树数据序列化非托管法则
	/// </summary>
	public interface TreeDataSerialize : ISendRefRule<object>, ITreeDataSerialize, ISourceGeneratorIgnore { }

	/// <summary>
	/// 树数据反序列化非托管法则
	/// </summary>
	public interface TreeDataDeserialize : ISendRefRule<object>, ITreeDataDeserialize, ISourceGeneratorIgnore { }



	/// <summary>
	/// 树数据序列化法则基类
	/// </summary>
	/// <remarks>打破常规写法，以参数类型为主，支持继承法则</remarks>
	public abstract class TreeDataSerializeRuleBase<N, R, T1> : Rule<T1, R>, ISendRefRule<object>
		where N : class, INode, AsRule<R>
		where R : ISendRefRule<object>
	{
		/// <summary>
		/// 调用
		/// </summary>
		public virtual void Invoke(INode self, ref object arg1) => Execute(self as N, ref arg1);
		/// <summary>
		/// 执行
		/// </summary>
		protected abstract void Execute(N self, ref object arg1);
	}

	/// <summary>
	/// 树数据序列化法则
	/// </summary>
	public abstract class TreeDataSerializeRule<N, GT> : TreeDataSerializeRuleBase<N, TreeDataSerialize, GT>
			where N : class, INode, AsRule<TreeDataSerialize>
	{ }

	/// <summary>
	/// 树数据反序列化法则
	/// </summary>
	public abstract class TreeDataDeserializeRule<N, GT> : TreeDataSerializeRuleBase<N, TreeDataDeserialize, GT>
			where N : class, INode, AsRule<ITreeDataDeserialize>
	{ }

}
