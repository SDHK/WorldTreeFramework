/****************************************

* 作者：闪电黑客
* 日期：2024/6/20 15:19

* 描述：

*/

using Microsoft.CodeAnalysis.CSharp;
using System.Text.RegularExpressions;

namespace WorldTree.Analyzer
{

	/// <summary>
	/// 基础对象诊断
	/// </summary>
	public class ObjectDiagnosticConfig : DiagnosticConfigGroup
	{
		public ObjectDiagnosticConfig()
		{
			Screen = (s) => true;

			SetConfig(DiagnosticKey.ClassNaming, new DiagnosticConfig()
			{
				Title = "类型命名",
				MessageFormat = "类型命名开头要大写",
				DeclarationKind = SyntaxKind.ClassDeclaration,
			});

			SetConfig(DiagnosticKey.StructNaming, new DiagnosticConfig()
			{
				Title = "结构体命名",
				MessageFormat = "结构体命名开头要大写",
				DeclarationKind = SyntaxKind.StructDeclaration,
			});

			SetConfig(DiagnosticKey.InterfaceNaming, new DiagnosticConfig()
			{
				Title = "接口命名",
				MessageFormat = "接口命名开头要大写",
				DeclarationKind = SyntaxKind.InterfaceDeclaration,
			});

			SetConfig(DiagnosticKey.DelegateNaming, new DiagnosticConfig()
			{
				Title = "委托命名",
				MessageFormat = "委托命名开头要大写",
				DeclarationKind = SyntaxKind.DelegateDeclaration,
			});

			SetConfig(DiagnosticKey.EnumNaming, new DiagnosticConfig()
			{
				Title = "枚举命名",
				MessageFormat = "枚举命名开头要大写",
				DeclarationKind = SyntaxKind.EnumDeclaration,
			});

			SetConfig(DiagnosticKey.EnumMemberNaming, new DiagnosticConfig()
			{
				Title = "枚举成员命名",
				MessageFormat = "枚举成员命名开头要大写",
				DeclarationKind = SyntaxKind.EnumMemberDeclaration,
			});

			SetConfig(DiagnosticKey.ConstNaming, new DiagnosticConfig()
			{
				Title = "常量命名",
				MessageFormat = "常量命名都要大写",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				KeywordKinds = new() { SyntaxKind.ConstKeyword, },
				Check = (semanticModel, identifier) => Regex.IsMatch(identifier.Text, "^[A-Z0-9]+(_[A-Z0-9]+)*$"),
				FixCode = s => Regex.Replace(s, "([a-z])([A-Z])", "$1_$2").ToUpper()
			});

			SetConfig(DiagnosticKey.PublicFieldNaming, new DiagnosticConfig()
			{
				Title = "公开字段命名",
				MessageFormat = "公开字段命名开头要大写",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				KeywordKinds = new() { SyntaxKind.PublicKeyword, },
				UnKeywordKinds = new() { SyntaxKind.ConstKeyword, },
			});
			SetConfig(DiagnosticKey.PrivateFieldNaming, new DiagnosticConfig()
			{
				Title = "私有字段命名",
				MessageFormat = "私有字段命名开头要小写",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				KeywordKinds = new() { SyntaxKind.PrivateKeyword, },
				UnKeywordKinds = new() { SyntaxKind.ConstKeyword, },
				Check = (semanticModel, identifier) => Regex.IsMatch(identifier.Text, "^[a-z].*$"),
				FixCode = s => char.ToLower(s[0]) + s.Substring(1)
			});
			SetConfig(DiagnosticKey.ProtectedFieldNaming, new DiagnosticConfig()
			{
				Title = "保护字段命名",
				MessageFormat = "保护字段命名开头要小写",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				KeywordKinds = new() { SyntaxKind.ProtectedKeyword, },
				UnKeywordKinds = new() { SyntaxKind.ConstKeyword, },
				Check = (semanticModel, identifier) => Regex.IsMatch(identifier.Text, "^[a-z].*$"),
				FixCode = s => char.ToLower(s[0]) + s.Substring(1)
			});
			SetConfig(DiagnosticKey.PublicPropertyNaming, new DiagnosticConfig()
			{
				Title = "公开属性命名",
				MessageFormat = "公开属性命名开头要大写",
				DeclarationKind = SyntaxKind.PropertyDeclaration,
				KeywordKinds = new() { SyntaxKind.PublicKeyword, },
			});
			SetConfig(DiagnosticKey.PrivatePropertyNaming, new DiagnosticConfig()
			{
				Title = "私有属性命名",
				MessageFormat = "私有属性命名开头要大写",
				DeclarationKind = SyntaxKind.PropertyDeclaration,
				KeywordKinds = new() { SyntaxKind.PrivateKeyword },
			});
			SetConfig(DiagnosticKey.ProtectedPropertyNaming, new DiagnosticConfig()
			{
				Title = "保护属性命名",
				MessageFormat = "保护属性命名开头要大写",
				DeclarationKind = SyntaxKind.PropertyDeclaration,
				KeywordKinds = new() { SyntaxKind.ProtectedKeyword, },
			});
			SetConfig(DiagnosticKey.MethodNaming, new DiagnosticConfig()
			{
				Title = "方法命名",
				MessageFormat = "方法命名开头要大写",
				DeclarationKind = SyntaxKind.MethodDeclaration,
				UnKeywordKinds = new() { SyntaxKind.OverrideKeyword, },
			});

			SetConfig(DiagnosticKey.ParameterNaming, new DiagnosticConfig()
			{
				Title = "方法参数命名",
				MessageFormat = "方法参数命名开头要小写",
				DeclarationKind = SyntaxKind.Parameter,
				Check = (semanticModel, identifier) => Regex.IsMatch(identifier.Text, "^[a-z].*$"),
				FixCode = s => char.ToLower(s[0]) + s.Substring(1)
			});

			SetConfig(DiagnosticKey.TypeParameterNaming, new DiagnosticConfig()
			{
				Title = "泛型参数命名",
				MessageFormat = "泛型参数命名开头要大写",
				DeclarationKind = SyntaxKind.TypeParameter,
				NeedComment = false,
				Check = (semanticModel, identifier) => Regex.IsMatch(identifier.Text, "^[A-Z].*$"),
				FixCode = s => char.ToUpper(s[0]) + s.Substring(1)
			});

			SetConfig(DiagnosticKey.LocalVariableNaming, new DiagnosticConfig()
			{
				Title = "局部变量命名",
				MessageFormat = "局部变量命名开头要小写",
				DeclarationKind = SyntaxKind.LocalDeclarationStatement,
				Check = (semanticModel, identifier) => Regex.IsMatch(identifier.Text, "^[a-z].*$"),
				FixCode = s => char.ToLower(s[0]) + s.Substring(1),
				NeedComment = false
			});

			SetConfig(DiagnosticKey.LocalMethodNaming, new DiagnosticConfig()
			{
				Title = "局部方法命名",
				MessageFormat = "局部方法命名开头要大写",
				DeclarationKind = SyntaxKind.LocalFunctionStatement,
				Check = (semanticModel, identifier) => Regex.IsMatch(identifier.Text, "^[A-Z].*$"),
				FixCode = s => char.ToUpper(s[0]) + s.Substring(1)
			});


		}
	}
}