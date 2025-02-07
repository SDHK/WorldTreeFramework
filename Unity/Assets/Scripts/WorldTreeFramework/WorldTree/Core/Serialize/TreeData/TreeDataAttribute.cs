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
	/// 树节点特别处理标记，参数为设定的字段数量
	/// </summary>
	public class TreeDataSpecialAttribute : Attribute
	{
		/// <summary>
		/// 字段数量
		/// </summary>
		public int FieldsCount;

		public TreeDataSpecialAttribute(int fieldsCount = 1)
		{
			FieldsCount = fieldsCount;
		}
	}
}
