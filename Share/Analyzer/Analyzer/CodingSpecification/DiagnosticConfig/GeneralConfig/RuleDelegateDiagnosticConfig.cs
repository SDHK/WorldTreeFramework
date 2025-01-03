/****************************************

* 作者：闪电黑客
* 日期：2025/1/3 14:44

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WorldTree.Analyzer
{
	public class RuleDelegateDiagnosticConfig : DiagnosticConfigGroup
	{
		private List<SyntaxKind> syntaxKinds = new List<SyntaxKind> { SyntaxKind.StaticKeyword, SyntaxKind.PrivateKeyword };

		public RuleDelegateDiagnosticConfig()
		{
			Screen = (Symbol) =>
			{
				if (Symbol is not ITypeSymbol TypeSymbol) return false;
				if (TypeSymbol.TypeKind != TypeKind.Delegate) return false;
				//判断类型名称是否以On开头
				return Regex.IsMatch(TypeSymbol.Name, "^On.*$");
			};

			SetConfig(DiagnosticKey.ClassFieldNaming, new DiagnosticConfig()
			{
				Title = "Rule委托类型字段命名",
				MessageFormat = "Rule委托类型字段命名开头要静态私有和大写",
				DeclarationKind = SyntaxKind.FieldDeclaration,
				Check = (semanticModel, identifier) =>
				{
					if (identifier.Parent is VariableDeclaratorSyntax variableDeclarator)
					{
						if (variableDeclarator.Parent.Parent is FieldDeclarationSyntax fieldDeclaration)
						{
							if (!fieldDeclaration.Modifiers.Any(SyntaxKind.PublicKeyword) && fieldDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword))
							{
								return Regex.IsMatch(identifier.Text, "^[A-Z].*$");
							}
						}
					}
					return false;
				},
				NeedComment = false,
			});
			SetConfig(DiagnosticKey.ClassPropertyNaming, new DiagnosticConfig()
			{
				Title = "Rule委托类型属性禁止",
				MessageFormat = "Rule委托类型属性禁止",
				DeclarationKind = SyntaxKind.PropertyDeclaration,
				Check = (semanticModel, identifier) => false,
				FixCode = s => string.Empty,
				NeedComment = false,
			});
			SetConfig(DiagnosticKey.ClassLocalVariableNaming, new DiagnosticConfig()
			{
				Title = "Rule委托类型局部变量禁止",
				MessageFormat = "Rule委托类型局部变量禁止",
				DeclarationKind = SyntaxKind.LocalDeclarationStatement,
				Check = (semanticModel, identifier) => false,
				FixCode = s => string.Empty,
				NeedComment = false,
			});


		}

	}
}