/****************************************

* 作者：闪电黑客
* 日期：2024/4/7 20:13

* 描述：异步通用通知法则 生成器帮助类

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class SendRuleAsyncGeneratorHelper
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 异步通用通知法则
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");

			for (int i = 0; i <= argumentCount; i++)
			{
				string genericsAngle = RuleGeneratorHelper.GetGenericsAngle(i);

				Code.AppendLine("    /// <summary>");
				Code.AppendLine("    /// 异步通用通知法则接口");
				Code.AppendLine("    /// </summary>");
				Code.AppendLine($"    public interface ISendRuleAsync{genericsAngle} : ISendRuleAsyncBase{genericsAngle} {{}}");
			}

			for (int i = 0; i <= argumentCount; i++)
			{
				string generics = RuleGeneratorHelper.GetGenerics(i);
				string genericsAngle = RuleGeneratorHelper.GetGenericsAngle(i);

				Code.AppendLine("    /// <summary>");
				Code.AppendLine("    /// 异步通用通知法则");
				Code.AppendLine("    /// </summary>");
				Code.AppendLine($"    public abstract class SendRuleAsync<N{generics}> : SendRuleAsyncBase<N, ISendRuleAsync{genericsAngle}{generics}> where N : class, INode, AsRule<ISendRuleAsync{genericsAngle}> {{}}");
			}

			Code.AppendLine("}");

			context.AddSource("SendRuleAsync.cs", SourceText.From(Code.ToString(), System.Text.Encoding.UTF8));
		}
	}
}