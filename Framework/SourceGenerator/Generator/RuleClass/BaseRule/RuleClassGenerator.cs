/****************************************

* 作者：闪电黑客
* 日期：2024/4/7 17:08

* 描述：法则生成器

*/

using Microsoft.CodeAnalysis;
using System;

namespace WorldTree.SourceGenerator
{
	public abstract class RuleClassGenerator : SourceGeneratorBase
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

				SendRuleBaseGeneratorHelper.Execute(context);
				SendRuleAsyncBaseGeneratorHelper.Execute(context);
				CallRuleBaseGeneratorHelper.Execute(context);
				CallRuleAsyncBaseGeneratorHelper.Execute(context);

				SendRefRuleBaseGeneratorHelper.Execute(context);
				AwakeRuleGeneratorHelper.Execute(context);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}
	}
}