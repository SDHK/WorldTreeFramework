/****************************************

* 作者：闪电黑客
* 日期：2024/6/24 18:07

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Text.RegularExpressions;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// Dictionary类型诊断
	/// </summary>
	public class DictionaryDiagnosticConfig : DiagnosticConfigGroup
	{
		public DictionaryDiagnosticConfig()
		{
			Screen = (Compilation, Symbol) =>
			{
				if (Symbol is not INamedTypeSymbol TypeSymbol) return false;
				if (TypeSymbol.TypeKind != TypeKind.Class) return false;

				return NamedSymbolHelper.IsDerivedFrom(TypeSymbol, NamedSymbolHelper.ToINamedTypeSymbol(Compilation, GeneratorHelper.IDictonary), out _, TypeCompareOptions.None);
			};

			SetConfig(DiagnosticKey.ClassFieldNaming, new DiagnosticConfig()
			{
				Title = "Dictionary类型字段命名",
				MessageFormat = "Dictionary类型字段 命名要加Dict后戳",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				Check = (semanticModel, identifier) => Regex.IsMatch(identifier.Text, ".*Dict$"),
				FixCode = s => s + "Dict",
				NeedComment = (semanticModel, identifier) => false,
			});
			SetConfig(DiagnosticKey.ClassPropertyNaming, new DiagnosticConfig()
			{
				Title = "Dictionary类型属性命名",
				MessageFormat = "Dictionary类型属性 命名要加Dict后戳",
				DeclarationKind = SyntaxKind.PropertyDeclaration,
				Check = (semanticModel, identifier) => Regex.IsMatch(identifier.Text, ".*Dict$"),
				FixCode = s => s + "Dict",
				NeedComment = (semanticModel, identifier) => false,
			});
			SetConfig(DiagnosticKey.ClassLocalVariableNaming, new DiagnosticConfig()
			{
				Title = "Dictionary类型局部变量命名",
				MessageFormat = "Dictionary类型局部变量 命名要加Dict后戳",
				DeclarationKind = SyntaxKind.LocalDeclarationStatement,
				Check = (semanticModel, identifier) => Regex.IsMatch(identifier.Text, ".*Dict$") || identifier.Text == "obj",
				FixCode = s => s + "Dict",
				NeedComment = (semanticModel, identifier) => false,
			});
		}
	}
}