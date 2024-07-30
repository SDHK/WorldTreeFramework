/****************************************

* 作者：闪电黑客
* 日期：2024/6/25 17:10

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// 实例字段访问限制诊断
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class SimpleMemberAccessDiagnostic : NamingDiagnosticBase
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.SimpleMemberAccessExpression;

		protected override void DiagnosticAction(SyntaxNodeAnalysisContext context)
		{
			if (!ProjectDiagnosticSetting.ProjectDiagnostics.TryGetValue(context.Compilation.AssemblyName, out List<DiagnosticConfigGroup> DiagnosticGroups)) return;

			//获取语义模型
			SemanticModel semanticModel = context.SemanticModel;
			ISymbol? filedSymbol = context.SemanticModel.GetSymbolInfo(context.Node).Symbol;
			//字段和属性和事件的访问
			if (filedSymbol is IFieldSymbol || filedSymbol is IPropertySymbol || filedSymbol is IEventSymbol)
			{
				MemberAccessExpressionSyntax memberAccess = (MemberAccessExpressionSyntax)context.Node;
				foreach (DiagnosticConfigGroup DiagnosticGroup in DiagnosticGroups)
				{
					//检测字段来源类型是否符合要求
					if (!DiagnosticGroup.Screen(filedSymbol.ContainingType)) continue;

					// 假设 filedSymbol 是一个 IFieldSymbol 实例
					bool isProtected = filedSymbol.DeclaredAccessibility == Accessibility.Protected || filedSymbol.GetAttributes().Any(attr => attr.AttributeClass?.Name == "ProtectedAttribute");

					//检测是否有对应的诊断配置
					if (DiagnosticGroup.Diagnostics.TryGetValue(DiagnosticKey.SimpleMemberAccess, out DiagnosticConfig codeDiagnostic))
					{
						if (codeDiagnostic.Check.Invoke(memberAccess.Name.Identifier.Text))
						{
							if (CheckMemberAccess(context, isProtected))
							{
								context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, memberAccess.Name.GetLocation()));
							}
						}
						return;
					}
				}
			}
		}


		/// <summary>
		/// 判断是否需要进行诊断
		/// </summary>
		private bool CheckMemberAccess(SyntaxNodeAnalysisContext context, bool isProtected)
		{
			// 获取语义模型
			SemanticModel semanticModel = context.SemanticModel;
			ISymbol? memberAccessSymbol = context.SemanticModel.GetSymbolInfo(context.Node).Symbol;

			if (memberAccessSymbol == null) return false;

			MemberAccessExpressionSyntax memberAccess = (MemberAccessExpressionSyntax)context.Node;
			BaseTypeDeclarationSyntax parentTypeSyntax = TreeSyntaxHelper.GetParentType(memberAccess);
			INamedTypeSymbol? parentTypeSymbol = semanticModel.GetDeclaredSymbol(parentTypeSyntax);

			if (parentTypeSymbol == null) return false;

			//判断当前字段所在的类型是否是来源类型，是则跳过
			if (SymbolEqualityComparer.Default.Equals(memberAccessSymbol.ContainingType.OriginalDefinition, parentTypeSymbol.OriginalDefinition)) return false;
			if (SymbolEqualityComparer.Default.Equals(memberAccessSymbol.ContainingType, parentTypeSymbol)) return false;

			bool isInterfaceContainingType = memberAccessSymbol.ContainingType.TypeKind == TypeKind.Interface;
			bool isClassContainingType = memberAccessSymbol.ContainingType.TypeKind == TypeKind.Class;



			//判断当前字段所在的类型是否继承了来源接口，是则跳过
			if (isProtected && isInterfaceContainingType && NamedSymbolHelper.CheckInterface(parentTypeSymbol, memberAccessSymbol.ContainingType)) return false;
			//判断当前字段所在的类型是否继承了来源类型，是则跳过
			if (isProtected && isClassContainingType && NamedSymbolHelper.CheckBase(parentTypeSymbol, memberAccessSymbol.ContainingType)) return false;

			// 判断是否在静态类中
			if (parentTypeSyntax.Modifiers.Any(SyntaxKind.StaticKeyword))
			{
				MethodDeclarationSyntax parentMethodSyntax = TreeSyntaxHelper.GetParentMethod(memberAccess);
				// 判断是否在方法中
				if (parentMethodSyntax != null)
				{
					// 获取第一个参数的类型的符号信息
					ParameterSyntax? firstParameterSyntax = parentMethodSyntax.ParameterList.Parameters.FirstOrDefault();
					if (firstParameterSyntax == null) return true;
					ITypeSymbol? firstParameterTypeSymbol = semanticModel.GetSymbolInfo(firstParameterSyntax.Type).Symbol as ITypeSymbol;
					if (firstParameterTypeSymbol == null) return true;

					// 检查方法的第一个参数是否使用了 this 关键字
					if (firstParameterSyntax.Modifiers.Any(SyntaxKind.ThisKeyword))
					{
						//判断扩展类型是否是来源类型，是则跳过
						//if (memberAccessSymbol.ContainingType.ToDisplayString() == firstParameterTypeSymbol.ToString()) return false;
						if (SymbolEqualityComparer.Default.Equals(memberAccessSymbol.ContainingType.OriginalDefinition, firstParameterTypeSymbol.OriginalDefinition)) return false;
						if (SymbolEqualityComparer.Default.Equals(memberAccessSymbol.ContainingType, firstParameterTypeSymbol)) return false;
						//判断当前字段所在的类型是否继承了来源接口，是则跳过
						if (isProtected && isInterfaceContainingType && NamedSymbolHelper.CheckInterface(firstParameterTypeSymbol, memberAccessSymbol.ContainingType)) return false;
						//判断当前字段所在的类型是否继承了来源类型，是则跳过
						if (isProtected && isClassContainingType && NamedSymbolHelper.CheckBase(firstParameterTypeSymbol, memberAccessSymbol.ContainingType)) return false;
					}
					// 不是静态类型,判断是否在委托中
					else
					{
						(SyntaxNode? parentDelegate, ParameterSyntax? firstParameter1) = GetParentAnonymousDelegateAndFirstParameter(memberAccess);
						if (parentDelegate != null && firstParameter1 != null && firstParameter1.Identifier.Text == "self")
						{
							//if (memberAccessSymbol.ContainingType.ToDisplayString() == firstParameterTypeSymbol.ToString()) return false;
							if (SymbolEqualityComparer.Default.Equals(memberAccessSymbol.ContainingType.OriginalDefinition, firstParameterTypeSymbol.OriginalDefinition)) return false;
							if (SymbolEqualityComparer.Default.Equals(memberAccessSymbol.ContainingType, firstParameterTypeSymbol)) return false;
							if (isProtected && isInterfaceContainingType && NamedSymbolHelper.CheckInterface(firstParameterTypeSymbol, memberAccessSymbol.ContainingType)) return false;
							if (isProtected && isClassContainingType && NamedSymbolHelper.CheckBase(firstParameterTypeSymbol, memberAccessSymbol.ContainingType)) return false;
						}
					}
				}
			}
			else // 检测是否在Rule类型中
			{
				MethodDeclarationSyntax parentMethodSyntax = TreeSyntaxHelper.GetParentMethod(memberAccess);
				if (parentMethodSyntax != null)
				{
					ParameterSyntax? firstParameterSyntax = parentMethodSyntax.ParameterList.Parameters.FirstOrDefault();
					if (firstParameterSyntax == null) return true;
					ITypeSymbol? firstParameterTypeSymbol = semanticModel.GetSymbolInfo(firstParameterSyntax.Type).Symbol as ITypeSymbol;
					if (firstParameterTypeSymbol == null) return true;
					if (firstParameterSyntax.Identifier.Text.Trim() == "self")
					{
						INamedTypeSymbol? IRuleSymbol = NamedSymbolHelper.ToINamedTypeSymbol(context.Compilation, "WorldTree.IRule");
						if (IRuleSymbol == null) return true;
						// 判断类型是否继承了IRule接口
						if (NamedSymbolHelper.CheckInterface(parentTypeSymbol, IRuleSymbol))
						{
							//if (memberAccessSymbol.ContainingType.ToDisplayString() == firstParameterTypeSymbol?.ToString()) return false;
							if (SymbolEqualityComparer.Default.Equals(memberAccessSymbol.ContainingType.OriginalDefinition, firstParameterTypeSymbol.OriginalDefinition)) return false;
							if (SymbolEqualityComparer.Default.Equals(memberAccessSymbol.ContainingType, firstParameterTypeSymbol)) return false;
							if (isProtected && isInterfaceContainingType && NamedSymbolHelper.CheckInterface(firstParameterTypeSymbol, memberAccessSymbol.ContainingType)) return false;
							if (isProtected && isClassContainingType && NamedSymbolHelper.CheckBase(firstParameterTypeSymbol, memberAccessSymbol.ContainingType)) return false;
						}

						//判断当前字段所在的Rule类型是否是类中类，并且是来源类型，是则跳过
						if (parentTypeSymbol.ContainingType != null)
						{
							if (SymbolEqualityComparer.Default.Equals(memberAccessSymbol.ContainingType.OriginalDefinition, parentTypeSymbol.ContainingType.OriginalDefinition)) return false;
							if (SymbolEqualityComparer.Default.Equals(memberAccessSymbol.ContainingType, parentTypeSymbol.ContainingType)) return false;
						}
					}
				}
			}
			return true;
		}



		private static (SyntaxNode?, ParameterSyntax?) GetParentAnonymousDelegateAndFirstParameter(SyntaxNode syntaxNode)
		{
			SyntaxNode parentDelegate = TreeSyntaxHelper.GetParentAnonymousDelegate(syntaxNode);
			ParameterSyntax? firstParameter = null;

			switch (parentDelegate)
			{
				case ParenthesizedLambdaExpressionSyntax lambda:
					firstParameter = lambda?.ParameterList?.Parameters.FirstOrDefault();
					break;
				case SimpleLambdaExpressionSyntax lambda2:
					firstParameter = lambda2.Parameter;
					break;
				case AnonymousMethodExpressionSyntax lambda3:
					firstParameter = lambda3?.ParameterList?.Parameters.FirstOrDefault();
					break;
			}

			return (parentDelegate, firstParameter);
		}

	}
}