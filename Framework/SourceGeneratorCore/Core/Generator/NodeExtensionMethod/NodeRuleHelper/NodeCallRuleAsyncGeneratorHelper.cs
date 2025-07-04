﻿/****************************************

* 作者：闪电黑客
* 日期：2024/4/10 20:08

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal static class NodeCallRuleAsyncGeneratorHelper
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = ProjectGeneratorSetting.ArgumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 节点执行异步调用法则
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");
			Code.AppendLine("	public static partial class NodeRuleHelper");
			Code.Append("	{");

			for (int i = 0; i <= argumentCount; i++)
			{
				string genericsType = GeneratorTemplate.GenericsTypes[i];
				string genericsTypeAfter = GeneratorTemplate.GenericsTypesAfter[i];
				string genericParameter = GeneratorTemplate.GenericsParameter[i];
				string genericTypeParameter = GeneratorTemplate.GenericsTypeParameter[i];
				Code.AppendLine(
				$$"""

						/// <summary>
						/// 尝试执行异步调用法则
						/// </summary>
						public static async TreeTask<OutT> TryCallRuleAsync<R{{genericsType}}, OutT>(INode self, R nullRule{{genericTypeParameter}}, OutT defaultOutT)
							where R : ICallRuleAsync<{{genericsTypeAfter}}OutT>
						{
							if (self.Core.RuleManager.TryGetRuleList(self.Type, out IRuleList<R> ruleList))
							{
								return await ruleList.CallAsync(self{{genericParameter}}, defaultOutT);
							}
							await self.TreeTaskCompleted();
							return defaultOutT;
						}

						/// <summary>
						/// 执行异步调用法则
						/// </summary>
						public static async TreeTask<OutT> CallRuleAsync<N, R{{genericsType}}, OutT>(N self, R nullRule{{genericTypeParameter}}, OutT defaultOutT)
							where N : class, INode, AsRule<R>
							where R : ICallRuleAsync<{{genericsTypeAfter}}OutT>
						{
							if (self.Core.RuleManager.TryGetRuleList(self.Type, out IRuleList<R> ruleList))
							{
								return await ruleList.CallAsync(self{{genericParameter}}, defaultOutT);
							}
							await self.TreeTaskCompleted();
							return defaultOutT;
						}

						/// <summary>
						/// 尝试执行异步调用法则
						/// </summary>
						public static async TreeTask<TreeList<OutT>> TryCallsRuleAsync<R{{genericsType}}, OutT>(INode self, R nullRule{{genericTypeParameter}}, TreeList<OutT> outTList)
							where R : ICallRuleAsync<{{genericsTypeAfter}}OutT>
						{
							if (self.Core.RuleManager.TryGetRuleList(self.Type, out IRuleList<R> ruleList))
							{
								return await ruleList.CallsAsync(self{{genericParameter}}, outTList);
							}
							await self.TreeTaskCompleted();
							return null;
						}

						/// <summary>
						/// 执行异步调用法则
						/// </summary>
						public static async TreeTask<TreeList<OutT>> CallsRuleAsync<N, R{{genericsType}}, OutT>(N self, R nullRule{{genericTypeParameter}}, TreeList<OutT> outTList)
							where N : class, INode, AsRule<R>
							where R : ICallRuleAsync<{{genericsTypeAfter}}OutT>
						{
							if (self.Core.RuleManager.TryGetRuleList(self.Type, out IRuleList<R> ruleList))
							{
								return await ruleList.CallsAsync(self{{genericParameter}}, outTList);
							}
							await self.TreeTaskCompleted();
							return null;
						}
				"""
				);
			}
			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("NodeRuleHelperCallRuleAsync.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}