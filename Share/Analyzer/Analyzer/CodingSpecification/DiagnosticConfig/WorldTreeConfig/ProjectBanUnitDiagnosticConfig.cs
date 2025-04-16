/****************************************

* 作者：闪电黑客
* 日期：2024/7/8 11:59

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// 项目禁止单元类型诊断
	/// </summary>
	public class ProjectBanUnitDiagnosticConfig : DiagnosticConfigGroup
	{
		public ProjectBanUnitDiagnosticConfig()
		{
			Screen = (Compilation, Symbol) =>
			{
				if (Symbol is not INamedTypeSymbol TypeSymbol) return false;
				if (TypeSymbol.TypeKind != TypeKind.Class) return false;

				return NamedSymbolHelper.IsDerivedFrom(TypeSymbol, NamedSymbolHelper.ToINamedTypeSymbol(Compilation, GeneratorHelper.IUnit), out _);
			};

			SetConfig(DiagnosticKey.ClassNaming, new DiagnosticConfig()
			{
				Title = "项目禁止声明Unit类型",
				MessageFormat = "项目禁止声明Unit类型",
				DeclarationKind = SyntaxKind.ClassDeclaration,
				Check = (semanticModel, identifier) => false,
				NeedComment = false,
			});
		}
	}
}