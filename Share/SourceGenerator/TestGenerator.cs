using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WorldTree.SourceGenerator
{
    public class TestGenerator : ISourceGenerator
    {

        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            //context.AddSource
        }


    }

    class SyntaxContextReceiver : ISyntaxContextReceiver
    {
        //internal static ISyntaxContextReceiver Create(AttributeTemplate attributeTemplate) => new SyntaxContextReceiver(attributeTemplate);

        //private AttributeTemplate attributeTemplate;

        //SyntaxContextReceiver(AttributeTemplate attributeTemplate) => this.attributeTemplate = attributeTemplate;

        public Dictionary<ClassDeclarationSyntax, HashSet<MethodDeclarationSyntax>> MethodDeclarations { get; } = new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            SyntaxNode node = context.Node;
            if (node is not MethodDeclarationSyntax methodDeclarationSyntax) return;


            if (methodDeclarationSyntax.AttributeLists.Count == 0) return;

            bool found = false;
            foreach (AttributeListSyntax attributeListSyntax in methodDeclarationSyntax.AttributeLists)
            {
                AttributeSyntax? attribute = attributeListSyntax.Attributes.FirstOrDefault();
                if (attribute == null) return;

                string attributeName = attribute.Name.ToString();

                //if (this.attributeTemplate.Contains(attributeName)) found = true;
            }

            if (!found) return;

            ClassDeclarationSyntax? parentClass = methodDeclarationSyntax.GetParentClassDeclaration();
            if (parentClass == null) return;

            if (!MethodDeclarations.ContainsKey(parentClass)) MethodDeclarations[parentClass] = new HashSet<MethodDeclarationSyntax>();

            MethodDeclarations[parentClass].Add(methodDeclarationSyntax);
        }
    }



    public static class AnalyzerHelper
    {
        /// <summary>
        /// 获取语法节点所属的 类型声明语法 ClassDeclarationSyntax
        /// </summary>
        public static ClassDeclarationSyntax? GetParentClassDeclaration(this SyntaxNode syntaxNode)
        {
            SyntaxNode? parentNode = syntaxNode.Parent;
            while (parentNode != null)
            {
                if (parentNode is ClassDeclarationSyntax classDeclarationSyntax) return classDeclarationSyntax;

                parentNode = parentNode.Parent;
            }

            return null;
        }
    }
}