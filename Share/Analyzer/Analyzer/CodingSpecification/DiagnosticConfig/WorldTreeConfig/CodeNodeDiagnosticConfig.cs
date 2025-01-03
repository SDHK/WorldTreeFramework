/****************************************

* 作者：闪电黑客
* 日期：2024/6/24 19:44

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Text.RegularExpressions;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// 核心Node类型诊断
	/// </summary>
	public class CodeNodeDiagnosticConfig : DiagnosticConfigGroup
	{
		public CodeNodeDiagnosticConfig()
		{
			Screen = (Symbol) =>
			{
				if (Symbol is not ITypeSymbol TypeSymbol) return false;
				if (TypeSymbol.TypeKind == TypeKind.Class || TypeSymbol.TypeKind == TypeKind.Interface)
				{
					return TypeSymbol.Name == "INode" ? true : NamedSymbolHelper.CheckInterface(TypeSymbol, GeneratorHelper.INode, out _);
				}
				else
				{
					return false;
				}
			};

			SetConfig(DiagnosticKey.PublicFieldNaming, new DiagnosticConfig()
			{
				Title = "Node公开字段命名",
				MessageFormat = "公开字段命名",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				Check = (semanticModel, identifier) => true,
				KeywordKinds = new() { SyntaxKind.PublicKeyword, },
				UnKeywordKinds = new() { SyntaxKind.ConstKeyword, },
			});
			SetConfig(DiagnosticKey.PublicPropertyNaming, new DiagnosticConfig()
			{
				Title = "Node公开属性命名",
				MessageFormat = "公开属性命名",
				Check = (semanticModel, identifier) => true,
				DeclarationKind = SyntaxKind.PropertyDeclaration,
				KeywordKinds = new() { SyntaxKind.PublicKeyword, },
			});
			SetConfig(DiagnosticKey.SimpleMemberAccess, new DiagnosticConfig()
			{
				Title = "Node成员访问限制",
				MessageFormat = "不可访问:小写为私有,[Protected]为受保护",
				NeedComment = false,
				Check = (semanticModel, identifier) => Regex.IsMatch(identifier.Text, "^[a-z].*$"),
				DeclarationKind = SyntaxKind.SimpleMemberAccessExpression,
				UnKeywordKinds = new() { SyntaxKind.PropertyKeyword, },
			});
		}
	}
}