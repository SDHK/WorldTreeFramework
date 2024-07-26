/****************************************

* 作者：闪电黑客
* 日期：2024/7/25 20:06

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal static class SendRefRuleBaseGenerator
	{

		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 可以理解为Node的无返回值扩展方法
*
* ISendRule 的引用类型版本 继承 IRule
* 主要作用：统一 调用方法 Invoke(INode self,ref T1 arg1, ...);
*
* 与 ISendRule 的区别在于参数为引用类型,可用于修改数值类型的参数。
* 这个版本应该是在特殊情况下使用。
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.Append("{");

			for (int i = 0; i <= argumentCount; i++)
			{
				string genericsType = GeneratorTemplate.GenericsTypes[i];
				string genericsTypeAngle = GeneratorTemplate.GenericsTypesAngle[i];
				string genericRefParameter = GeneratorTemplate.GenericsRefParameter[i];
				string genericRefTypeParameter = GeneratorTemplate.GenericsRefTypeParameter[i];

				Code.AppendLine(
					$$"""

						/// <summary>
						/// 通知法则基类接口：引用参数
						/// </summary>
						public interface ISendRefRule{{genericsTypeAngle}} : IRule
						{
							/// <summary>
							/// 调用
							/// </summary>
							void Invoke(INode self{{genericRefTypeParameter}});
						}

						/// <summary>
						/// 通知法则基类：引用参数
						/// </summary>
						public abstract class SendRefRule<N, R{{genericsType}}> : Rule<N, R>, ISendRefRule{{genericsTypeAngle}}
							where N : class, INode, AsRule<R>
							where R : ISendRefRule{{genericsTypeAngle}}
						{
							/// <summary>
							/// 调用
							/// </summary>
							public virtual void Invoke(INode self{{genericRefTypeParameter}}) => Execute(self as N{{genericRefParameter}});
							/// <summary>
							/// 执行
							/// </summary>
							protected abstract void Execute(N self{{genericRefTypeParameter}});
						}
					""");
			}

			Code.Append("}");

			context.AddSource("SendRefRule.cs", SourceText.From(Code.ToString(), System.Text.Encoding.UTF8));//生成代码
		}
	}
}