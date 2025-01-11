/****************************************

* 作者：闪电黑客
* 日期：2024/12/3 16:17

* 描述：

*/
using System.Collections.Generic;

namespace WorldTree
{

	/// <summary>
	/// 输入检测器组
	/// </summary>
	public class InputDetectorGroup
	{



	}

	/// <summary>
	/// 输入检测器
	/// </summary>
	public class InputDetector : Node
	{
		/// <summary>
		/// 输入检测器配置
		/// </summary>
		public InputDetectorConfig config;

		/// <summary>
		/// 输入信息队列
		/// </summary>
		public List<InputInfo> InfoList;
	}

}