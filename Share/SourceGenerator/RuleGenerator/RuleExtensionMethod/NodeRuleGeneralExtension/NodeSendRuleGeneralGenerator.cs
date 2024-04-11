/****************************************

* 作者：闪电黑客
* 日期：2024/4/11 20:07

* 描述：

*/

using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class NodeSendRuleGeneralGenerator
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
			Code.AppendLine("	public static class NodeSendRuleGeneral");
			Code.Append("	{");

			for (int i = 0; i <= argumentCount; i++)
			{
				string generics = RuleGeneratorHelper.GetGenerics(i);
				string genericsAfter = RuleGeneratorHelper.GetGenerics(i, true);
				string genericsAngle = RuleGeneratorHelper.GetGenericsAngle(i);
				string genericParameter = RuleGeneratorHelper.GetGenericParameter(i);
				string genericTypeParameter = RuleGeneratorHelper.GetGenericTypeParameter(i);
				Code.Append
($@"

		/// <summary>
		/// 尝试执行异步通知法则
		/// </summary>
		public static bool TrySend{genericsAngle}(this INode self{genericTypeParameter})
			=> self.TrySendRule(TypeInfo<ISendRule{genericsAngle}>.Default{genericParameter});

		/// <summary>
		/// 执行异步通知法则
		/// </summary>
		public static void Send<N{generics}>(this N self{genericTypeParameter})
			where N : class, INode, AsRule<ISendRule{genericsAngle}>
		=> self.SendRule(TypeInfo<ISendRule{genericsAngle}>.Default{genericParameter});
");
			}
			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("NodeSendRuleGeneral.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}