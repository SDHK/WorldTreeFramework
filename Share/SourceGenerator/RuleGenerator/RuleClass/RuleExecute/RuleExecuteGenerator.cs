/****************************************

* 作者：闪电黑客
* 日期：2024/6/6 10:28

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorldTree.SourceGenerator
{
	[Generator]
	internal class RuleExecuteGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new FindSubInterfaceSyntaxReceiver());

		}
		
		public void Execute(GeneratorExecutionContext context)
		{
			foreach (var syntaxTree in context.Compilation.SyntaxTrees)
			{
				var semanticModel = context.Compilation.GetSemanticModel(syntaxTree);

				var delegateInstances = syntaxTree.GetRoot()
					.DescendantNodes()
					.OfType<FieldDeclarationSyntax>()
					.Where(fieldDeclaration =>
					{
						var modifiers = fieldDeclaration.Modifiers;
						if (!modifiers.Any(SyntaxKind.StaticKeyword) || !modifiers.Any(SyntaxKind.PrivateKeyword))
						{
							return false;
						}

						var typeSymbol = semanticModel.GetTypeInfo(fieldDeclaration.Declaration.Type).Type;
						return typeSymbol.TypeKind == TypeKind.Delegate;
					})
					.ToList();

				// 在这里处理 delegateInstances
			}
		}

	}
}
