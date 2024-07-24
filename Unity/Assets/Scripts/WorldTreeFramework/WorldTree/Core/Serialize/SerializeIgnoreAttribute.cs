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
	public class SerializeIgnoreAttribute : Attribute{}
}
