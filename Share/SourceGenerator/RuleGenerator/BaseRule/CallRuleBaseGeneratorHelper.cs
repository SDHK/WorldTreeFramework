/****************************************

* 作者：闪电黑客
* 日期：2024/4/7 17:37

* 描述：调用法则基类 生成器帮助类

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class CallRuleBaseGeneratorHelper
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 可以理解为Node的有返回值扩展方法
*
* ICallRuleBase 继承 IRule
* 主要作用：统一 调用方法  OutT Invoke(INode self,T1 arg1, ...);
*
* CallRuleBase 则继承 RuleBase
* 同时还继承了 ICallRuleBase 可以转换为 ICallRuleBase 进行统一调用。
*
* 主要作用：确定Node的类型并转换，并统一 Invoke 中转调用 Execute 的过程。
* 其中 Invoke 设定为虚方法方便子类写特殊的中转调用。
*/
"
);

			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");

			for (int i = 0; i <= argumentCount; i++)
			{
				string generics = RuleGeneratorHelper.GetGenerics(i, false);
				string GenericTypeParameter = RuleGeneratorHelper.GetGenericTypeParameter(i);

				Code.AppendLine("    /// <summary>");
				Code.AppendLine("    /// 调用法则基类接口");
				Code.AppendLine("    /// </summary>");
				Code.AppendLine($"    public interface ICallRuleBase<{generics}OutT> : IRule");
				Code.AppendLine("    {");
				Code.AppendLine($"        OutT Invoke(INode self{GenericTypeParameter});");
				Code.AppendLine("    }");
			}

			for (int i = 0; i <= argumentCount; i++)
			{
				string genericsPre = RuleGeneratorHelper.GetGenerics(i);
				string generics = RuleGeneratorHelper.GetGenerics(i, false);
				string GenericTypeParameter = RuleGeneratorHelper.GetGenericTypeParameter(i);
				string GenericParameter = RuleGeneratorHelper.GetGenericParameter(i);

				Code.AppendLine("    /// <summary>");
				Code.AppendLine("    /// 调用法则基类");
				Code.AppendLine("    /// </summary>");
				Code.AppendLine($"    public abstract class CallRuleBase<N, R{genericsPre}, OutT> : RuleBase<N, R>, ICallRuleBase<{generics}OutT>");
				Code.AppendLine("        where N : class, INode, AsRule<R>");
				Code.AppendLine($"        where R : ICallRuleBase<{generics}OutT>");
				Code.AppendLine("    {");

				Code.AppendLine($"        public virtual OutT Invoke(INode self{GenericTypeParameter}) => Execute(self as N{GenericParameter});");
				Code.AppendLine($"        protected abstract OutT Execute(N self{GenericTypeParameter});");
				Code.AppendLine("    }");
			}

			Code.AppendLine("}");

			context.AddSource("CallRuleBase.cs", SourceText.From(Code.ToString(), System.Text.Encoding.UTF8));//生成代码
		}
	}
}