/****************************************

* 作者：闪电黑客
* 日期：2024/4/7 20:21

* 描述：异步通用调用法则 生成器帮助类

*/

using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class CallRuleAsyncGeneratorHelper
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 异步通用调用法则
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");

			for (int i = 0; i <= argumentCount; i++)
			{
				string generics = RuleGeneratorHelper.GetGenerics(i, false);

				Code.AppendLine("    /// <summary>");
				Code.AppendLine("    /// 异步通用调用法则接口");
				Code.AppendLine("    /// </summary>");
				Code.AppendLine($"    public interface ICallRuleAsync<{generics}OutT> : ICallRuleAsyncBase<{generics}OutT> {{}}");
			}

			for (int i = 0; i <= argumentCount; i++)
			{
				string genericsPre = RuleGeneratorHelper.GetGenerics(i);
				string generics = RuleGeneratorHelper.GetGenerics(i, false);

				Code.AppendLine("    /// <summary>");
				Code.AppendLine("    /// 异步通用调用法则");
				Code.AppendLine("    /// </summary>");
				Code.AppendLine($"    public abstract class CallRuleAsync<N{genericsPre}, OutT> : CallRuleAsyncBase<N, ICallRuleAsync<{generics}OutT>, {generics}OutT> where N : class, INode, AsRule<ICallRuleAsync<{generics}OutT>> {{}}");
			}

			Code.AppendLine("}");

			context.AddSource("CallRuleAsync.cs", SourceText.From(Code.ToString(), System.Text.Encoding.UTF8));
		}
	}
}