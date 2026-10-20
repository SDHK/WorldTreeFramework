/****************************************

* 作者：闪电黑客
* 日期：2024/4/9 20:41

* 描述：

*/

using Microsoft.CodeAnalysis;
using System;

namespace WorldTree.SourceGenerator
{
	/// <summary>
	/// 法则集合扩展调用方法生成器
	/// </summary>
	public abstract class RuleExtensionMethodGenerator<C> : SourceGeneratorBase<C>
		where C : ProjectGeneratorsConfig, new()
	{
		public override void Initialize(GeneratorInitializationContext context)
		{
		}

		public override void ExecuteCore(GeneratorExecutionContext context)
		{
			try
			{
				var receiver = SyntaxReceiverHelper.Fill(context.Compilation, new FindCoreSyntaxReceiver());
				if (receiver.isGenerator == false) return;

				RuleListSendRuleGeneratorHelper.Execute(context);
				RuleListSendAsyncRuleGeneratorHelper.Execute(context);
				RuleListCallRuleGeneratorHelper.Execute(context);
				RuleListCallAsyncRuleGeneratorHelper.Execute(context);

				RuleListSendRefRuleGeneratorHelper.Execute(context);

				RuleGroupSendRuleGeneratorHelper.Execute(context);
				RuleGroupSendAsyncRuleGeneratorHelper.Execute(context);
				RuleGroupCallRuleGeneratorHelper.Execute(context);
				RuleGroupCallAsyncRuleGeneratorHelper.Execute(context);

				RuleGroupSendRefRuleGeneratorHelper.Execute(context);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}
	}
}