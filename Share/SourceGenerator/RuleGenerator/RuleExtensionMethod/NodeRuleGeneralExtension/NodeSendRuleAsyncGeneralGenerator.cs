/****************************************

* 作者：闪电黑客
* 日期：2024/4/11 20:02

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class NodeSendRuleAsyncGeneralGenerator
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 节点执行异步通知法则
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");
			Code.AppendLine("	public static class NodeSendRuleAsyncGeneral");
			Code.Append("	{");

			for (int i = 0; i <= argumentCount; i++)
			{
				string generics = RuleGeneratorHelper.GetGenerics(i);
				string genericsAngle = RuleGeneratorHelper.GetGenericsAngle(i);
				string genericParameter = RuleGeneratorHelper.GetGenericParameter(i);
				string genericTypeParameter = RuleGeneratorHelper.GetGenericTypeParameter(i);
				Code.Append
($@"

		/// <summary>
		/// 尝试执行异步通知法则
		/// </summary>
		public static TreeTask<bool> TrySendAsync{genericsAngle}(this INode self{genericTypeParameter})
			=> self.TrySendRuleAsync(TypeInfo<ISendRuleAsync{genericsAngle}>.Default{genericParameter});

		/// <summary>
		/// 执行异步通知法则
		/// </summary>
		public static TreeTask SendAsync<N{generics}>(this N self{genericTypeParameter})
			where N : class, INode, AsRule<ISendRuleAsync{genericsAngle}>
		=> self.SendRuleAsync(TypeInfo<ISendRuleAsync{genericsAngle}>.Default{genericParameter});
");
			}
			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("NodeSendRuleAsyncGeneral.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}