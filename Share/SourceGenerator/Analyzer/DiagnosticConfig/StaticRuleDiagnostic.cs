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
	public class StaticRuleDiagnostic : ObjectDiagnostic
	{
		public override ObjectDiagnostic Init()
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

			SetNamingRule(DiagnosticKey.ConstNaming, new CodeDiagnosticConfig()
			{
				Title = "Rule常量命名规范诊断",
				MessageFormat = "Rule静态类型中不准写常量字段",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				KeywordKinds = new() { SyntaxKind.ConstKeyword},
				NeedComment = false,
				Check = s => false,
			});

			SetNamingRule(DiagnosticKey.PublicFieldNaming, new CodeDiagnosticConfig()
			{
				Title = "Rule公开字段命名规范诊断",
				MessageFormat = "Rule静态类型中不准写公开字段",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				KeywordKinds = new() { SyntaxKind.PublicKeyword, },
				UnKeywordKinds = new() { SyntaxKind.ConstKeyword },
				NeedComment = false,
				Check = s => false,
			});

			SetNamingRule(DiagnosticKey.PrivateFieldNaming, new CodeDiagnosticConfig()
			{
				Title = "Rule私有字段命名规范诊断",
				MessageFormat = "Rule私有字段命名开头要大写",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				KeywordKinds = new() { SyntaxKind.PrivateKeyword, },
				UnKeywordKinds = new() { SyntaxKind.ConstKeyword, },
				NeedComment = false,
			});
			

			SetNamingRule(DiagnosticKey.PublicPropertyNaming, new CodeDiagnosticConfig()
			{
				Title = "公开属性命名规范诊断",
				MessageFormat = "Rule静态类型中不准写公开属性",
				DeclarationKind = SyntaxKind.PropertyDeclaration,
				KeywordKinds = new() { SyntaxKind.PublicKeyword, },
				NeedComment = false,
				Check = s => false,

			});
			SetNamingRule(DiagnosticKey.PrivatePropertyNaming, new CodeDiagnosticConfig()
			{
				Title = "私有属性命名规范诊断",
				MessageFormat = "Rule静态类型中不准写私有属性",
				DeclarationKind = SyntaxKind.PropertyDeclaration,
				KeywordKinds = new() { SyntaxKind.PrivateKeyword },
				NeedComment = false,
				Check = s => false,
			});
			SetNamingRule(DiagnosticKey.MethodNaming, new CodeDiagnosticConfig()
			{
				Title = "方法命名规范诊断",
				MessageFormat = "方法命名开头要大写",
				DeclarationKind = SyntaxKind.MethodDeclaration,
			});
			SetNamingRule(DiagnosticKey.ParameterNaming, new CodeDiagnosticConfig()
			{
				Title = "方法参数命名规范诊断",
				MessageFormat = "方法参数命名开头要小写",
				DeclarationKind = SyntaxKind.Parameter,
				Check = s => Regex.IsMatch(s, "^[a-z].*$"),
				FixCode = s => char.ToLower(s[0]) + s.Substring(1)
			});

			return this;
		}
	}


}