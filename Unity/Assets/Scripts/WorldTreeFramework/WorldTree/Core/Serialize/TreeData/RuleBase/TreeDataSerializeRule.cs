﻿/****************************************

* 作者：闪电黑客
* 日期：2024/8/21 11:22

* 描述：

*/
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
	/// 树数据序列化法则
	/// </summary>
	public interface TreeDataSerialize : ISendRefRule<object, SerializedTypeMode>, ITreeDataSerialize, ISourceGeneratorIgnore { }

	/// <summary>
	/// 树数据反序列化法则
	/// </summary>
	public interface TreeDataDeserialize : ISendRefRule<object, int>, ITreeDataDeserialize, ISourceGeneratorIgnore { }

	/// <summary>
	/// 树数据序列化法则
	/// </summary>
	public abstract class TreeDataSerializeRule<GT> : Rule<GT, TreeDataSerialize>, ISendRefRule<object, SerializedTypeMode>
	{
		/// <summary>
		/// 调用
		/// </summary>
		public virtual void Invoke(INode self, ref object value, ref SerializedTypeMode typeMode) => Execute(self as TreeDataByteSequence, ref value, ref typeMode);
		/// <summary>
		/// 执行
		/// </summary>
		protected abstract void Execute(TreeDataByteSequence self, ref object value, ref SerializedTypeMode typeMode);
	}

	/// <summary>
	/// 树数据反序列化法则
	/// </summary>
	public abstract class TreeDataDeserializeRule<GT> : Rule<GT, TreeDataDeserialize>, ISendRefRule<object, int>
	{
		/// <summary>
		/// 调用
		/// </summary>
		public virtual void Invoke(INode self, ref object value, ref int fieldNameCode) => Execute(self as TreeDataByteSequence, ref value, ref fieldNameCode);
		/// <summary>
		/// 执行
		/// </summary>
		protected abstract void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode);
	}
}
