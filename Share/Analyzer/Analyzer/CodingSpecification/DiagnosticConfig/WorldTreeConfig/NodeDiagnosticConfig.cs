/****************************************

* 作者：闪电黑客
* 日期：2024/6/26 14:41

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// Node类型诊断
	/// </summary>
	public class NodeDiagnosticConfig : DiagnosticConfigGroup
	{
		public NodeDiagnosticConfig()
		{
			Screen = (Symbol) =>
			{
				if (Symbol is not ITypeSymbol TypeSymbol) return false;
				if (TypeSymbol.TypeKind == TypeKind.Class)
				{
					return TypeSymbol.Name == "INode" ? true : NamedSymbolHelper.CheckInterface(TypeSymbol, GeneratorHelper.INode, out _);
				}
				else
				{
					return false;
				}
			};

			SetConfig(DiagnosticKey.MethodNaming, new DiagnosticConfig()
			{
				Title = "Node类型方法禁止",
				MessageFormat = "Node类禁止声明方法",
				DeclarationKind = SyntaxKind.MethodDeclaration,
				UnKeywordKinds = new List<SyntaxKind> { SyntaxKind.AbstractKeyword },
				Check = s => false,
				NeedComment = false,
			});
		}
	}


}