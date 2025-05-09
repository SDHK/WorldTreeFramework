/****************************************

* 作者：闪电黑客
* 日期：2025/5/9 14:44

* 描述：

*/
namespace WorldTree.SourceGenerator
{

	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.CodeAnalysis.Text;
	using System.Linq;
	using System.Text;

	[Generator]
	public class TranslateKeyFindGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context) { }

		public void Execute(GeneratorExecutionContext context)
		{
			var stringArgArguments = new StringBuilder();
			var stringArguments = new StringBuilder();

			// 遍历所有语法树
			foreach (var syntaxTree in context.Compilation.SyntaxTrees)
			{
				var semanticModel = context.Compilation.GetSemanticModel(syntaxTree);
				var root = syntaxTree.GetRoot();

				// 查找所有方法调用
				var translateInvocations = root.DescendantNodes()
					.OfType<InvocationExpressionSyntax>()
					.Where(invocation =>
					{
						// 判断 invocation 是一个方法调用，并且名称为 Translate
						if (invocation.Expression is MemberAccessExpressionSyntax memberAccess && memberAccess.Name.Identifier.Text == "Translate")
							return true;
						return false;
					});


				// 遍历找到的Translate方法调用，提取字符串参数
				foreach (var invocation in translateInvocations)
				{
					var argument = invocation.ArgumentList.Arguments.FirstOrDefault();
					if (argument != null)
					{
						var expr = argument.Expression;
						//获取invocation 所在的文件，行数
						var location = invocation.GetLocation();
						var lineSpan = location.GetLineSpan();
						var filePath = lineSpan.Path;
						var startLine = lineSpan.StartLinePosition.Line + 1;

						// 处理字符串字面量
						if (expr is LiteralExpressionSyntax stringLiteral)
						{
							stringArguments.AppendLine($"//\"{stringLiteral.Token.ValueText}\" \t| 文件: {filePath}, 行: {startLine}");
						}
						// 处理变量和其他表达式
						else
						{
							// 对于非字面量的字符串参数，可以添加变量名或表达式文本
							stringArgArguments.AppendLine($"//变量: {expr.ToString()} \t| 文件: {filePath}, 行: {startLine}");
						}
					}
				}
			}

			// 如果没有找到任何Translate调用，跳过当前语法树
			if (stringArguments.Length == 0 && stringArgArguments.Length == 0) return;

			// 生成代码文件内容
			var sourceBuilder = new StringBuilder();
			sourceBuilder.AppendLine("//找到的参数");
			sourceBuilder.Append(stringArguments.ToString());
			sourceBuilder.Append(stringArgArguments.ToString());

			//获取当前程序集名称
			var assemblyName = context.Compilation.Assembly.Name;

			// 添加生成的代码到编译上下文
			context.AddSource($"{assemblyName}TranslationKeys.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
		}
	}
}
