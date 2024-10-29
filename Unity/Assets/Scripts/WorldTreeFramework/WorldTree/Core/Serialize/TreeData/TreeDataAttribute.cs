/****************************************

* 作者：闪电黑客
* 日期：2024/8/20 17:35

* 描述：

*/
using System;

namespace WorldTree
{

	/// <summary>
	/// 树节点数据装特性标记
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false)]
	public class TreeDataSerializableAttribute : Attribute
	{
		/// <summary>
		/// 是否是子类
		/// </summary>
		public bool IsSub = false;

		public TreeDataSerializableAttribute(bool isSub = false)
		{
			IsSub = isSub;
		}

		public TreeDataSerializableAttribute() { }
	}

	/// <summary>
	/// 序列化忽略特性标记
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class TreeDataIgnoreAttribute : Attribute { }
}
