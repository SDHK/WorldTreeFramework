/****************************************

* 作者：闪电黑客
* 日期：2024/4/7 20:18

* 描述：通用调用法则 生成器帮助类

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class CallRuleGeneratorHelper
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 通用调用法则
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");

			for (int i = 0; i <= argumentCount; i++)
			{
				string generics = RuleGeneratorHelper.GetGenerics(i, false);

				Code.AppendLine("    /// <summary>");
				Code.AppendLine("    /// 通用调用法则接口");
				Code.AppendLine("    /// </summary>");
				Code.AppendLine($"    public interface ICallRule<{generics}OutT> : ICallRuleBase<{generics}OutT> {{}}");
			}

			for (int i = 0; i <= argumentCount; i++)
			{
				string genericsPre = RuleGeneratorHelper.GetGenerics(i);
				string generics = RuleGeneratorHelper.GetGenerics(i, false);

				Code.AppendLine("    /// <summary>");
				Code.AppendLine("    /// 通用调用法则");
				Code.AppendLine("    /// </summary>");
				Code.AppendLine($"    public abstract class CallRule<N{genericsPre}, OutT> : CallRuleBase<N, ICallRule<{generics}OutT>, {generics}OutT> where N : class, INode, AsRule<ICallRule<{generics}OutT>> {{}}");
			}

			Code.AppendLine("}");

			context.AddSource("CallRule.cs", SourceText.From(Code.ToString(), System.Text.Encoding.UTF8));
		}
	}
}