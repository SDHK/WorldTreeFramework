/****************************************

* 作者：闪电黑客
* 日期：2024/4/9 20:41

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorldTree.SourceGenerator
{
	[Generator]
	internal class RuleExtensionMethodGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new SyntaxContextReceiver());
		}

		public void Execute(GeneratorExecutionContext context)
		{
			try
			{
				if (!(context.SyntaxReceiver is SyntaxContextReceiver receiver and not null)) return;
				if (receiver.isGenerator == false) return;

				RuleListSendRuleGenerator.Execute(context);
				RuleListSendAsyncRuleGenerator.Execute(context);
				RuleListCallRuleGenerator.Execute(context);
				RuleListCallAsyncRuleGenerator.Execute(context);

				RuleGroupSendRuleGenerator.Execute(context);
				RuleGroupSendAsyncRuleGenerator.Execute(context);
				RuleGroupCallRuleGenerator.Execute(context);
				RuleGroupCallAsyncRuleGenerator.Execute(context);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		private class SyntaxContextReceiver : ISyntaxReceiver
		{
			public bool isGenerator = false;

			public void OnVisitSyntaxNode(SyntaxNode node)
			{
				//判断是否是类
				if (node is not InterfaceDeclarationSyntax interfaceDeclarationSyntax) return;

				//判断类型是否是IRule,因为IRule是基类在核心程序集中
				if (interfaceDeclarationSyntax.Identifier.ValueText == "IRule")
				{
					isGenerator = true;
				}
			}
		}
	}
}