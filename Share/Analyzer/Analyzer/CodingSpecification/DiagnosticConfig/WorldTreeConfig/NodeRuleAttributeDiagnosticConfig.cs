/****************************************

* 作者：闪电黑客
* 日期：2024/6/26 14:41

* 描述：

*/
using Microsoft.CodeAnalysis.CSharp;

namespace WorldTree.Analyzer
{
	public class RuleSwitchAttributeDiagnosticConfig : DiagnosticConfigGroup
	{
		public RuleSwitchAttributeDiagnosticConfig()
		{
			Screen = (Compilation, Symbol) =>
			{
				return true;
			};

			SetConfig(DiagnosticKey.RuleSwitchAttributeAnalysis, new DiagnosticConfig()
			{
				Title = "RuleSwitch特性标记分析",
				MessageFormat = "RuleSwitch键值修复",
				CodeFixTitle = "【键值修复】",
				DeclarationKind = SyntaxKind.Attribute,
				Check = (semanticModel, identifier) => false,
				NeedComment = (semanticModel, identifier) => false,
			});
		}
	}


	/// <summary>
	/// NodeRule特性诊断
	/// </summary>
	public class NodeRuleAttributeDiagnosticConfig : DiagnosticConfigGroup
	{
		public NodeRuleAttributeDiagnosticConfig()
		{
			Screen = (Compilation, Symbol) =>
			{
				return true;
			};

			SetConfig(DiagnosticKey.NodeRuleAttributeAnalysis, new DiagnosticConfig()
			{
				Title = "NodeRule特性标记分析",
				MessageFormat = "NodeRule法则方法补全",
				CodeFixTitle = "【补全法则方法】",
				DeclarationKind = SyntaxKind.Attribute,
				Check = (semanticModel, identifier) => false,
				NeedComment = (semanticModel, identifier) => false,
			});
		}
	}



}