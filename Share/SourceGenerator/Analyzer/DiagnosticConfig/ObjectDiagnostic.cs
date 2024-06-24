/****************************************

* 作者：闪电黑客
* 日期：2024/6/20 15:19

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Text.RegularExpressions;

namespace WorldTree.Analyzer
{

	/// <summary>
	/// 基础对象诊断
	/// </summary>
	public class ObjectDiagnostic
	{
		/// <summary>
		/// 类名筛选
		/// </summary>
		public Func<ISymbol, bool> Screen;

		/// <summary>
		/// 命名规则
		/// </summary>
		public SortedDictionary<DiagnosticKey, CodeDiagnosticConfig> CodeDiagnostics = new();

		/// <summary>
		/// 获取命名规则
		/// </summary>
		public CodeDiagnosticConfig GetNamingRule(DiagnosticKey diagnosticKey)
		{
			return CodeDiagnostics[diagnosticKey];
		}

		/// <summary>
		/// 设置命名规则
		/// </summary>
		public void SetNamingRule(DiagnosticKey diagnosticKey, CodeDiagnosticConfig namingRule)
		{
			namingRule.Init(GetType().Name.Replace("Diagnostic", ""));
			if (!CodeDiagnostics.ContainsKey(diagnosticKey))
			{
				CodeDiagnostics.Add(diagnosticKey, namingRule);
			}
			else
			{
				CodeDiagnostics[diagnosticKey] = namingRule;
			}
		}

		/// <summary>
		/// 诊断配置初始化
		/// </summary>
		public virtual ObjectDiagnostic Init()
		{
			Screen = (s) => true;

			SetNamingRule(DiagnosticKey.ClassNaming, new CodeDiagnosticConfig()
			{
				Title = "类型命名规范诊断",
				MessageFormat = "类型命名开头要大写",
				DeclarationKind = SyntaxKind.ClassDeclaration,
			});

			SetNamingRule(DiagnosticKey.StructNaming, new CodeDiagnosticConfig()
			{
				Title = "结构体命名规范诊断",
				MessageFormat = "结构体命名开头要大写",
				DeclarationKind = SyntaxKind.StructDeclaration,
			});

			SetNamingRule(DiagnosticKey.InterfaceNaming, new CodeDiagnosticConfig()
			{
				Title = "接口命名规范诊断",
				MessageFormat = "接口命名开头要大写",
				DeclarationKind = SyntaxKind.InterfaceDeclaration,
			});

			SetNamingRule(DiagnosticKey.DelegateNaming, new CodeDiagnosticConfig()
			{
				Title = "委托命名规范诊断",
				MessageFormat = "委托命名开头要大写",
				DeclarationKind = SyntaxKind.DelegateDeclaration,
			});

			SetNamingRule(DiagnosticKey.EnumNaming, new CodeDiagnosticConfig()
			{
				Title = "枚举命名规范诊断",
				MessageFormat = "枚举命名开头要大写",
				DeclarationKind = SyntaxKind.EnumDeclaration,
			});

			SetNamingRule(DiagnosticKey.EnumMemberNaming, new CodeDiagnosticConfig()
			{
				Title = "枚举成员命名规范诊断",
				MessageFormat = "枚举成员命名开头要大写",
				DeclarationKind = SyntaxKind.EnumMemberDeclaration,
			});

			SetNamingRule(DiagnosticKey.ConstNaming, new CodeDiagnosticConfig()
			{
				Title = "常量命名规范诊断",
				MessageFormat = "常量命名都要大写",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				KeywordKinds = new() { SyntaxKind.ConstKeyword, },
				Check = s => Regex.IsMatch(s, "^[A-Z]+(_[A-Z]+)*$"),
				FixCode = s => Regex.Replace(s, "([a-z])([A-Z])", "$1_$2").ToUpper()
			});

			SetNamingRule(DiagnosticKey.PublicFieldNaming, new CodeDiagnosticConfig()
			{
				Title = "公开字段命名规范诊断",
				MessageFormat = "公开字段命名开头要大写",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				KeywordKinds = new() { SyntaxKind.PublicKeyword,},
				UnKeywordKinds = new() { SyntaxKind.ConstKeyword, },
			});
			SetNamingRule(DiagnosticKey.PrivateFieldNaming, new CodeDiagnosticConfig()
			{
				Title = "私有字段命名规范诊断",
				MessageFormat = "私有字段命名开头要小写",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				KeywordKinds = new() { SyntaxKind.PrivateKeyword, },
				UnKeywordKinds = new() { SyntaxKind.ConstKeyword, },
				Check = s => Regex.IsMatch(s, "^[a-z].*$"),
				FixCode = s => char.ToLower(s[0]) + s.Substring(1)
			});
			SetNamingRule(DiagnosticKey.ProtectedFieldNaming, new CodeDiagnosticConfig()
			{
				Title = "保护字段命名规范诊断",
				MessageFormat = "保护字段命名开头要小写",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				KeywordKinds = new() { SyntaxKind.ProtectedKeyword, },
				UnKeywordKinds = new() { SyntaxKind.ConstKeyword, },
				Check = s => Regex.IsMatch(s, "^[a-z].*$"),
				FixCode = s => char.ToLower(s[0]) + s.Substring(1)
			});
			SetNamingRule(DiagnosticKey.PublicPropertyNaming, new CodeDiagnosticConfig()
			{
				Title = "公开属性命名规范诊断",
				MessageFormat = "公开属性命名开头要大写",
				DeclarationKind = SyntaxKind.PropertyDeclaration,
				KeywordKinds = new() { SyntaxKind.PublicKeyword, },
			});
			SetNamingRule(DiagnosticKey.PrivatePropertyNaming, new CodeDiagnosticConfig()
			{
				Title = "私有属性命名规范诊断",
				MessageFormat = "私有属性命名开头要大写",
				DeclarationKind = SyntaxKind.PropertyDeclaration,
				KeywordKinds = new() { SyntaxKind.PrivateKeyword },
			});
			SetNamingRule(DiagnosticKey.ProtectedPropertyNaming, new CodeDiagnosticConfig()
			{
				Title = "保护属性命名规范诊断",
				MessageFormat = "保护属性命名开头要大写",
				DeclarationKind = SyntaxKind.PropertyDeclaration,
				KeywordKinds = new() { SyntaxKind.ProtectedKeyword, },
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