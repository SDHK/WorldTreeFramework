/****************************************

* 作者：闪电黑客
* 日期：2024/4/3 11:58

* 描述：通知法则基类 生成器

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	public static class SendRuleBaseGeneratorHelper
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = GeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 可以理解为Node的无返回值扩展方法
*
* ISendRule 继承 IRule
* 主要作用：统一 调用方法 Invoke(INode self,T1 arg1, ...);
*
* SendRule 则继承 Rule
* 同时还继承了 ISendRule 可以转换为 ISendRule 进行统一调用。
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
						/// 通知法则基类接口
						/// </summary>
						public interface ISendRule{{genericsTypeAngle}} : IRule
						{
							/// <summary>
							/// 调用
							/// </summary>
							void Invoke(INode self{{genericTypeParameter}});
						}

						/// <summary>
						/// 通知法则基类
						/// </summary>
						public abstract class SendRule<N, R{{genericsType}}> : Rule<N, R>, ISendRule{{genericsTypeAngle}}
							where N : class, INode, AsRule<R>
							where R : ISendRule{{genericsTypeAngle}}
						{
							/// <summary>
							/// 调用
							/// </summary>
							public virtual void Invoke(INode self{{genericTypeParameter}}) => Execute(self as N{{genericParameter}});
							/// <summary>
							/// 执行
							/// </summary>
							protected abstract void Execute(N self{{genericTypeParameter}});
						}

						/// <summary>
						/// 通知法则基类实现
						/// </summary>
						public abstract class SendRuleDefault<R{{genericsType}}> : Rule<INode, R>, ISendRule{{genericsTypeAngle}}
							where R : ISendRule{{genericsTypeAngle}}
						{
							/// <summary>
							/// 调用
							/// </summary>
							public virtual void Invoke(INode self{{genericTypeParameter}}) => Execute(self{{genericParameter}});
							/// <summary>
							/// 执行
							/// </summary>
							protected abstract void Execute(INode self{{genericTypeParameter}});
						}
					""");
			}

			Code.Append("}");

			context.AddSource("SendRule.cs", SourceText.From(Code.ToString(), System.Text.Encoding.UTF8));//生成代码
		}
	}
}