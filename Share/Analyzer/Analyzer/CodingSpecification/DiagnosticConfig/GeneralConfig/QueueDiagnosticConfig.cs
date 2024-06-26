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
	/// Queue类型诊断
	/// </summary>
	public class QueueDiagnosticConfig : DiagnosticConfigGroup
	{
		public QueueDiagnosticConfig()
		{
			Screen = (Symbol) =>
			{
				if (Symbol is not ITypeSymbol TypeSymbol) return false;
				if (TypeSymbol.TypeKind != TypeKind.Class) return false;
				if (TypeSymbol.DeclaredAccessibility != Accessibility.Public) return false;
				return NamedSymbolHelper.CheckBase(TypeSymbol, "Queue", out _);
				//return Regex.IsMatch(typeName, "^System.Collections.Generic.Queue<.*>$");
			};

			SetConfig(DiagnosticKey.ClassFieldNaming, new DiagnosticConfig()
			{
				Title = "Queue类型字段命名",
				MessageFormat = "Queue类型字段 命名要加Queue后戳",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				Check = s => Regex.IsMatch(s, ".*Queue$"),
				FixCode = s => s + "Queue",
			});
			SetConfig(DiagnosticKey.ClassPropertyNaming, new DiagnosticConfig()
			{
				Title = "Queue类型属性命名",
				MessageFormat = "Queue类型属性 命名要加Queue后戳",
				DeclarationKind = SyntaxKind.PropertyDeclaration,
				Check = s => Regex.IsMatch(s, ".*Queue$"),
				FixCode = s => s + "Queue",
			});
		}
	}
}