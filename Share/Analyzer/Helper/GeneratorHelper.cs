/****************************************

* 作者：闪电黑客
* 日期：2024/7/29 11:37

* 描述：

*/
namespace WorldTree
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
		/// 通知法则接口：引用参数
		/// </summary>
		public static string ISendRefRule = "ISendRefRule";

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

		/// <summary>
		/// 生成忽略接口
		/// </summary>
		public static string ISourceGeneratorIgnore = "ISourceGeneratorIgnore";

		/// <summary>
		/// 树节点数据包特性标记
		/// </summary>
		public static string TreePackAttribute = "TreePackAttribute";

		/// <summary>
		/// 树节点数据包子类转换特性标记
		/// </summary>
		public static string TreePackSubAttribute = "TreePackSubAttribute";

		/// <summary>
		/// 树节点数据忽略特性标记
		/// </summary>
		public static string TreePackIgnoreAttribute = "TreePackIgnoreAttribute";

	}

}

