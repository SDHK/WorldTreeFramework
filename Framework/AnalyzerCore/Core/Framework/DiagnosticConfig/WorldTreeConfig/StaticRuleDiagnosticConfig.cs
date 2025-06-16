/****************************************

* 作者：闪电黑客
* 日期：2024/6/24 17:58

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text.RegularExpressions;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// Rule后缀静态类型诊断
	/// </summary>
	public class StaticRuleDiagnosticConfig : DiagnosticConfigGroup
	{
		public bool IsRuleDelegateType(SemanticModel semanticModel, SyntaxToken identifier)
		{
			if (identifier.Parent is VariableDeclaratorSyntax variableDeclarator)
			{
				if (variableDeclarator.Parent is VariableDeclarationSyntax parentVariableDeclarator)
				{
					ITypeSymbol fieldTypeSymbol = semanticModel.GetTypeInfo(parentVariableDeclarator.Type).Type;
					if (fieldTypeSymbol.TypeKind == TypeKind.Delegate)
					{
						//判断类型名称是否以On开头
						return Regex.IsMatch(fieldTypeSymbol.Name, "^On.*$");
					}
				}
			}
			return false;
		}

		public bool IsNodeRuleAttributeMethod(SemanticModel semanticModel, SyntaxToken identifier)
		{
			if (identifier.Parent is not MethodDeclarationSyntax methodDeclaration)
				return false;

			if (methodDeclaration.AttributeLists.Count == 0)
				return false;

			foreach (var attributeList in methodDeclaration.AttributeLists)
			{
				foreach (var attribute in attributeList.Attributes)
				{
					if (attribute.Name.ToString().Contains("NodeRule"))
					{
						return true;
					}
				}
			}

			return false;
		}


		public StaticRuleDiagnosticConfig()
		{
			Screen = (Compilation, Symbol) =>
			{
				if (Symbol is not ITypeSymbol TypeSymbol) return false;
				if (TypeSymbol.TypeKind != TypeKind.Class) return false;
				if (!TypeSymbol.IsStatic) return false;
				string typeName = TypeSymbol?.ToDisplayString() ?? string.Empty;
				return Regex.IsMatch(typeName, "Rule$");
			};


			SetConfig(DiagnosticKey.ClassNaming, new DiagnosticConfig()
			{
				Title = "类型命名",
				MessageFormat = "类型命名开头要大写",
				DeclarationKind = SyntaxKind.ClassDeclaration,
				Check = (semanticModel, identifier) =>
				{
					if (IsRuleDelegateType(semanticModel, identifier)) return true;
					return Regex.IsMatch(identifier.Text, "^[A-Z].*$");
				},
				NeedComment = (semanticModel, identifier) => false
			});


			SetConfig(DiagnosticKey.PublicFieldNaming, new DiagnosticConfig()
			{
				Title = "Rule静态类型公开字段命名",
				MessageFormat = "公开字段命名开头要大写",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				KeywordKinds = new() { SyntaxKind.PublicKeyword, },
				UnKeywordKinds = new() { SyntaxKind.ConstKeyword },
				Check = (semanticModel, identifier) =>
				{
					if (IsRuleDelegateType(semanticModel, identifier)) return true;
					return Regex.IsMatch(identifier.Text, "^[A-Z].*$");
				},
				NeedComment = (semanticModel, identifier) => false
			});

			SetConfig(DiagnosticKey.PrivateFieldNaming, new DiagnosticConfig()
			{
				Title = "Rule静态类型私有字段命名",
				MessageFormat = "私有字段命名开头要小写",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				KeywordKinds = new() { SyntaxKind.PrivateKeyword, },
				UnKeywordKinds = new() { SyntaxKind.ConstKeyword, },
				Check = (semanticModel, identifier) =>
				{
					if (IsRuleDelegateType(semanticModel, identifier)) return true;
					return Regex.IsMatch(identifier.Text, "^[a-z].*$");

				},
				FixCode = s => char.ToLower(s[0]) + s.Substring(1),
				NeedComment = (semanticModel, identifier) => false
			});

			SetConfig(DiagnosticKey.PublicPropertyNaming, new DiagnosticConfig()
			{
				Title = "Rule静态类型公开属性命名",
				MessageFormat = "公开属性命名开头要大写",
				DeclarationKind = SyntaxKind.PropertyDeclaration,
				KeywordKinds = new() { SyntaxKind.PublicKeyword, },
				Check = (semanticModel, identifier) =>
				{
					if (IsRuleDelegateType(semanticModel, identifier)) return true;
					return Regex.IsMatch(identifier.Text, "^[A-Z].*$");
				},
				NeedComment = (semanticModel, identifier) => false
			});
			SetConfig(DiagnosticKey.PrivatePropertyNaming, new DiagnosticConfig()
			{
				Title = "Rule静态类型私有属性命名",
				MessageFormat = "私有属性命名开头要大写",
				DeclarationKind = SyntaxKind.PropertyDeclaration,
				KeywordKinds = new() { SyntaxKind.PrivateKeyword },
				Check = (semanticModel, identifier) =>
				{
					if (IsRuleDelegateType(semanticModel, identifier)) return true;
					return Regex.IsMatch(identifier.Text, "^[A-Z].*$");
				},
				NeedComment = (semanticModel, identifier) => false
			});

			SetConfig(DiagnosticKey.MethodNaming, new DiagnosticConfig()
			{
				Title = "Rule静态类型方法命名",
				MessageFormat = "方法命名开头要大写",
				DeclarationKind = SyntaxKind.MethodDeclaration,
				UnKeywordKinds = new() { SyntaxKind.OverrideKeyword, },
				NeedComment = (semanticModel, identifier) =>
				{
					return !IsNodeRuleAttributeMethod(semanticModel, identifier);
				}
			});
		}
	}


}