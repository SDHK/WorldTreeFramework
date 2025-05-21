/****************************************

* 作者：闪电黑客
* 日期：2025/5/20 20:14

* 描述：

*/


using System;
using System.Runtime.CompilerServices;

namespace WorldTree
{
	/// <summary>
	/// 深拷贝标记
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false)]
	public class TreeCopyableAttribute : Attribute { }

	/// <summary>
	/// 深拷贝成员忽略特性标记
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class TreeCopyIgnoreAttribute : Attribute { }



	/// <summary>
	/// 树深拷贝法则接口：用于序列化未知泛型，解除AsRule的法则限制
	/// </summary>
	public interface ITreeCopy : ISendRefRule { }

	///// <summary>
	///// 树深拷贝非托管法则
	///// </summary>
	//public interface TreeCopyUnmanaged<T> : ISendRefRule<T, T>, ITreeCopy, ISourceGeneratorIgnore { }
	///// <summary>
	///// 树深拷贝非托管法则
	///// </summary>
	//public abstract class TreeCopyUnmanagedRule<T> : SendRefRule<TreeCopyExecutor, TreeCopyUnmanaged<T>, T, T> { }


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
		public virtual void Invoke(INode self, ref object source, ref object target) => Execute(self as TreeCopyExecutor, ref source, ref target);
		/// <summary>
		/// 执行
		/// </summary>
		protected abstract void Execute(TreeCopyExecutor self, ref object source, ref object target);
	}

	/// <summary>
	/// 树深拷贝执行器
	/// </summary>
	public class TreeCopyExecutor : Node, ICloneable
		, AsRule<ITreeCopy>
	{
		/// <summary>
		/// 对象对应Id
		/// </summary>
		public UnitDictionary<object, int> ObjectToIdDict;

		/// <summary>
		/// Id对应对象
		/// </summary>
		public UnitDictionary<int, object> IdToObjectDict;

		public object Clone()
		{
			return null;
		}

		/// <summary>
		/// 拷贝对象
		/// </summary>
		public void CopyTo<T>(in T source, ref T target)
		{
			if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
			{
				target = source;
				return;
			}


		}
	}


}