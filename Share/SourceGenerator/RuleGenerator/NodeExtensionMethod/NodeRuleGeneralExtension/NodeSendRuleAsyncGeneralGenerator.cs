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
				string genericsType = GeneratorTemplate.GenericsTypes[i];
				string genericsTypeAngle = GeneratorTemplate.GenericsTypesAngle[i];
				string genericParameter = GeneratorTemplate.GenericsParameter[i];
				string genericTypeParameter = GeneratorTemplate.GenericsTypeParameter[i];
			
				Code.AppendLine(
					$$"""

							/// <summary>
							/// 尝试执行异步通知法则
							/// </summary>
							public static TreeTask<bool> TrySendAsync{{genericsTypeAngle}}(this INode self{{genericTypeParameter}})
								=> self.TrySendRuleAsync(TypeInfo<ISendRuleAsync{{genericsTypeAngle}}>.Default{{genericParameter}});

							/// <summary>
							/// 执行异步通知法则
							/// </summary>
							public static TreeTask SendAsync<N{{genericsType}}>(this N self{{genericTypeParameter}})
								where N : class, INode, AsRule<ISendRuleAsync{{genericsTypeAngle}}>
							=> self.SendRuleAsync(TypeInfo<ISendRuleAsync{{genericsTypeAngle}}>.Default{{genericParameter}});
					""");
			}
			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("NodeSendRuleAsyncGeneral.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}