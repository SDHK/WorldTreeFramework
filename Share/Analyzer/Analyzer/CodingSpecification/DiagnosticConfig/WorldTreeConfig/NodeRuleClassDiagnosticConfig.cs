/****************************************

* 作者：闪电黑客
* 日期：2024/6/26 14:41

* 描述：

*/
using Microsoft.CodeAnalysis.CSharp;

namespace WorldTree.Analyzer
{


	public class NodeRuleClassDiagnosticConfig : DiagnosticConfigGroup
	{
		public NodeRuleClassDiagnosticConfig()
		{
			Screen = (Compilation, Symbol) =>
			{
				return true;
			};

			SetConfig(DiagnosticKey.NodeRuleClassAnalysis, new DiagnosticConfig()
			{
				Title = "NodeRule孤立类型分析",
				MessageFormat = "NodeRule法则方法补全",
				CodeFixTitle = "【补全法则方法】",
				DeclarationKind = SyntaxKind.GenericName,
				Check = (semanticModel, identifier) => false,
				NeedComment = (semanticModel, identifier) => false,
			});
		}
	}



}