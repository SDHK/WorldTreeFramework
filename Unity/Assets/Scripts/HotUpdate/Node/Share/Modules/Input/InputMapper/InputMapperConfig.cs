/****************************************

* 作者：闪电黑客
* 日期：2024/12/3 16:17

* 描述：

*/
using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 输入检测器配置
	/// </summary>
	[TreeDataSerializable]
	public partial struct InputMapperConfig
	{
		/// <summary>
		/// 全局事件Type
		/// </summary>
		public long InputRuleType;

		/// <summary>
		/// 是否可改变
		/// </summary>
		public bool IsChange;

		/// <summary>
		/// 输入信息列表
		/// </summary>
		public List<InputInfo> InfoList;
	}
}