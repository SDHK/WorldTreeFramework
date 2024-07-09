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
	/// HashSet类型诊断
	/// </summary>
	public class HashSetDiagnosticConfig : DiagnosticConfigGroup
	{
		public HashSetDiagnosticConfig()
		{
			Screen = (Symbol) =>
			{
				if (Symbol is not ITypeSymbol TypeSymbol) return false;
				if (TypeSymbol.TypeKind != TypeKind.Class) return false;
				return NamedSymbolHelper.CheckBase(TypeSymbol, "HashSet", out _);
				//return Regex.IsMatch(typeName, "^System.Collections.Generic.HashSet<.*>$");
			};

			SetConfig(DiagnosticKey.ClassFieldNaming, new DiagnosticConfig()
			{
				Title = "HashSet类型字段命名",
				MessageFormat = "HashSet类型字段 命名要加Hash后戳",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				Check = s => Regex.IsMatch(s, ".*Hash$"),
				FixCode = s => s + "Hash",
				NeedComment = false,
			});
			SetConfig(DiagnosticKey.ClassPropertyNaming, new DiagnosticConfig()
			{
				Title = "HashSet类型属性命名",
				MessageFormat = "HashSet类型属性 命名要加Set后戳",
				DeclarationKind = SyntaxKind.PropertyDeclaration,
				Check = s => Regex.IsMatch(s, ".*Hash$"),
				FixCode = s => s + "Hash",
				NeedComment = false,
			});
			SetConfig(DiagnosticKey.ClassLocalVariableNaming, new DiagnosticConfig()
			{
				Title = "HashSet类型局部变量命名",
				MessageFormat = "HashSet类型局部变量 命名要加Set后戳",
				DeclarationKind = SyntaxKind.LocalDeclarationStatement,
				Check = s => Regex.IsMatch(s, ".*Hash$"),
				FixCode = s => s + "Hash",
				NeedComment = false,
			});
		}
	}
}