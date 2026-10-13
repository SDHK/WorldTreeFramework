/****************************************

* 作者：闪电黑客
* 日期：2024/10/15 14:34

* 描述：

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// 树节点数据标记，默认为非恒定类型，恒定类型将不会记录类名到数据中，减少数据体积
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false)]
	public class TreeDataSerializableAttribute : Attribute
	{
		/// <summary>
		/// 是否为恒定类型
		/// </summary>
		public bool IsConstant = false;
		public TreeDataSerializableAttribute(bool isConstant = false)
		{
			this.IsConstant = isConstant;
		}
	}

	/// <summary>
	/// 序列化成员忽略标记 
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class TreeDataIgnoreAttribute : Attribute { }

	/// <summary>
	/// 树节点特别处理标记，参数为设定的字段数量。标记「集合 / 特殊容器」
	/// </summary>
	/// <remarks>
	/// 声明集合泛型在源生成器中对应的「字段数」
	/// 避免继承该集合的子类把基类实现细节当成多个持久化字段。
	/// 
	/// 通过子类扫描到父级有该特性后，源生成器会把特性标记数量当成父级字段数量，
	/// 而不是根据成员类型的字段数量来处理。
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class)]
	public class TreeDataSpecialAttribute : Attribute
	{
		/// <summary>
		/// 字段数量
		/// </summary>
		public int FieldsCount;

		public TreeDataSpecialAttribute(int fieldsCount)
		{
			FieldsCount = fieldsCount;
		}
	}
}