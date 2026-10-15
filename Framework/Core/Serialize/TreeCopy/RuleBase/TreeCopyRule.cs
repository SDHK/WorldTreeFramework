using System;
namespace WorldTree
{

	/// <summary>
	/// 深拷贝标记
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false)]
	public class TreeCopyableAttribute : Attribute { }

	/// <summary>
	/// 深拷贝特别处理标记
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false)]
	public class TreeCopySpecialAttribute : Attribute { }

	/// <summary>
	/// 深拷贝成员忽略特性标记
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class TreeCopyIgnoreAttribute : Attribute { }

	/// <summary>
	/// 树深拷贝法则接口：用于序列化未知泛型，解除AsRule的法则限制
	/// </summary>
	public interface ITreeCopy : IRule { }

	/// <summary>
	/// 树深拷贝法则
	/// </summary>
	public interface TreeCopy : ISendRefRule<object, object>, ITreeCopy, ISourceGeneratorIgnore { }
	/// <summary>
	/// 树深拷贝法则
	/// </summary>
	public abstract class TreeCopyRule<GT> : Rule<GT, TreeCopy>, ISendRefRule<object, object>
	{
		/// <summary>
		/// 调用
		/// </summary>
		public virtual void Invoke(INode self, ref object source, ref object target) => Execute(self as TreeCopier, ref source, ref target);
		/// <summary>
		/// 执行
		/// </summary>
		protected abstract void Execute(TreeCopier self, ref object source, ref object target);
	}

	#region 非常规法则

	/// <summary>
	/// 结构体拷贝接口：用于反序列化未知泛型，解除AsRule的法则限制
	/// </summary>
	public interface TreeCopyStruct : ISendRefRule, ISourceGeneratorIgnore { }


	/// <summary>
	/// 结构体拷贝法则
	/// </summary>
	/// <remarks>打破常规写法，以参数类型为主</remarks>
	public abstract class TreeCopyStructRule<GT> : Rule<GT, TreeCopyStruct>, ISendRefRule<GT, GT>
	{
		/// <summary>
		/// 调用
		/// </summary>
		public virtual void Invoke(INode self, ref GT arg1, ref GT arg2) => Execute(self as TreeCopier, ref arg1, ref arg2);
		/// <summary>
		/// 执行
		/// </summary>
		protected abstract void Execute(TreeCopier self, ref GT arg1, ref GT arg2);
	}

	#endregion
}