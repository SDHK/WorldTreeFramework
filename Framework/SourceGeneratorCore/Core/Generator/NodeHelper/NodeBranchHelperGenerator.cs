/****************************************

* 作者：闪电黑客
* 日期：2024/4/15 17:10

* 描述：

*/
using Microsoft.CodeAnalysis;
using System;

namespace WorldTree.SourceGenerator
{
	/// <summary>
	/// 节点添加到分支帮助方法生成器
	/// </summary>
	public abstract class NodeBranchHelperGenerator<C> : SourceGeneratorBase<C>
		where C : ProjectGeneratorsConfig, new()
	{
		public override void Initialize(GeneratorInitializationContext context)
		{
		}

		public override void ExecuteCore(GeneratorExecutionContext context)
		{
			try
			{
				var receiver = SyntaxReceiverHelper.Fill(context.Compilation, new FindCoreSyntaxReceiver());
				if (receiver.isGenerator == false) return;

				NodeBranchExtensionGeneratorHelper.Execute(context);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}
	}
}
