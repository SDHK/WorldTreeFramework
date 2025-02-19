/****************************************

* 作者：闪电黑客
* 日期：2024/8/12 11:49

* 描述：

*/
using System;

namespace WorldTree
{

	/// <summary>
	/// 树节点数据包标记
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
	public class TreePackSerializableAttribute : Attribute { }

	/// <summary>
	/// 序列化成员忽略特性标记
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class TreePackIgnoreAttribute : Attribute { }

}
