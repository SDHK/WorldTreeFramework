/****************************************

* 作者：闪电黑客
* 日期：2024/4/7 16:40

* 描述：异步通知法则基类 生成器帮助类

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	public static class SendRuleAsyncBaseGeneratorHelper
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = ProjectGeneratorSetting.ArgumentCount;

			StringBuilder Code = new StringBuilder();

			Code.AppendLine(
@$"/****************************************
* 可以理解为Node的无返回值异步方法
*
* ISendRuleAsync 继承 IRule
* 主要作用：统一 调用方法 Invoke(INode self,T1 arg1, ...);
*
* SendRuleAsync 则继承 Rule
* 同时还继承了 ISendRuleAsync 可以转换为 ISendRuleAsync 进行统一调用。
*
* 主要作用：确定Node的类型并转换，并统一 Invoke 中转调用 Execute 的过程。
* 其中 Invoke 设定为虚方法方便子类写特殊的中转调用。
*/
"
);

			Code.AppendLine("namespace WorldTree");
			Code.Append("{");

			for (int i = 0; i <= argumentCount; i++)
			{
				string genericsType = GeneratorTemplate.GenericsTypes[i];
				string genericsTypeAngle = GeneratorTemplate.GenericsTypesAngle[i];
				string genericParameter = GeneratorTemplate.GenericsParameter[i];
				string genericTypeParameter = GeneratorTemplate.GenericsTypeParameter[i];

				Code.AppendLine(
					$$"""

						/// <summary>
						/// 异步通知系统基类接口
						/// </summary>
						public interface ISendRuleAsync{{genericsTypeAngle}} : IRule
						{
							/// <summary>
							/// 调用
							/// </summary>
							TreeTask Invoke(INode self{{genericTypeParameter}});
						}

						/// <summary>
						/// 异步通知法则基类
						/// </summary>
						public abstract class SendRuleAsync<N, R{{genericsType}}> : Rule<N, R>, ISendRuleAsync{{genericsTypeAngle}}
							where N : class, INode, AsRule<R>
							where R : ISendRuleAsync{{genericsTypeAngle}}
						{
							/// <summary>
							/// 调用
							/// </summary>
							public virtual TreeTask Invoke(INode self{{genericTypeParameter}}) => Execute(self as N{{genericParameter}});
							/// <summary>
							/// 执行
							/// </summary>
							protected abstract TreeTask Execute(N self{{genericTypeParameter}});
						}

						/// <summary>
						/// 异步通知法则基类实现
						/// </summary>
						public abstract class SendRuleAsyncDefault< R{{genericsType}}> : Rule<Node, R>, ISendRuleAsync{{genericsTypeAngle}}
							where R : ISendRuleAsync{{genericsTypeAngle}}
						{
							/// <summary>
							/// 调用
							/// </summary>
							public virtual TreeTask Invoke(INode self{{genericTypeParameter}}) => Execute(self{{genericParameter}});
							/// <summary>
							/// 执行
							/// </summary>
							protected abstract TreeTask Execute(INode self{{genericTypeParameter}});
						}

					""");
			}

			Code.Append("}");

			context.AddSource("SendRuleAsync.cs", SourceText.From(Code.ToString(), System.Text.Encoding.UTF8));//生成代码
		}
	}
}