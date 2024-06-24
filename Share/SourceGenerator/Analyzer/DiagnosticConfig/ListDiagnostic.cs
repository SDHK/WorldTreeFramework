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
	public class ListDiagnostic : ObjectDiagnostic
	{
		public override ObjectDiagnostic Init()
		{
			Screen = (Symbol) =>
			{
				if (Symbol is not ITypeSymbol TypeSymbol) return false;
				if (TypeSymbol.TypeKind != TypeKind.Class) return false;
				if (TypeSymbol.DeclaredAccessibility != Accessibility.Public) return false;
				string typeName = TypeSymbol?.ToDisplayString() ?? string.Empty;
				return Regex.IsMatch(typeName, "^System.Collections.Generic.List<.*>$");
			};

			SetNamingRule(DiagnosticKey.ClassFieldNaming, new CodeDiagnosticConfig()
			{
				Title = "List类型字段命名规范诊断",
				MessageFormat = "List类型字段 命名要加List后戳",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				Check = s => Regex.IsMatch(s, ".*List$"),
				FixCode = s => s + "List",
			});
			SetNamingRule(DiagnosticKey.ClassPropertyNaming, new CodeDiagnosticConfig()
			{
				Title = "List类型属性命名规范诊断",
				MessageFormat = "List类型属性 命名要加List后戳",
				DeclarationKind = SyntaxKind.PropertyDeclaration,
				Check = s => Regex.IsMatch(s, ".*List$"),
				FixCode = s => s + "List",
			});
			return this;
		}
	}
}