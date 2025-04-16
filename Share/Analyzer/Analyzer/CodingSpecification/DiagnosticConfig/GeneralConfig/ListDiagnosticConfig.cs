/****************************************

* 作者：闪电黑客
* 日期：2024/6/20 20:46

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Text.RegularExpressions;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// List类型诊断
	/// </summary>
	public class ListDiagnosticConfig : DiagnosticConfigGroup
	{
		public ListDiagnosticConfig()
		{
			Screen = (Compilation, Symbol) =>
			{
				if (Symbol is not INamedTypeSymbol TypeSymbol) return false;
				if (TypeSymbol.TypeKind != TypeKind.Class) return false;
				return NamedSymbolHelper.IsDerivedFrom(TypeSymbol, NamedSymbolHelper.ToINamedTypeSymbol(Compilation, GeneratorHelper.IList), out _, TypeCompareOptions.None);
			};

			SetConfig(DiagnosticKey.ClassFieldNaming, new DiagnosticConfig()
			{
				Title = "List类型字段命名",
				MessageFormat = "List类型字段 命名要加List后戳",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				Check = (semanticModel, identifier) => Regex.IsMatch(identifier.Text, ".*List$"),
				FixCode = s => s + "List",
				NeedComment = false,
			});
			SetConfig(DiagnosticKey.ClassPropertyNaming, new DiagnosticConfig()
			{
				Title = "List类型属性命名",
				MessageFormat = "List类型属性 命名要加List后戳",
				DeclarationKind = SyntaxKind.PropertyDeclaration,
				Check = (semanticModel, identifier) => Regex.IsMatch(identifier.Text, ".*List$"),
				FixCode = s => s + "List",
				NeedComment = false,
			});
			SetConfig(DiagnosticKey.ClassLocalVariableNaming, new DiagnosticConfig()
			{
				Title = "List类型局部变量命名",
				MessageFormat = "List类型局部变量 命名要加List后戳",
				DeclarationKind = SyntaxKind.LocalDeclarationStatement,
				Check = (semanticModel, identifier) => Regex.IsMatch(identifier.Text, ".*List$") || identifier.Text == "obj",
				FixCode = s => s + "List",
				NeedComment = false,
			});
		}
	}
}