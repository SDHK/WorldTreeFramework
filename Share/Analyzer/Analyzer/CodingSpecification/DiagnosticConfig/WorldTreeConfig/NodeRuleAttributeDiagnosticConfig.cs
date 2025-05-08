/****************************************

* 作者：闪电黑客
* 日期：2024/6/26 14:41

* 描述：

*/
using Microsoft.CodeAnalysis.CSharp;

namespace WorldTree.Analyzer
{
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

			SetConfig(DiagnosticKey.AttributeNaming, new DiagnosticConfig()
			{
				Title = "NodeRule特性检查",
				MessageFormat = "NodeRule只能标记在方法上",
				DeclarationKind = SyntaxKind.Attribute,
				Check = (semanticModel, identifier) => false,
				NeedComment = false
			});
		}

	}


}