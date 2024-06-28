/****************************************

* 作者：闪电黑客
* 日期：2024/6/24 17:58

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Text.RegularExpressions;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// Rule后缀静态类型诊断
	/// </summary>
	public class StaticRuleDiagnosticConfig : DiagnosticConfigGroup
	{
		public StaticRuleDiagnosticConfig()
		{
			Screen = (Symbol) =>
			{
				if (Symbol is not ITypeSymbol TypeSymbol) return false;
				if (TypeSymbol.TypeKind != TypeKind.Class) return false;
				if (TypeSymbol.DeclaredAccessibility != Accessibility.Public) return false;
				if (!TypeSymbol.IsStatic) return false;
				string typeName = TypeSymbol?.ToDisplayString() ?? string.Empty;
				return Regex.IsMatch(typeName, "Rule$");
			};

			SetConfig(DiagnosticKey.ClassNaming, new DiagnosticConfig()
			{
				Title = "类型命名",
				MessageFormat = "类型命名开头要大写",
				DeclarationKind = SyntaxKind.ClassDeclaration,
				NeedComment = false
			});

			SetConfig(DiagnosticKey.ConstNaming, new DiagnosticConfig()
			{
				Title = "Rule静态类型常量字段禁止",
				MessageFormat = "Rule静态类型禁止声明常量字段",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				KeywordKinds = new() { SyntaxKind.ConstKeyword },
				Check = s => false,
				NeedComment = false,
			});

			SetConfig(DiagnosticKey.PublicFieldNaming, new DiagnosticConfig()
			{
				Title = "Rule静态类型公开字段禁止",
				MessageFormat = "Rule静态类型禁止声明公开字段",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				KeywordKinds = new() { SyntaxKind.PublicKeyword, },
				UnKeywordKinds = new() { SyntaxKind.ConstKeyword },
				Check = s => false,
				NeedComment = false,
			});

			SetConfig(DiagnosticKey.PrivateFieldNaming, new DiagnosticConfig()
			{
				Title = "Rule静态类型私有字段命名",
				MessageFormat = "Rule私有字段命名开头要大写",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				KeywordKinds = new() { SyntaxKind.PrivateKeyword, },
				UnKeywordKinds = new() { SyntaxKind.ConstKeyword, },
				NeedComment = false,
			});

			SetConfig(DiagnosticKey.PublicPropertyNaming, new DiagnosticConfig()
			{
				Title = "Rule静态类型公开属性禁止",
				MessageFormat = "Rule静态类型禁止声明公开属性",
				DeclarationKind = SyntaxKind.PropertyDeclaration,
				KeywordKinds = new() { SyntaxKind.PublicKeyword, },
				Check = s => false,
				NeedComment = false,
			});
			SetConfig(DiagnosticKey.PrivatePropertyNaming, new DiagnosticConfig()
			{
				Title = "Rule静态类型私有属性禁止",
				MessageFormat = "Rule静态类型禁止声明私有属性",
				DeclarationKind = SyntaxKind.PropertyDeclaration,
				KeywordKinds = new() { SyntaxKind.PrivateKeyword },
				Check = s => false,
				NeedComment = false,
			});
			SetConfig(DiagnosticKey.MethodNaming, new DiagnosticConfig()
			{
				Title = "方法命名",
				MessageFormat = "方法命名开头要大写",
				DeclarationKind = SyntaxKind.MethodDeclaration,
			});
			SetConfig(DiagnosticKey.ParameterNaming, new DiagnosticConfig()
			{
				Title = "方法参数命名",
				MessageFormat = "方法参数命名开头要小写",
				DeclarationKind = SyntaxKind.Parameter,
				Check = s => Regex.IsMatch(s, "^[a-z].*$"),
				FixCode = s => char.ToLower(s[0]) + s.Substring(1),
				NeedComment = false,
			});

		}
	}


}