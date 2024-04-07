/****************************************

* 作者：闪电黑客
* 日期：2024/4/7 19:55

* 描述：通用通知法则 生成器帮助类

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class SendRuleGeneratorHelper
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 通用通知法则
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");

			for (int i = 0; i <= argumentCount; i++)
			{
				string genericsAngle = RuleGeneratorHelper.GetGenericsAngle(i);

				Code.AppendLine("    /// <summary>");
				Code.AppendLine("    /// 通用通知法则接口");
				Code.AppendLine("    /// </summary>");
				Code.AppendLine($"    public interface ISendRule{genericsAngle} : ISendRuleBase{genericsAngle} {{}}");
			}

			for (int i = 0; i <= argumentCount; i++)
			{
				string generics = RuleGeneratorHelper.GetGenerics(i);
				string genericsAngle = RuleGeneratorHelper.GetGenericsAngle(i);

				Code.AppendLine("    /// <summary>");
				Code.AppendLine("    /// 通用通知法则");
				Code.AppendLine("    /// </summary>");
				Code.AppendLine($"    public abstract class SendRule<N{generics}> : SendRuleBase<N, ISendRule{genericsAngle}{generics}> where N : class, INode, AsRule<ISendRule{genericsAngle}> {{}}");
			}

			Code.AppendLine("}");

			context.AddSource("SendRule.cs", SourceText.From(Code.ToString(), System.Text.Encoding.UTF8));
		}
	}
}