/****************************************

* 作者：闪电黑客
* 日期：2024/6/24 19:44

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Text.RegularExpressions;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// Branch类型诊断
	/// </summary>
	public class BranchDiagnosticConfig : DiagnosticConfigGroup
	{
		public BranchDiagnosticConfig()
		{
			Screen = (Compilation, Symbol) =>
			{
				if (Symbol is not INamedTypeSymbol TypeSymbol) return false;
				if (TypeSymbol.TypeKind != TypeKind.Class) return false;

				return NamedSymbolHelper.IsDerivedFrom(TypeSymbol, NamedSymbolHelper.ToINamedTypeSymbol(Compilation, GeneratorHelper.IBranch), out _, TypeCompareOptions.None);
			};

			SetConfig(DiagnosticKey.ClassFieldNaming, new DiagnosticConfig()
			{
				Title = "Branch类型字段命名",
				MessageFormat = "Branch类型字段 命名要加Branch后戳",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				Check = (semanticModel, identifier) => Regex.IsMatch(identifier.Text, ".*Branch$"),
				FixCode = s => s + "Branch",
				NeedComment = (semanticModel, identifier) => false,
			});
			SetConfig(DiagnosticKey.ClassPropertyNaming, new DiagnosticConfig()
			{
				Title = "Branch类型属性命名",
				MessageFormat = "Branch类型属性 命名要加Branch后戳",
				DeclarationKind = SyntaxKind.PropertyDeclaration,
				Check = (semanticModel, identifier) => Regex.IsMatch(identifier.Text, ".*Branch$"),
				FixCode = s => s + "Branch",
				NeedComment = (semanticModel, identifier) => false,
			});
			SetConfig(DiagnosticKey.ClassLocalVariableNaming, new DiagnosticConfig()
			{
				Title = "Branch类型局部变量命名",
				MessageFormat = "Branch类型局部变量 命名要加Branch后戳",
				DeclarationKind = SyntaxKind.LocalDeclarationStatement,
				Check = (semanticModel, identifier) => Regex.IsMatch(identifier.Text, ".*Branch$") || identifier.Text == "branch",
				FixCode = s => s + "Branch",
				NeedComment = (semanticModel, identifier) => false,
			});
		}

	}
}