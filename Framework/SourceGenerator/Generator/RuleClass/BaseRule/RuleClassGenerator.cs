/****************************************

* 作者：闪电黑客
* 日期：2024/4/7 17:08

* 描述：法则生成器

*/

using Microsoft.CodeAnalysis;
using System;

namespace WorldTree.SourceGenerator
{
	[Generator]
	internal class RuleClassGenerator : ISourceGenerator
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

				SendRuleBaseGenerator.Execute(context);
				SendRuleAsyncBaseGenerator.Execute(context);
				CallRuleBaseGenerator.Execute(context);
				CallRuleAsyncBaseGenerator.Execute(context);

				SendRefRuleBaseGenerator.Execute(context);
				AwakeRuleGenerator.Execute(context);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}
	}
}