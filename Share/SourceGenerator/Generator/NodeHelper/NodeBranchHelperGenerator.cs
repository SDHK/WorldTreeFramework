/****************************************

* 作者：闪电黑客
* 日期：2024/4/15 17:10

* 描述：

*/
using Microsoft.CodeAnalysis;

namespace WorldTree.SourceGenerator
{
	[Generator]
	internal class NodeBranchHelperGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new FindCoreSyntaxReceiver());
		}

		public void Execute(GeneratorExecutionContext context)
		{
			try
			{
				if (!(context.SyntaxReceiver is FindCoreSyntaxReceiver receiver and not null)) return;
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
