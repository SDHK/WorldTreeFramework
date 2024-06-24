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
				Title = "Array类型字段命名规范诊断",
				MessageFormat = "Array类型字段 命名要加s后戳",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				Check = s => Regex.IsMatch(s, ".*s"),
				FixCode = s => s + "s",
			});
			SetConfig(DiagnosticKey.ClassPropertyNaming, new DiagnosticConfig()
			{
				Title = "Array类型属性命名规范诊断",
				MessageFormat = "Array类型属性 命名要加s后戳",
				DeclarationKind = SyntaxKind.PropertyDeclaration,
				Check = s => Regex.IsMatch(s, ".*s"),
				FixCode = s => s + "s",
			});
		}
		
	}
}