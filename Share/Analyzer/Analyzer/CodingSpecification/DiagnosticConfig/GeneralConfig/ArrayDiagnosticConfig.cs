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
	/// Array类型诊断
	/// </summary>
	public class ArrayDiagnosticConfig : DiagnosticConfigGroup
	{
		public ArrayDiagnosticConfig()
		{
			Screen = (Symbol) =>
			{
				if (Symbol is not ITypeSymbol TypeSymbol) return false;
				return TypeSymbol.TypeKind == TypeKind.Array;
			};

			SetConfig(DiagnosticKey.ClassFieldNaming, new DiagnosticConfig()
			{
				Title = "Array类型字段命名",
				MessageFormat = "Array类型字段 命名要加s后戳",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				Check = (semanticModel, identifier) => Regex.IsMatch(identifier.Text, ".*s$"),
				FixCode = s => s + "s",
				NeedComment = false,
			});
			SetConfig(DiagnosticKey.ClassPropertyNaming, new DiagnosticConfig()
			{
				Title = "Array类型属性命名",
				MessageFormat = "Array类型属性 命名要加s后戳",
				DeclarationKind = SyntaxKind.PropertyDeclaration,
				Check = (semanticModel, identifier) => Regex.IsMatch(identifier.Text, ".*s$"),
				FixCode = s => s + "s",
				NeedComment = false,
			});
			SetConfig(DiagnosticKey.ClassLocalVariableNaming, new DiagnosticConfig()
			{
				Title = "Array类型局部变量命名",
				MessageFormat = "Array类型局部变量 命名要加s后戳",
				DeclarationKind = SyntaxKind.LocalDeclarationStatement,
				Check = (semanticModel, identifier) => Regex.IsMatch(identifier.Text, ".*s$") || identifier.Text == "obj",
				FixCode = s => s + "s",
				NeedComment = false,
			});
		}

	}
}