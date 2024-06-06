/****************************************

* 作者：闪电黑客
* 日期：2024/4/7 16:01

* 描述：生成器帮助类

*/

using System.Text;

namespace WorldTree.SourceGenerator
{
	/// <summary>
	/// 生成器帮助类
	/// </summary>
	internal static class GeneratorHelper
	{
		/// <summary>
		/// 通知法则接口
		/// </summary>
		public static string ISendRule = "ISendRule";
		/// <summary>
		/// 调用法则接口
		/// </summary>
		public static string ICallRule = "ICallRule";
		/// <summary>
		/// 通知法则异步接口
		/// </summary>
		public static string ISendRuleAsync = "ISendRuleAsync";
		/// <summary>
		/// 调用法则异步接口
		/// </summary>
		public static string ICallRuleAsync = "ICallRuleAsync";

	}

}

