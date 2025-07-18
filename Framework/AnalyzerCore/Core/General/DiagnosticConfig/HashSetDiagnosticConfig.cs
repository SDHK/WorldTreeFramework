﻿/****************************************

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
			Screen = (Compilation, Symbol) =>
			{
				if (Symbol is not ITypeSymbol TypeSymbol) return false;
				if (TypeSymbol.TypeKind != TypeKind.Class) return false;
				return NamedSymbolHelper.CheckBase(TypeSymbol, "HashSet", out _);
			};

			SetConfig(DiagnosticKey.ClassFieldNaming, new DiagnosticConfig()
			{
				Title = "HashSet类型字段命名",
				MessageFormat = "HashSet类型字段 命名要加Hash后戳",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				Check = (semanticModel, identifier) => Regex.IsMatch(identifier.Text, ".*Hash$"),
				FixCode = s => s + "Hash",
				NeedComment = (semanticModel, identifier) => false,
			});
			SetConfig(DiagnosticKey.ClassPropertyNaming, new DiagnosticConfig()
			{
				Title = "HashSet类型属性命名",
				MessageFormat = "HashSet类型属性 命名要加Hash后戳",
				DeclarationKind = SyntaxKind.PropertyDeclaration,
				Check = (semanticModel, identifier) => Regex.IsMatch(identifier.Text, ".*Hash$"),
				FixCode = s => s + "Hash",
				NeedComment = (semanticModel, identifier) => false,
			});
			SetConfig(DiagnosticKey.ClassLocalVariableNaming, new DiagnosticConfig()
			{
				Title = "HashSet类型局部变量命名",
				MessageFormat = "HashSet类型局部变量 命名要加Hash后戳",
				DeclarationKind = SyntaxKind.LocalDeclarationStatement,
				Check = (semanticModel, identifier) => Regex.IsMatch(identifier.Text, ".*Hash$") || identifier.Text == "obj",
				FixCode = s => s + "Hash",
				NeedComment = (semanticModel, identifier) => false,
			});
		}
	}
}