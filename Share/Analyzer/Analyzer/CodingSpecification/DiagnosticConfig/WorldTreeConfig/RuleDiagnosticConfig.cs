/****************************************

* 作者：闪电黑客
* 日期：2024/6/26 14:41

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// IRule类型方法诊断
	/// </summary>
	public class RuleDiagnosticConfig : DiagnosticConfigGroup
	{
		public RuleDiagnosticConfig()
		{
			Screen = (Symbol) =>
			{
				if (Symbol is not ITypeSymbol TypeSymbol) return false;
				if (TypeSymbol.TypeKind != TypeKind.Class) return false;
				if (TypeSymbol.IsAbstract) return false;
				return NamedSymbolHelper.CheckInterface(TypeSymbol, "IRule", out _);
			};

			SetConfig(DiagnosticKey.ClassNaming, new DiagnosticConfig()
			{
				Title = "Rule类型命名",
				MessageFormat = "类型命名开头要大写",
				DeclarationKind = SyntaxKind.ClassDeclaration,
				NeedComment = false
			});

			//SetConfig(DiagnosticKey.MethodNaming, new DiagnosticConfig()
			//{
			//	Title = "Rule类型方法禁止",
			//	MessageFormat = "Rule中不可以写方法",
			//	DeclarationKind = SyntaxKind.MethodDeclaration,
			//	UnKeywordKinds = new() { SyntaxKind.ProtectedKeyword, SyntaxKind.OverrideKeyword },
			//	Check = s => false,
			//});
			

		}

	}


}