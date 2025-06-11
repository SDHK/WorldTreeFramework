/****************************************

* 作者：闪电黑客
* 日期：2024/4/15 14:45

* 描述：

*/

using Microsoft.CodeAnalysis;
using System;

namespace WorldTree.SourceGenerator
{
	[Generator]
	internal class NodeExtensionMethodGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new FindCoreSyntaxReceiver());
		}

		public void Execute(GeneratorExecutionContext context)
		{
			try
			{
				if (!(context.SyntaxReceiver is FindCoreSyntaxReceiver receiver and not null)) return;
				if (receiver.isGenerator == false) return;

				NodeSendRuleGenerator.Execute(context);
				NodeSendRuleAsyncGenerator.Execute(context);
				NodeCallRuleGenerator.Execute(context);
				NodeCallRuleAsyncGenerator.Execute(context);

				RuleExecutorSendRuleGenerator.Execute(context);
				RuleExecutorCallRuleGenerator.Execute(context);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}
	}
}