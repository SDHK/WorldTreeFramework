/****************************************

* 作者：闪电黑客
* 日期：2024/6/26 14:41

* 描述：

*/
using Microsoft.CodeAnalysis.CSharp;

namespace WorldTree.Analyzer
{
	public class NodeRuleMethodDiagnosticConfig : DiagnosticConfigGroup
	{
		public NodeRuleMethodDiagnosticConfig()
		{
			Screen = (Compilation, Symbol) =>
			{
				return true;
			};

			SetConfig(DiagnosticKey.NodeRuleMethodAnalysis, new DiagnosticConfig()
			{
				Title = "NodeRule法则方法参数分析",
				MessageFormat = "NodeRule法则方法参数错误修复",
				CodeFixTitle = "【修复法则方法】",
				DeclarationKind = SyntaxKind.MethodDeclaration,
				Check = (semanticModel, identifier) => false,
				NeedComment = (semanticModel, identifier) => false,
			});
		}

	}



}