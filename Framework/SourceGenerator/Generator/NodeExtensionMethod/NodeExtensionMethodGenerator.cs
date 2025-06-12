/****************************************

* 作者：闪电黑客
* 日期：2024/4/15 14:45

* 描述：

*/

using Microsoft.CodeAnalysis;
using System;

namespace WorldTree.SourceGenerator
{
	public abstract class NodeExtensionMethodGenerator : SourceGeneratorBase
	{
		public override void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new FindCoreSyntaxReceiver());
		}

		public override void ExecuteCore(GeneratorExecutionContext context)
		{
			try
			{
				if (!(context.SyntaxReceiver is FindCoreSyntaxReceiver receiver and not null)) return;
				if (receiver.isGenerator == false) return;

				NodeSendRuleGeneratorHelper.Execute(context);
				NodeSendRuleAsyncGeneratorHelper.Execute(context);
				NodeCallRuleGeneratorHelper.Execute(context);
				NodeCallRuleAsyncGeneratorHelper.Execute(context);

				RuleExecutorSendRuleGeneratorHelper.Execute(context);
				RuleExecutorCallRuleGeneratorHelper.Execute(context);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}
	}
}