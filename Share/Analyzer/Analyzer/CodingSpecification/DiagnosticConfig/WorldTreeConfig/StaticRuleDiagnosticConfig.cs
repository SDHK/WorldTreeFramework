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

			SetConfig(DiagnosticKey.ConstNaming, new DiagnosticConfig()
			{
				Title = "Rule常量命名",
				MessageFormat = "Rule静态类型中不准写常量字段",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				KeywordKinds = new() { SyntaxKind.ConstKeyword },
				NeedComment = false,
				Check = s => false,
			});

			SetConfig(DiagnosticKey.PublicFieldNaming, new DiagnosticConfig()
			{
				Title = "Rule公开字段命名",
				MessageFormat = "Rule静态类型中不准写公开字段",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				KeywordKinds = new() { SyntaxKind.PublicKeyword, },
				UnKeywordKinds = new() { SyntaxKind.ConstKeyword },
				NeedComment = false,
				Check = s => false,
			});

			SetConfig(DiagnosticKey.PrivateFieldNaming, new DiagnosticConfig()
			{
				Title = "Rule私有字段命名",
				MessageFormat = "Rule私有字段命名开头要大写",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				KeywordKinds = new() { SyntaxKind.PrivateKeyword, },
				UnKeywordKinds = new() { SyntaxKind.ConstKeyword, },
				NeedComment = false,
			});


			SetConfig(DiagnosticKey.PublicPropertyNaming, new DiagnosticConfig()
			{
				Title = "公开属性命名",
				MessageFormat = "Rule静态类型中不准写公开属性",
				DeclarationKind = SyntaxKind.PropertyDeclaration,
				KeywordKinds = new() { SyntaxKind.PublicKeyword, },
				NeedComment = false,
				Check = s => false,

			});
			SetConfig(DiagnosticKey.PrivatePropertyNaming, new DiagnosticConfig()
			{
				Title = "私有属性命名",
				MessageFormat = "Rule静态类型中不准写私有属性",
				DeclarationKind = SyntaxKind.PropertyDeclaration,
				KeywordKinds = new() { SyntaxKind.PrivateKeyword },
				NeedComment = false,
				Check = s => false,
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
				NeedComment = false,
				Check = s => Regex.IsMatch(s, "^[a-z].*$"),
				FixCode = s => char.ToLower(s[0]) + s.Substring(1)
			});

		}
	}


}