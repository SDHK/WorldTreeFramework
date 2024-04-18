using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace WorldTree.SourceGenerator
{
	[Generator]
	public class TestGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{
			//在编译过程开始时调用，用于初始化源生成器。
			//通常，你可以在这里注册一些需要分析的语法树节点、提供一些生成器的选项等。

			//context.RegisterForSyntaxNotifications(() => SyntaxContextReceiver.Create());
		}

		public void Execute(GeneratorExecutionContext context)
		{
			try
			{
				//在编译过程的代码生成阶段调用，用于执行实际的代码生成逻辑。
				//在这个方法中，你可以分析语法树、生成新的代码，并通过 context 参数将生成的代码添加到编译中。

				if (!(context.SyntaxReceiver is SyntaxContextReceiver receiver)) return;
				if (receiver.ClassDeclarations == null) return;

				NamespaceDeclarationSyntax namespaceDeclarationSyntax = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("WorldTree"));

				//法则静态兄弟类
				ClassDeclarationSyntax classDeclarationSyntax = SyntaxFactory.ClassDeclaration(receiver.ClassDeclarations.Identifier.ValueText + "Rule")
					.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
					.AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword))
					.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));

				//获取类名当作泛型参数
				TypeSyntax nodeType = SyntaxFactory.ParseTypeName(receiver.ClassDeclarations.Identifier.ValueText);

				//继承基类
				ClassDeclarationSyntax ruleBaseClass = SyntaxFactory.ClassDeclaration("AddRule");

				//泛型组装
				GenericNameSyntax genericNameSyntax = SyntaxFactory.GenericName(ruleBaseClass.Identifier)
				.WithTypeArgumentList(SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(nodeType)));

				ClassDeclarationSyntax ruleClass = SyntaxFactory.ClassDeclaration("AddRule_")
					.AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword))
					.AddBaseListTypes(SyntaxFactory.SimpleBaseType(genericNameSyntax));

				//方法参数
				ParameterSyntax parameterSyntax = SyntaxFactory.Parameter(SyntaxFactory.Identifier("self")).WithType(nodeType);

				//表达式参数
				ArgumentListSyntax argumentListSyntax = SyntaxFactory.ArgumentList(
					SyntaxFactory.SingletonSeparatedList(
						SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("代码生成 初始化！！！")))));

				//表达式组装
				ExpressionSyntax expressionSyntax = SyntaxFactory.InvocationExpression(
							SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("self"), SyntaxFactory.IdentifierName("Log")))
							.WithArgumentList(argumentListSyntax);

				//方法体
				BlockSyntax block = SyntaxFactory.Block(SyntaxFactory.ExpressionStatement(expressionSyntax));

				//方法组装
				MethodDeclarationSyntax methodDeclarationSyntax = SyntaxFactory.MethodDeclaration(

					//方法声明和返回值
					SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), "Execute")
					.AddModifiers(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword))
					.AddModifiers(SyntaxFactory.Token(SyntaxKind.OverrideKeyword))

					//方法参数组装到方法
					.AddParameterListParameters(parameterSyntax)

					//方法体组装到方法
					.WithBody(block);

				ruleClass = ruleClass.AddMembers(methodDeclarationSyntax);//方法组装到类
				classDeclarationSyntax = classDeclarationSyntax.AddMembers(ruleClass);//类中类组装
				namespaceDeclarationSyntax = namespaceDeclarationSyntax.AddMembers(classDeclarationSyntax);//静态类组装到命名空间

				//命名空间组装到编译单元
				CompilationUnitSyntax compilationUnitSyntax = SyntaxFactory.CompilationUnit().AddMembers(namespaceDeclarationSyntax).WithTrailingTrivia(SyntaxFactory.ElasticSpace);

				string Code = compilationUnitSyntax.NormalizeWhitespace().ToFullString();
				context.AddSource(classDeclarationSyntax.Identifier.ValueText + ".cs", SourceText.From(Code, System.Text.Encoding.UTF8));//生成代码
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		private class SyntaxContextReceiver : ISyntaxReceiver
		{
			internal static ISyntaxReceiver Create() => new SyntaxContextReceiver();

			public ClassDeclarationSyntax? ClassDeclarations;

			public void OnVisitSyntaxNode(SyntaxNode node)
			{
				if (node is not ClassDeclarationSyntax classDeclarationSyntax) return;

				SeparatedSyntaxList<BaseTypeSyntax> baseTypes = classDeclarationSyntax.BaseList?.Types ?? default;
				foreach (var baseType in baseTypes)
				{
					if (baseType.Type is IdentifierNameSyntax identifierNameSyntax && identifierNameSyntax.Identifier.ValueText == "Node")
					{
						ClassDeclarations = classDeclarationSyntax;
						return;
					}
				}
			}
		}
	}
}