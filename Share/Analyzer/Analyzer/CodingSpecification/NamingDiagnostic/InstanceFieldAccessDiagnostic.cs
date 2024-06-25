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

namespace WorldTree.Analyzer
{
	/// <summary>
	/// 实例字段访问限制诊断
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class InstanceFieldAccessDiagnostic : NamingDiagnosticBase
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.SimpleMemberAccessExpression;

		protected override void DiagnosticAction(SyntaxNodeAnalysisContext context)
		{
			if (!ProjectDiagnosticSetting.ProjectDiagnostics.TryGetValue(context.Compilation.AssemblyName, out List<DiagnosticConfigGroup> DiagnosticGroups)) return;

			// 获取语义模型
			SemanticModel semanticModel = context.SemanticModel;

			ISymbol? filedSymbol = context.SemanticModel.GetSymbolInfo(context.Node).Symbol;

			//字段访问
			if (filedSymbol is IFieldSymbol instanceField)
			{
				MemberAccessExpressionSyntax memberAccess = (MemberAccessExpressionSyntax)context.Node;
				BaseTypeDeclarationSyntax parentType = TreeSyntaxHelper.GetParentType(memberAccess);

				//判断当前字段所在的类型是否是来源类型，是则跳过
				if (filedSymbol.ContainingType == semanticModel.GetDeclaredSymbol(parentType)) return;

				MethodDeclarationSyntax parentMethod = TreeSyntaxHelper.GetParentMethod(memberAccess);
				if (parentMethod != null)
				{
					ParameterSyntax? firstParameter = parentMethod.ParameterList.Parameters.FirstOrDefault();
					if (firstParameter != null)//判断方法是否为来源类型的静态扩展方法
					{
						// 使用语义模型获取第一个参数的类型的符号信息
						ITypeSymbol? firstParameterTypeSymbol = semanticModel.GetSymbolInfo(firstParameter.Type).Symbol as ITypeSymbol;

						// 如果方法是静态的，并且第一个参数使用了 this 关键字，则该方法是静态扩展方法
						bool isStaticMethod = parentMethod.Modifiers.Any(SyntaxKind.StaticKeyword);
						if (isStaticMethod)
						{
							// 检查方法的第一个参数是否使用了 this 关键字
							bool isFirstParameterWithThis = firstParameter?.Modifiers.Any(SyntaxKind.ThisKeyword) ?? false;
							if (isFirstParameterWithThis)
							{
								//判断扩展类型是否是来源类型，是则跳过
								if (instanceField.ContainingType.ToDisplayString() == firstParameterTypeSymbol?.ToString()) return;
							}
						}
						//如果不是静态扩展方法，则判断方法第一个参数名称是否为self
						else if (firstParameter?.Identifier.Text.Trim() == "self")
						{
							//判断扩展类型是否是来源类型，是则跳过
							if (instanceField.ContainingType.ToDisplayString() == firstParameterTypeSymbol?.ToString()) return;
						}
					}
				}
				else
				{
					// 检测是否在匿名委托中，并获取第一个参数
					(SyntaxNode parentDelegate, ParameterSyntax firstParameter1) = GetParentAnonymousDelegateAndFirstParameter(memberAccess);
					if (parentDelegate != null && firstParameter1 != null && firstParameter1.Identifier.Text == "self")
					{
						ITypeSymbol? firstParameterTypeSymbol = semanticModel.GetSymbolInfo(firstParameter1.Type).Symbol as ITypeSymbol;
						// 判断扩展类型是否是来源类型，是则跳过
						if (instanceField.ContainingType.ToDisplayString() == firstParameterTypeSymbol?.ToString()) return;
					}
				}

				foreach (DiagnosticConfigGroup DiagnosticGroup in DiagnosticGroups)
				{
					//检测字段来源类型是否符合要求
					if (!DiagnosticGroup.Screen(filedSymbol.ContainingType)) continue;
					//检测是否有对应的诊断配置
					if (!DiagnosticGroup.Diagnostics.TryGetValue(DiagnosticKey.InstanceFieldAccess, out DiagnosticConfig codeDiagnostic)) continue;

					if (codeDiagnostic.Check.Invoke(memberAccess.Name.Identifier.Text))
					{
						context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, memberAccess.Name.GetLocation()));
					}
				}
			}

		}

		private static (SyntaxNode, ParameterSyntax) GetParentAnonymousDelegateAndFirstParameter(SyntaxNode syntaxNode)
		{
			SyntaxNode parentDelegate = TreeSyntaxHelper.GetParentAnonymousDelegate(syntaxNode);
			ParameterSyntax firstParameter = null;

			switch (parentDelegate)
			{
				case ParenthesizedLambdaExpressionSyntax lambda:
					firstParameter = lambda.ParameterList.Parameters.FirstOrDefault();
					break;
				case SimpleLambdaExpressionSyntax lambda2:
					firstParameter = lambda2.Parameter;
					break;
				case AnonymousMethodExpressionSyntax lambda3:
					firstParameter = lambda3.ParameterList.Parameters.FirstOrDefault();
					break;
			}

			return (parentDelegate, firstParameter);
		}

	}
}