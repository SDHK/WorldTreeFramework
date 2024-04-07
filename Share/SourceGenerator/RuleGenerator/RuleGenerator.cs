/****************************************

* 作者：闪电黑客
* 日期：2024/4/7 17:08

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WorldTree.SourceGenerator
{
	[Generator]
	internal class RuleGenerator : ISourceGenerator
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

				SendRuleBaseGeneratorHelper.Execute(context);
				SendRuleAsyncBaseGeneratorHelper.Execute(context);
				CallRuleBaseGeneratorHelper.Execute(context);
				CallRuleAsyncBaseGeneratorHelper.Execute(context);
				SendRuleGeneratorHelper.Execute(context);
				SendRuleAsyncGeneratorHelper.Execute(context);
				CallRuleGeneratorHelper.Execute(context);
				CallRuleAsyncGeneratorHelper.Execute(context);
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