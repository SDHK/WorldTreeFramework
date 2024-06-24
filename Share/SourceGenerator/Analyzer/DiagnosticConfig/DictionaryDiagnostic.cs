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
	public class DictionaryDiagnostic : DiagnosticGroupConfig
	{
		public override DiagnosticGroupConfig Init()
		{
			Screen = (Symbol) =>
			{
				if (Symbol is not ITypeSymbol TypeSymbol) return false;
				if (TypeSymbol.TypeKind != TypeKind.Class) return false;
				if (TypeSymbol.DeclaredAccessibility != Accessibility.Public) return false;
				string typeName = TypeSymbol?.ToDisplayString() ?? string.Empty;
				return Regex.IsMatch(typeName, "^System.Collections.Generic.Dictionary<.*>$");
			};

			SetNamingRule(DiagnosticKey.ClassFieldNaming, new DiagnosticConfig()
			{
				Title = "Dictionary类型字段命名规范诊断",
				MessageFormat = "Dictionary类型字段 命名要加Dict后戳",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				Check = s => Regex.IsMatch(s, ".*Dict"),
				FixCode = s => s + "Dict",
			});
			SetNamingRule(DiagnosticKey.ClassPropertyNaming, new DiagnosticConfig()
			{
				Title = "Dictionary类型属性命名规范诊断",
				MessageFormat = "Dictionary类型属性 命名要加Dict后戳",
				DeclarationKind = SyntaxKind.PropertyDeclaration,
				Check = s => Regex.IsMatch(s, ".*Dict"),
				FixCode = s => s + "Dict",
			});
			return this;
		}
	}
}