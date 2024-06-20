﻿/****************************************

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
	public class ObjectDiagnostic
	{
		/// <summary>
		/// 语法形式
		/// </summary>
		public List<SyntaxKind> SyntaxKinds;

		/// <summary>
		/// 类名筛选
		/// </summary>
		public Func<string, bool> CheckClassName;

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
			namingRule.Init(GetType().Name);
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
			SyntaxKinds = new() { SyntaxKind.ClassDeclaration, };
			CheckClassName = (s) => true;


			SetNamingRule(DiagnosticKey.ClassNaming, new CodeDiagnosticConfig()
			{
				Title = "类型命名规范诊断",
				MessageFormat = "类型：{0} 命名开头要大写",
				DeclarationSyntaxKinds = new() { SyntaxKind.ClassDeclaration },
				Check = s => Regex.IsMatch(s, "^[A-Z].*$"),
				Fix = s => char.ToUpper(s[0]) + s.Substring(1),
			});

			SetNamingRule(DiagnosticKey.PublicFieldNaming, new CodeDiagnosticConfig()
			{
				Title = "公开字段命名规范诊断",
				MessageFormat = "公开字段：{0} 命名开头要大写",
				DeclarationSyntaxKinds = new() { SyntaxKind.FieldDeclaration },
				KeywordSyntaxKinds = new() { SyntaxKind.PublicKeyword, },
				Check = s => Regex.IsMatch(s, "^[A-Z].*$"),
				Fix = s => char.ToUpper(s[0]) + s.Substring(1),
			});
			SetNamingRule(DiagnosticKey.PrivateFieldNaming, new CodeDiagnosticConfig()
			{
				Title = "私有字段命名规范诊断",
				MessageFormat = "私有字段：{0} 命名开头要小写",
				DeclarationSyntaxKinds = new() { SyntaxKind.FieldDeclaration },
				KeywordSyntaxKinds = new() { SyntaxKind.PrivateKeyword, },
				Check = s => Regex.IsMatch(s, "^[a-z].*$"),
				Fix = s => char.ToLower(s[0]) + s.Substring(1)
			});
			SetNamingRule(DiagnosticKey.ProtectedFieldNaming, new CodeDiagnosticConfig()
			{
				Title = "保护字段命名规范诊断",
				MessageFormat = "保护字段：{0} 命名开头要小写",
				DeclarationSyntaxKinds = new() { SyntaxKind.FieldDeclaration },
				KeywordSyntaxKinds = new() { SyntaxKind.ProtectedKeyword, },
				Check = s => Regex.IsMatch(s, "^[a-z].*$"),
				Fix = s => char.ToLower(s[0]) + s.Substring(1)
			});
			SetNamingRule(DiagnosticKey.PublicPropertyNaming, new CodeDiagnosticConfig()
			{
				Title = "公开属性命名规范诊断",
				MessageFormat = "公开属性：{0} 命名开头要大写",
				DeclarationSyntaxKinds = new() { SyntaxKind.PropertyDeclaration },
				KeywordSyntaxKinds = new() { SyntaxKind.PublicKeyword, },
				Check = s => Regex.IsMatch(s, "^[A-Z].*$"),
				Fix = s => char.ToUpper(s[0]) + s.Substring(1)
			});
			SetNamingRule(DiagnosticKey.PrivatePropertyNaming, new CodeDiagnosticConfig()
			{
				Title = "私有属性命名规范诊断",
				MessageFormat = "私有属性：{0} 命名开头要大写",
				DeclarationSyntaxKinds = new() { SyntaxKind.PropertyDeclaration },
				KeywordSyntaxKinds = new() { SyntaxKind.PrivateKeyword, },
				Check = s => Regex.IsMatch(s, "^[A-Z].*$"),
				Fix = s => char.ToUpper(s[0]) + s.Substring(1)
			});
			SetNamingRule(DiagnosticKey.ProtectedPropertyNaming, new CodeDiagnosticConfig()
			{
				Title = "保护属性命名规范诊断",
				MessageFormat = "保护属性：{0} 命名开头要大写",
				DeclarationSyntaxKinds = new() { SyntaxKind.PropertyDeclaration },
				KeywordSyntaxKinds = new() { SyntaxKind.ProtectedKeyword, },
				Check = s => Regex.IsMatch(s, "^[A-Z].*$"),
				Fix = s => char.ToUpper(s[0]) + s.Substring(1)
			});
			SetNamingRule(DiagnosticKey.MethodNaming, new CodeDiagnosticConfig()
			{
				Title = "方法命名规范诊断",
				MessageFormat = "方法：{0} 命名开头要大写",
				DeclarationSyntaxKinds = new() { SyntaxKind.MethodDeclaration },
				Check = s => Regex.IsMatch(s, "^[A-Z].*$"),
				Fix = s => char.ToUpper(s[0]) + s.Substring(1)
			});

			return this;
		}
	}
}