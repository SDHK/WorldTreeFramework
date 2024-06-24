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
	public class StackDiagnostic : DiagnosticGroupConfig
	{
		public override DiagnosticGroupConfig Init()
		{
			Screen = (Symbol) =>
			{
				if (Symbol is not ITypeSymbol TypeSymbol) return false;
				if (TypeSymbol.TypeKind != TypeKind.Class) return false;
				if (TypeSymbol.DeclaredAccessibility != Accessibility.Public) return false;
				string typeName = TypeSymbol?.ToDisplayString() ?? string.Empty;
				return Regex.IsMatch(typeName, "^System.Collections.Generic.Stack<.*>$");
			};

			SetNamingRule(DiagnosticKey.ClassFieldNaming, new DiagnosticConfig()
			{
				Title = "Stack类型字段命名规范诊断",
				MessageFormat = "Stack类型字段 命名要加Stack后戳",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				Check = s => Regex.IsMatch(s, ".*Stack$"),
				FixCode = s => s + "Stack",
			});
			SetNamingRule(DiagnosticKey.ClassPropertyNaming, new DiagnosticConfig()
			{
				Title = "Stack类型属性命名规范诊断",
				MessageFormat = "Stack类型属性 命名要加Stack后戳",
				DeclarationKind = SyntaxKind.PropertyDeclaration,
				Check = s => Regex.IsMatch(s, ".*Stack$"),
				FixCode = s => s + "Stack",
			});
			return this;
		}
	}
}