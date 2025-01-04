/****************************************

* 作者：闪电黑客
* 日期：2024/8/12 11:49

* 描述：

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// 序列化忽略特性标记
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class TreePackIgnoreAttribute : Attribute { }

	/// <summary>
	/// 树节点数据包装特性标记
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
	public class TreePackSerializableAttribute : Attribute { }

	/// <summary>
	/// 树节点数据包子类转换标记
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
	public class TreePackSubAttribute : Attribute
	{
		/// <summary>
		/// 子类类型
		/// </summary>
		public Type SubType;

		public TreePackSubAttribute(Type type)
		{
			SubType = type;
		}
	}
}
