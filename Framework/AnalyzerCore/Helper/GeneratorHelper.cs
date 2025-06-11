/****************************************

* 作者：闪电黑客
* 日期：2024/7/29 11:37

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 法则基类枚举码
	/// </summary>
	public enum RuleBaseEnum
	{
		SendRule,
		SendRuleAsync,
		CallRule,
		CallRuleAsync,
	}

	/// <summary>
	/// 生成器帮助类
	/// </summary>
	public static class GeneratorHelper
	{
		public const string INode = "WorldTree.INode";

		public const string IUnit = "WorldTree.IUnit";

		public const string IBranch = "WorldTree.IBranch";
		public const string IBranch_1 = "WorldTree.IBranch`1";


		public const string IBranchIdKey = "WorldTree.IBranchIdKey";

		public const string IBranchTypeKey = "WorldTree.IBranchTypeKey";

		public const string IBranchUnConstraint = "WorldTree.IBranchUnConstraint";

		public const string IMethodRule = "WorldTree.IMethodRule";


		public const string IRule = "WorldTree.IRule";

		public const string IDictonary = "System.Collections.IDictionary";

		public const string IList = "System.Collections.IList";

		/// <summary>
		/// 数据序列化接口
		/// </summary>
		public const string ISerializable = "WorldTree.ISerializable";

		#region 代码生成的接口，无法检测命名空间

		/// <summary>
		/// 通知法则接口
		/// </summary>
		public const string ISendRule = "ISendRule";
		/// <summary>
		/// 通知法则接口：引用参数
		/// </summary>
		public const string ISendRefRule = "ISendRefRule";
		/// <summary>
		/// 调用法则接口
		/// </summary>
		public const string ICallRule = "ICallRule";
		/// <summary>
		/// 通知法则异步接口
		/// </summary>
		public const string ISendRuleAsync = "ISendRuleAsync";
		/// <summary>
		/// 调用法则异步接口
		/// </summary>
		public const string ICallRuleAsync = "ICallRuleAsync";


		/// <summary>
		/// 通知法则基类
		/// </summary>
		public const string SendRule = "SendRule";
		/// <summary>
		/// 通知法则基类：引用参数
		/// </summary>
		public const string SendRefRule = "SendRefRule";
		/// <summary>
		/// 调用法则基类
		/// </summary>
		public const string CallRule = "CallRule";
		/// <summary>
		/// 通知法则异步基类
		/// </summary>
		public const string SendRuleAsync = "SendRuleAsync";
		/// <summary>
		/// 调用法则异步基类
		/// </summary>
		public const string CallRuleAsync = "CallRuleAsync";

		#endregion



		/// <summary>
		/// 生成忽略接口
		/// </summary>
		public const string ISourceGeneratorIgnore = "WorldTree.ISourceGeneratorIgnore";

		/// <summary>
		/// 树节点数据包特性标记
		/// </summary>
		public const string TreePackSerializableAttribute = "TreePackSerializableAttribute";

		/// <summary>
		/// 树节点数据忽略特性标记
		/// </summary>
		public const string TreePackIgnoreAttribute = "TreePackIgnoreAttribute";

		/// <summary>
		/// 树节点数据特性标记
		/// </summary>
		public const string TreeDataSerializableAttribute = "TreeDataSerializableAttribute";

		/// <summary>
		/// 树节点数据忽略特性标记
		/// </summary>
		public const string TreeDataIgnoreAttribute = "TreeDataIgnoreAttribute";

		/// <summary>
		/// 树节点数据特别处理特性标记
		/// </summary>
		public const string TreeDataSpecialAttribute = "TreeDataSpecialAttribute";


		/// <summary>
		/// 深拷贝特性标记
		/// </summary>
		public const string TreeCopyableAttribute = "TreeCopyableAttribute";

		/// <summary>
		/// 深拷贝特别处理特性标记
		/// </summary>
		public const string TreeCopySpecialAttribute = "TreeCopySpecialAttribute";

		/// <summary>
		/// 深拷贝忽略特性标记
		/// </summary>
		public const string TreeCopyIgnoreAttribute = "TreeCopyIgnoreAttribute";



		/// <summary>
		/// 法则分流类型特性标记
		/// </summary>
		public const string RuleSwitchAttribute = "RuleSwitchAttribute";

		/// <summary>
		/// 法则简写方法特性标记
		/// </summary>
		public const string NodeRuleAttribute = "NodeRuleAttribute";

		/// <summary>
		/// 法则类型预注册数组生成特性标记，暂时无用
		/// </summary>
		public const string RuleTypesGeneratorAttribute = "RuleTypesGeneratorAttribute";


	}

}

