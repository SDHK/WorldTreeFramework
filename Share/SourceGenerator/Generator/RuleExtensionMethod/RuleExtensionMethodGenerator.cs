/****************************************

* 作者：闪电黑客
* 日期：2024/4/9 20:41

* 描述：

*/

using Microsoft.CodeAnalysis;

namespace WorldTree.SourceGenerator
{
	[Generator]
	internal class RuleExtensionMethodGenerator : ISourceGenerator
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

				RuleListSendRuleGenerator.Execute(context);
				RuleListSendAsyncRuleGenerator.Execute(context);
				RuleListCallRuleGenerator.Execute(context);
				RuleListCallAsyncRuleGenerator.Execute(context);

				RuleListSendRefRuleGenerator.Execute(context);

				RuleGroupSendRuleGenerator.Execute(context);
				RuleGroupSendAsyncRuleGenerator.Execute(context);
				RuleGroupCallRuleGenerator.Execute(context);
				RuleGroupCallAsyncRuleGenerator.Execute(context);

				RuleGroupSendRefRuleGenerator.Execute(context);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}
	}
}