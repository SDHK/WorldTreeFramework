/****************************************

* 作者：闪电黑客
* 日期：2024/7/9 15:29

* 描述：

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// 序列化忽略特性标记
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class TreePackIgnoreAttribute : Attribute{}

	/// <summary>
	/// 树节点数据包装特性标记
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class TreePackAttribute : Attribute
	{
	
	}
}
