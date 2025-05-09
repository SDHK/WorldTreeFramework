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
	/// Stack类型诊断
	/// </summary>
	public class StackDiagnosticConfig : DiagnosticConfigGroup
	{
		public StackDiagnosticConfig()
		{
			Screen = (Compilation, Symbol) =>
			{
				if (Symbol is not ITypeSymbol TypeSymbol) return false;
				if (TypeSymbol.TypeKind != TypeKind.Class) return false;
				//if (TypeSymbol.DeclaredAccessibility != Accessibility.Public) return false;
				return NamedSymbolHelper.CheckBase(TypeSymbol, "Stack", out _);
				//string typeName = TypeSymbol?.ToDisplayString() ?? string.Empty;
				//return Regex.IsMatch(typeName, "^System.Collections.Generic.Stack<.*>$");
			};

			SetConfig(DiagnosticKey.ClassFieldNaming, new DiagnosticConfig()
			{
				Title = "Stack类型字段命名",
				MessageFormat = "Stack类型字段 命名要加Stack后戳",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				Check = (semanticModel, identifier) => Regex.IsMatch(identifier.Text, ".*Stack$"),
				FixCode = s => s + "Stack",
				NeedComment = (semanticModel, identifier) => false,
			});
			SetConfig(DiagnosticKey.ClassPropertyNaming, new DiagnosticConfig()
			{
				Title = "Stack类型属性命名",
				MessageFormat = "Stack类型属性 命名要加Stack后戳",
				DeclarationKind = SyntaxKind.PropertyDeclaration,
				Check = (semanticModel, identifier) => Regex.IsMatch(identifier.Text, ".*Stack$"),
				FixCode = s => s + "Stack",
				NeedComment = (semanticModel, identifier) => false,
			});
			SetConfig(DiagnosticKey.ClassLocalVariableNaming, new DiagnosticConfig()
			{
				Title = "Stack类型局部变量命名",
				MessageFormat = "Stack类型局部变量 命名要加Stack后戳",
				DeclarationKind = SyntaxKind.LocalDeclarationStatement,
				Check = (semanticModel, identifier) => Regex.IsMatch(identifier.Text, ".*Stack$") || identifier.Text == "obj",
				FixCode = s => s + "Stack",
				NeedComment = (semanticModel, identifier) => false,
			});
		}
	}
}